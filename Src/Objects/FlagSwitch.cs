using Godot;
using System;

[GlobalClass]
public partial class FlagSwitch : Node2D
{
    [Export]
    protected string flag;
    protected Node2D defaultNode;     // 默认节点
    protected Node2D switchNode;      // flag触发后切换的节点
    protected Game game;

    public override void _Ready()
    {
        game = GetNode<Game>("/root/Game");

        int count = GetChildCount();
        if (count > 0) defaultNode = GetChild<Node2D>(0);
        if (count > 1) switchNode = GetChild<Node2D>(1);

        game.flags.Changed += UpdateNodes;

        UpdateNodes();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        game.flags.Changed -= UpdateNodes;
    }

    protected void UpdateNodes()
    {
        bool exists = game.flags.HasFlag(flag);
        if (defaultNode != null) defaultNode.Visible = !exists;
        if (switchNode != null) switchNode.Visible = exists;
    }
}
