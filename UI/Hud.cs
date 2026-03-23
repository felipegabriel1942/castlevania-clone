using Godot;
using CastlevaniaClone.Scenes;
using System;

namespace CastlevaniaClone.UI;

public partial class Hud : Control
{
    private TextureProgressBar _textureProgressBar;

    public override void _Ready()
    {
        _textureProgressBar = GetNode<TextureProgressBar>("%TextureProgressBar");

        GameEvents.Instance.Connect(GameEvents.SignalName.PlayerMaxHealth, Callable.From<int>(OnSetPlayerMaxHealth));
        GameEvents.Instance.Connect(GameEvents.SignalName.PlayerCurrentHealth, Callable.From<int>(OnPlayerCurrentHealthChanged));
    }

    private void OnPlayerCurrentHealthChanged(int maxValue)
    {
        _textureProgressBar.Value = maxValue;
    }

    private void OnSetPlayerMaxHealth(int maxValue)
    {
        _textureProgressBar.MaxValue = maxValue;
        _textureProgressBar.Value = maxValue;
    }
}
