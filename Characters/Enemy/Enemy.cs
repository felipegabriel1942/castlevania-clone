using Godot;

namespace CastlevaniaClone.Characters;

public partial class Enemy : CharacterBody2D
{

    private Area2D _hurtbox;
    private AnimatedSprite2D _animatedSprite2D;


    public override void _Ready()
    {
        _hurtbox = GetNode<Area2D>("Hurtbox");
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _animatedSprite2D.AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished()
    {
        if (_animatedSprite2D.Animation == "Hurt")
        {
            _animatedSprite2D.Play("Idle");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            var gravity = (float) ProjectSettings.GetSetting("physics/2d/default_gravity");
            Velocity += Vector2.Down * gravity * (float) delta;
        }

        MoveAndSlide();
    }

    public void TakeDamage(int damage)
    {
        _animatedSprite2D.Play("Hurt");
    }
}
