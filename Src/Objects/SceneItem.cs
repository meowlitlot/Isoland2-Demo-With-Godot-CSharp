using System.Reflection.Metadata;
using Godot;
using System;

[GlobalClass, Tool]
public partial class SceneItem : Interactable
{
    Item _item;

    [Export]
    public Item item
    {
        set
        {
            _item = value;
            setTexture(_item?.SceneTexture);
        }
        get => _item;
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
        base._Ready();
        if (game.flags.HasFlag(GetFlag())) QueueFree();
    }

    protected override void Interact()
    {
        base.Interact();

        game.flags.AddFlag(GetFlag());
        game.inventory.AddItem(item);

        Sprite2D sprite = new Sprite2D
        {
            Texture = _item.SceneTexture
        };

        GetParent().AddChild(sprite);
        sprite.GlobalPosition = GlobalPosition;

        Tween tween = sprite.CreateTween();
        tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(sprite, "scale", Vector2.Zero, 0.15);
        tween.TweenCallback(Callable.From(sprite.QueueFree));

        QueueFree();
    }

    private string GetFlag()
    {
        return "picked:" + _item.ResourcePath.GetFile();
    }
}
