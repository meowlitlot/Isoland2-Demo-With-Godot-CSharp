using Godot;

public partial class H2A : Background
{
    H2ABoard board;
    Sprite2D gear;
    public override void _Ready()
    {
        base._Ready();
        board = GetNode<H2ABoard>("Board");
        gear = GetNode<Sprite2D>("Reset/Gear");
        GetNode<Interactable>("Reset").interact += OnResetInteract;
    }

    private void OnResetInteract()
    {
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(gear, "rotation_degrees", 360.0, 0.2).AsRelative();
        tween.TweenCallback(Callable.From(board.Reset));
    }
}
