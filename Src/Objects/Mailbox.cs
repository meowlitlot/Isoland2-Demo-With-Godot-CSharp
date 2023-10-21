using Godot;
using System;

public partial class Mailbox : FlagSwitch
{
	Item item;
	public override void _Ready()
	{
		base._Ready();
		GetNode<Interactable>("MailBoxClose/Interactable").interact += OnInteractableInteract;
	}

	private void OnInteractableInteract()
	{
		item = game.inventory.activeItem;
		if (item is null || item != GD.Load<Item>("res://Src/Items/Key.tres")) return;
		game.flags.AddFlag(flag);
		game.inventory.RemoveItem(item);
		game.inventory.EmitInteractFinished();
	}
}
