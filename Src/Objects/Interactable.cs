using Godot;
using System;

[GlobalClass, Tool]
public partial class Interactable : Area2D
{
	[Export]
	public bool allowItem;

	[Signal]
	public delegate void interactEventHandler();

	protected Game game;

	Texture2D _texture;
	[Export]
	public Texture2D texture
	{
		set
		{
			setTexture(value);
		}
		get => _texture;
	}

	public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;
		game = GetNode<Game>("/root/Game");
		MouseEntered += () => game.inventory.isInInteractable = true;
		MouseExited += () => game.inventory.isInInteractable = false;
	}

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (!@event.IsActionPressed("interact")) return;
		if (!allowItem && game.inventory.activeItem is not null)
		{
			GetNode<Inventory>("/root/Hud/Inventory").HandFadeOut();
			return;
		}
		Interact();
	}

	protected virtual void Interact()
	{
		EmitSignal(SignalName.interact);
	}

	protected void setTexture(Texture2D value)
	{
		_texture = value;

		foreach (var node in GetChildren())
		{
			if (node.Owner is null)
			{
				node.QueueFree();
			}
		}

		if (_texture is null) return;

		Sprite2D sprite = new Sprite2D
		{
			Texture = _texture
		};

		AddChild(sprite);

		RectangleShape2D rect = new RectangleShape2D
		{
			Size = value.GetSize()
		};

		CollisionShape2D collider = new CollisionShape2D
		{
			Shape = rect
		};

		AddChild(collider);

	}

}
