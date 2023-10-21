using Godot;
using System;
using System.Linq;

public partial class DialogBubble : Control
{
	Label content;
	string[] dialogs = { "Deafult Dialog" };
	int currentLine = -1;


	public override void _Ready()
	{
		content = GetNode<Label>("Content");
		content.GuiInput += OnContentGuiInput;

		Hide();
	}

	public void ShowDialog(string[] _dialogs)
	{
		if (currentLine == -1 || !_dialogs.SequenceEqual(dialogs))
		{
			dialogs = _dialogs;
			ShowLine(0);
			Show();
		}
		else
		{
			Advance();
		}
	}

	private void ShowLine(int line)
	{
		currentLine = line;
		content.Text = dialogs[line];

		Tween tween = CreateTween();
		tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(this, "scale", Vector2.One, 0.2).From(Vector2.Zero);
	}

	private void Advance()
	{
		if (currentLine + 1 < dialogs.Length)
		{
			ShowLine(currentLine + 1);
		}
		else
		{
			currentLine = -1;
			Hide();
		}
	}

	private void OnContentGuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("interact"))
		{
			Advance();
		}
	}
}
