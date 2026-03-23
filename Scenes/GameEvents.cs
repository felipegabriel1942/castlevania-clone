using Godot;

namespace CastlevaniaClone.Scenes;

public partial class GameEvents : Node
{
    
    public static GameEvents Instance { get; private set; }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }

    [Signal]
    public delegate void PlayerMaxHealthEventHandler(int health);

    [Signal]
    public delegate void PlayerCurrentHealthEventHandler(int currentHealth);

    public static void EmitPlayerMaxHealth(int health)
    {
        Instance.EmitSignal(SignalName.PlayerMaxHealth, health);
    }

    public static void EmitPlayerCurrentHealth(int currentHealth)
    {
        Instance.EmitSignal(SignalName.PlayerCurrentHealth, currentHealth);
    }
}
