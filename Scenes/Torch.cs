using Godot;

namespace CastlevaniaClone.Scenes;

public partial class Torch : PointLight2D
{
    
    public override void _Process(double delta)
    {
        Energy = 1.1f + Mathf.Sin(Time.GetTicksMsec() * 0.01f) * 0.15f;
    }
}
