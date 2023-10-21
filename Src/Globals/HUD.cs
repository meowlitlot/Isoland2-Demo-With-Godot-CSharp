using Godot;
using System;

public partial class HUD : CanvasLayer
{
    public override void _Ready()
    {
        GetNode<TextureButton>("Menu").Pressed += () => { GetNode<Game>("/root/Game").BackToTitle(); };
        GetNode<SceneChanger>("/root/SceneChanger").GameEntered += Show;
        GetNode<SceneChanger>("/root/SceneChanger").GameExited += Hide;
        Visible = GetTree().CurrentScene is Background;
    }
}
