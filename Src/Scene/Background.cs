using Godot;
using System;

public partial class Background : Sprite2D
{
    [Export(PropertyHint.File, "*.mp3")]
    public string musicOverride = "";
    public override void _Ready()
    {
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(this, "scale", Vector2.One, 0.3).From(Vector2.One * new Vector2(1.05f, 1.05f));
    }
}
