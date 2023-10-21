using Godot;
using System;

public partial class Inventory : VBoxContainer
{
    Label label;
    TextureButton prev;
    TextureButton next;
    TextureButton use;
    Sprite2D prop;
    Sprite2D hand;
    Timer timer;
    Game game;

    Tween handOutro;
    Tween labelOutro;

    public override void _Ready()
    {
        game = GetNode<Game>("/root/Game");
        label = GetNode<Label>("Label");
        prev = GetNode<TextureButton>("ItemBar/Prev");
        next = GetNode<TextureButton>("ItemBar/Next");
        use = GetNode<TextureButton>("ItemBar/Use");
        prop = GetNode<Sprite2D>("ItemBar/Use/Prop");
        hand = GetNode<Sprite2D>("ItemBar/Use/Hand");
        timer = GetNode<Timer>("Label/Timer");

        hand.Hide();
        hand.Modulate = hand.Modulate with { A = 0.0f };

        label.Hide();
        label.Modulate = label.Modulate with { A = 0.0f };

        game.inventory.Changed += UpdateUI;
        prev.Pressed += game.inventory.SelectPrev;
        next.Pressed += game.inventory.SelectNext;
        use.Pressed += OnUsePressed;
        timer.Timeout += OnTimerTimeout;

        UpdateUI();
    }

    private void UpdateUI()
    {
        int count = game.inventory.GetItemCount();
        prev.Disabled = count < 2;
        next.Disabled = count < 2;
        Visible = count > 0;

        Item item = game.inventory.GetCurrentItem();
        if (item is null) return;
        label.Text = item.Description;
        prop.Texture = item.PropTexture;

        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(prop, "scale", Vector2.One, 0.15).From(Vector2.Zero);

        ShowLabel();
    }

    private void OnUsePressed()
    {
        game.inventory.activeItem = game.inventory.GetCurrentItem();

        if (handOutro is not null && handOutro.IsValid())
        {
            handOutro.Kill();
            handOutro = null;
        }
        hand.Show();
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back).SetParallel();
        tween.TweenProperty(hand, "scale", Vector2.One, 0.15).From(Vector2.Zero);
        tween.TweenProperty(hand, "modulate:a", 1.0, 0.15);

        ShowLabel();
    }

    public override async void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact") && game.inventory.activeItem is not null)
        {
            if (game.inventory.isInInteractable)
            {
                await ToSignal(game.inventory, Game.Inventory.SignalName.InteractFinished);
                HandFadeOut();
            }
            else
            {
                HandFadeOut();
            }
        }
    }

    private void OnTimerTimeout()
    {
        labelOutro = GetTree().CreateTween();
        labelOutro.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        labelOutro.TweenProperty(label, "modulate:a", 0.0, 0.2);
        labelOutro.TweenCallback(Callable.From(label.Hide));
    }

    private void ShowLabel()
    {
        if (labelOutro is not null && labelOutro.IsValid())
        {
            labelOutro.Kill();
            labelOutro = null;
        }

        label.Show();
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(label, "modulate:a", 1.0, 0.2);
        tween.TweenCallback(Callable.From(() => timer.Start()));
    }

    public void HandFadeOut()
    {
        game.inventory.SetDeferred("activeItem", default);
        handOutro = GetTree().CreateTween();
        handOutro.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine).SetParallel();
        handOutro.TweenProperty(hand, "scale", Vector2.One * 3, 0.15);
        handOutro.TweenProperty(hand, "modulate:a", 0.0, 0.15);
        handOutro.Chain().TweenCallback(Callable.From(hand.Hide));
    }


}
