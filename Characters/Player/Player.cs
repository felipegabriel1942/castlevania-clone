using Godot;

namespace CastlevaniaClone.Characters.Player;

public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed = 40f;


    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            var gravity = (float) ProjectSettings.GetSetting("physics/2d/default_gravity");
            Velocity += Vector2.Down * gravity * (float) delta;
        } else
        {
            float movementDirection = Input.GetAxis("Move_Left", "Move_Right");
            Velocity = new Vector2(movementDirection * Speed, 0); 
        }

        MoveAndSlide();
    }


}
