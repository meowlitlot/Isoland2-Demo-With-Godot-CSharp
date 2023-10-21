using Godot;
using System;

public partial class H2 : Background
{
	DialogBubble dialogBubble;
	Game game;
	const string flag = "mail_accepted";
	public override void _Ready()
	{
		base._Ready();
		dialogBubble = GetNode<DialogBubble>("Granny/DialogBubble");
		game = GetNode<Game>("/root/Game");
		GetNode<Interactable>("Granny").interact += OnGrannyInteract;
	}
	private void OnGrannyInteract()
	{
		var item = game.inventory.activeItem;

		if (item is not null)
		{
			if (item == GD.Load<Item>("res://Src/Items/Mail.tres"))
			{
				game.flags.AddFlag(flag);
				game.inventory.RemoveItem(item);
				game.inventory.EmitInteractFinished();
			}
			else
			{
				return;
			}
		}

		if (game.flags.HasFlag(flag))
		{
			dialogBubble.ShowDialog(new string[] { "没想到老头子的船票寄过来了，谢谢你。" });
		}
		else
		{
			dialogBubble.ShowDialog(new string[] {
				"我年纪大了，很多事情想不起来了。",
				"你是谁？算了，我也不在乎你是谁。你能帮我找到信箱的钥匙吗？",
				"老头子说最近会给我寄船票过来，叫我和他一起出去看看。虽然我没有什么兴趣...",
				"他折腾了一辈子，不是躲在楼上捣鼓什么时间机器，就是出海找点什么东西。",
				"这些古怪的电视节目真没有什么意思。",
				"老头子说这个岛上有很多秘密，其实我知道，不过是岛上的日子太孤独，他找点事情做罢了。",
				"人嘛，谁没有年轻过。年轻的时候...算了，不说这些往事了。",
				"老了才明白，万物静默如迷。",
			});
		}
	}
}
