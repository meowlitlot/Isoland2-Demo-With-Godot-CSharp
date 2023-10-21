using Godot;
using System;

public partial class SceneChanger : CanvasLayer
{
    ColorRect colorRect;
    SoundManager soundManager;
    public override void _Ready()
    {
        colorRect = GetNode<ColorRect>("ColorRect");
        soundManager = GetNode<SoundManager>("/root/SoundManager");
        OnSceneChanged(null, GetTree().CurrentScene);
    }

    [Signal] public delegate void GameEnteredEventHandler();
    [Signal] public delegate void GameExitedEventHandler();

    public void ChangeScene(string path)
    {
        Tween tween = CreateTween();
        tween.TweenCallback(Callable.From(colorRect.Show));
        tween.TweenProperty(colorRect, "color:a", 1.0, 0.2);
        tween.TweenCallback(Callable.From(() => { _ChangeScene(path); }));
        tween.TweenProperty(colorRect, "color:a", 0.0, 0.3);
        tween.TweenCallback(Callable.From(colorRect.Hide));
    }

    private void _ChangeScene(string path)
    {
        var oldScene = GetTree().CurrentScene;
        var newScene = GD.Load<PackedScene>(path).Instantiate();

        var root = GetTree().Root;
        root.RemoveChild(oldScene);
        root.AddChild(newScene);
        GetTree().CurrentScene = newScene;

        OnSceneChanged(oldScene, newScene);

        oldScene.QueueFree();
    }

    private void OnSceneChanged(Node oldScene, Node newScene)
    {
        var wasInGame = oldScene is Background;
        var isInGame = newScene is Background;
        if (wasInGame != isInGame)
        {
            if (isInGame) { EmitSignal(SignalName.GameEntered); }
            else { EmitSignal(SignalName.GameExited); }
        }

        var music = "res://Assets/Music/PaperWings.mp3";
        if (isInGame && (newScene as Background).musicOverride != "") music = (newScene as Background).musicOverride;
        soundManager.PlayMusic(music);
    }
}
