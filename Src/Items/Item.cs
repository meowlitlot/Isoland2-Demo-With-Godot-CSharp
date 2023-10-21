using Godot;
using System;

[GlobalClass, Tool]
public partial class Item : Resource
{
    [Export]
    public string Description { get; set; }
    [Export]
    public Texture2D PropTexture { get; set; }
    [Export]
    public Texture2D SceneTexture { get; set; }
}
