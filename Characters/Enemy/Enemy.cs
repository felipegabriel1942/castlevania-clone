using Godot;

namespace CastlevaniaClone.Characters;

public partial class Enemy : CharacterBody2D
{
    [Export]
    public float Speed = 40f;

    private Area2D _hurtbox;
    private AnimatedSprite2D _animatedSprite2D;
    private Player.Player _player;
    private Node2D _visual;
    private Area2D _detectionArea;
    private Area2D _attackArea;
    private bool _isPursuing = false;
    private bool _isHurt = false;
    private bool _inAttackRange = false;
    private bool _isAttacking = false;
    private AnimationPlayer _animationPlayer;
    private Area2D _hitbox;
    private ShaderMaterial _material;

    public override void _Ready()
    {
        _hurtbox = GetNode<Area2D>("Hurtbox");
        _animatedSprite2D = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        _player = (Player.Player) GetTree().GetNodesInGroup("Player")[0];
        _visual = GetNode<Node2D>("Visual");
        _detectionArea = GetNode<Area2D>("%DetectionArea");
        _attackArea = GetNode<Area2D>("%AttackArea");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _hitbox = GetNode<Area2D>("%Hitbox");
        _material = (ShaderMaterial) _animatedSprite2D.Material;

        _animationPlayer.AnimationFinished += OnAnimationPlayerFinished;
        _animatedSprite2D.AnimationFinished += OnAnimationFinished;
        _detectionArea.BodyEntered += OnDetectionAreaEntered;
        _detectionArea.BodyExited += OnDetectionAreaExited;
        _attackArea.BodyEntered += OnAttackAreaEntered;
        _attackArea.BodyExited += OnAttackAreaExited;
        _hitbox.AreaEntered += OnHitboxCollide;
    }

    private void OnAnimationPlayerFinished(StringName animName)
    {
        if (animName == "Attack")
        {
            _isAttacking = false;
            _animatedSprite2D.Play("Idle");
        }
    }

    private void OnDetectionAreaExited(Node2D body)
    {
        _isPursuing = false;
    }

    private void OnAttackAreaExited(Node2D body)
    {
        _inAttackRange = false;
    }

    private void OnAttackAreaEntered(Node2D body)
    {
        _inAttackRange = true;
    }

    private void OnDetectionAreaEntered(Node2D body)
    {
        _isPursuing = true;
    }

    private void OnAnimationFinished()
    {
        if (_animatedSprite2D.Animation == "Hurt")
        {
            _isHurt = false;
            _animatedSprite2D.Play("Idle");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            var gravity = (float) ProjectSettings.GetSetting("physics/2d/default_gravity");
            Velocity += Vector2.Down * gravity * (float) delta;
        } else
        {
            if (_isHurt && !_isAttacking)
            {
                // _isAttacking = false;
                _animationPlayer.Stop(true);
                _animatedSprite2D.Play("Hurt");
                return;
            }

            if (_inAttackRange)
            {   
                if (!_isAttacking)
                {
                    _isAttacking = true;
                    _animationPlayer.Play("Attack");
                }

                return;
            }

            if (_isAttacking)
                return;

            if (!_isPursuing)
            {
                Velocity = Vector2.Zero;
                _animatedSprite2D.Play("Idle");
                return;
            }
                
            float direction = GlobalPosition.X < _player.GlobalPosition.X ? 1 : -1;
            Velocity = new Vector2(direction * Speed, 0);
            _visual.Scale = new Vector2(direction < 0 ? 1 : -1 , 1);
            _animatedSprite2D.Play("Run");
        }

        MoveAndSlide();
    }

    public void TakeDamage(int damage)
    {
        if (!_isAttacking)
        {
           _isHurt = true; 
        }

        FlashWhite();
    }

    private async void FlashWhite()
    {
        _material.SetShaderParameter("flash", true);

        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

        _material.SetShaderParameter("flash", false);
    }


    public void StartAttack()
    {
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
        if (area.GetParent() is Player.Player player)
        {
            player.TakeDamage(1);
        }
    }
}
