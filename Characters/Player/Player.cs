using System;
using Godot;

namespace CastlevaniaClone.Characters.Player;

public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed = 40f;
    [Export]
    public float JumpForce = -300f;

    private AnimatedSprite2D _animatedSprite2D;

    private bool _isAttacking;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite2D.AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished()
    {
        if (_animatedSprite2D.Animation == "Attack")
        {
            _isAttacking = false;
        }
    }


    public override void _PhysicsProcess(double delta)
    {
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
            } else if (IsOnFloor() && Input.IsActionJustPressed("Attack"))
            {
                _isAttacking = true;
                Velocity = Vector2.Zero;
                _animatedSprite2D.Play("Attack");
            } else if (IsMoving())
            {
                float movementDirection = Input.GetAxis("Move_Left", "Move_Right");
                Velocity = new Vector2(movementDirection * Speed, 0);
                _animatedSprite2D.FlipH = movementDirection < 0;
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


}
