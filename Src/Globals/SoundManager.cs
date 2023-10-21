using Godot;
using System;

public partial class SoundManager : Node
{
    AudioStreamPlayer bgmPlayer;
    public override void _Ready()
    {
        bgmPlayer = GetNode<AudioStreamPlayer>("BGMPlayer");
    }

    public void PlayMusic(string path)
    {
        if (bgmPlayer.Playing && bgmPlayer.Stream.ResourcePath == path) return;
        bgmPlayer.Stream = GD.Load<AudioStream>(path);
        bgmPlayer.Play();
    }
}
