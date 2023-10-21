using Godot;
using System;

public partial class ClickVisualizer : CanvasLayer
{
    public override void _Ready()
    {
        Layer = 99;
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("interact")) { return; }

        Sprite2D sprite = new Sprite2D();
        AddChild(sprite);
        sprite.Texture = GD.Load<Texture2D>("res://Assets/UI/click.svg");
        sprite.GlobalPosition = GetViewport().GetMousePosition();

        Tween tween = CreateTween();
        tween.TweenProperty(sprite, "scale", Vector2.One, 0.3).From(Vector2.One * new Vector2(0.9f, 0.9f));
        tween.Parallel().TweenProperty(sprite, "modulate:a", 1.0, 0.2).From(0.0);
        tween.TweenProperty(sprite, "modulate:a", 0.0, 0.3);
        tween.TweenCallback(Callable.From(sprite.QueueFree));
    }
}
