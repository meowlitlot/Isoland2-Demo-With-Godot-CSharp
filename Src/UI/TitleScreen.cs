using Godot;
using System;

public partial class TitleScreen : TextureRect
{
    Button newGame;
    Button loadGame;
    Button quit;
    Game game;
    public override void _Ready()
    {
        newGame = GetNode<Button>("VBoxContainer/New");
        loadGame = GetNode<Button>("VBoxContainer/Load");
        quit = GetNode<Button>("VBoxContainer/Quit");
        game = GetNode<Game>("/root/Game");

        newGame.Pressed += () => { game.NewGame(); };
        loadGame.Pressed += () => { game.LoadGame(); };
        quit.Pressed += () => { GetTree().Quit(); };

        loadGame.Disabled = !game.HasSaveFile();
    }
}
