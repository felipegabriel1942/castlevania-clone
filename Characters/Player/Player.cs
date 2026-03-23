using Godot;
using System.Collections.Generic;
using CastlevaniaClone.Scenes;

namespace CastlevaniaClone.Characters.Player;

public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed = 40f;

    [Export]
    public float JumpForce = -300f;

    [Export]
    public int Health = 2;

    [Export]
    private int Stamina = 2;

    private AnimatedSprite2D _animatedSprite2D;
    private AnimationPlayer _animationPlayer;
    private Node2D _visual;
    private Area2D _hitbox;
    private Timer _staminaTimer;
    private bool _isAttacking;
    private bool _isHurt;
    private bool _isDead;
    private int _currentHealth;
    private int _currentStamina;
    private HashSet<Node> _enemiesHit = new HashSet<Node>();

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _hitbox = GetNode<Area2D>("%Hitbox");
        _visual = GetNode<Node2D>("Visual");
        _staminaTimer = GetNode<Timer>("Timer");
        _currentHealth = Health;
        _currentStamina = Stamina;

        AddToGroup("Player");

        _animationPlayer.AnimationFinished += AnimationFinished;
        _animatedSprite2D.AnimationFinished += OnAnimationFinished;
        _hitbox.AreaEntered += OnHitboxCollide;
        _staminaTimer.Timeout += OnTimerTimeout;
        
        GameEvents.EmitPlayerMaxHealth(Health);
    }

    private void OnTimerTimeout()
    {
        if (_currentStamina < Stamina)
        {
            _currentStamina += 1;
        }
    }

    private void OnAnimationFinished()
    {
        if (_animatedSprite2D.Animation == "Hurt")
        {
            _isHurt = false;
            _animatedSprite2D.Play("Idle");
        }
    }

    private void AnimationFinished(StringName animName)
    {
        if (animName == "Attack")
        {
            _isAttacking = false;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead)
        {
            return;
        }

        if (_isHurt)
        {
            _animatedSprite2D.Play("Hurt");
            return;
        }

        if (!_isAttacking)
        {
            if (!IsOnFloor())
            {
                var gravity = (float) ProjectSettings.GetSetting("physics/2d/default_gravity");
                Velocity += Vector2.Down * gravity * (float) delta;
                _animatedSprite2D.Play("Fall");
            } else if (IsOnFloor() && Input.IsActionJustPressed("Jump"))
            {
                Velocity = new Vector2(Velocity.X, JumpForce);
            } else if (IsOnFloor() && Input.IsActionJustPressed("Attack") && _currentStamina > 0)
            {
                _isAttacking = true;
                Velocity = Vector2.Zero;
                _animationPlayer.Play("Attack");
            } else if (IsMoving())
            {
                float direction = Input.GetAxis("Move_Left", "Move_Right");
                Velocity = new Vector2(direction * Speed, 0);

                if (direction != 0)
                {
                    _visual.Scale = new Vector2(direction > 0 ? 1 : -1 , 1);
                }
                
                _animatedSprite2D.Play("Run");
            } else
            {
                Velocity = new Vector2(0 * Speed, 0);
                _animatedSprite2D.Play("Idle");
            }
        }
        
        MoveAndSlide();
    }

    private bool IsMoving()
    {
        return Input.GetAxis("Move_Left", "Move_Right") != 0;
    }

    public void StartAttack()
    {
        _enemiesHit.Clear();
        _currentStamina = Mathf.Max(0, _currentStamina - 1);
        _staminaTimer.Start();
        _hitbox.Monitoring = true;
        _hitbox.Monitorable = true;
    }

    public void EndAttack()
    {
        _hitbox.Monitoring = false;
        _hitbox.Monitorable = false;
    }

    private void OnHitboxCollide(Area2D area)
    {
        if (area.GetParent() is Enemy enemy)
        {
            if (_enemiesHit.Contains(enemy))
                return;

            enemy.TakeDamage(1);
            _enemiesHit.Add(enemy);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDead)
            return;

        _animationPlayer.Stop(true);
        
        _isHurt = true;
        _currentHealth -= damage;
        _currentHealth = _currentHealth < 0 ? 0 : _currentHealth;

        GameEvents.EmitPlayerCurrentHealth(_currentHealth);

        if (_currentHealth == 0)
        {
            Die();
        }
    }

    private async void Die()
    {
        _isDead = true;
        _animatedSprite2D.Play("Death");

        await ToSignal(GetTree().CreateTimer(4f), "timeout");

        GetTree().ReloadCurrentScene();
    }
}
