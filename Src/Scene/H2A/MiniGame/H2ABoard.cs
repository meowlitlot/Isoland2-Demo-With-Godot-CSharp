using Godot;
using Godot.Collections;
using System;
using System.Linq;

[Tool]
public partial class H2ABoard : Node2D
{
    [Export]
    Texture2D SlotTexture;
    [Export]
    Texture2D LineTexture;
    float radius = 100.0f;
    H2AConfig config;
    Dictionary<int, H2AStone> stoneMap = new Dictionary<int, H2AStone>();
    [Export]
    public float Radius
    {
        set
        {
            radius = value;
            QueueRedraw();
        }
        get => radius;
    }
    [Export]
    public H2AConfig Config
    {
        set
        {
            config = value;
            if (config != null && config.IsConnected(Resource.SignalName.Changed, Callable.From(UpdateBoard)))
            {
                config.Disconnect(Resource.SignalName.Changed, Callable.From(UpdateBoard));
            }
            if (config != null && !config.IsConnected(Resource.SignalName.Changed, Callable.From(UpdateBoard)))
            {
                config.Connect(Resource.SignalName.Changed, Callable.From(UpdateBoard));
            }
            UpdateBoard();
        }
        get => config;
    }

    public override void _Draw()
    {
        for (int i = 0; i < H2AConfig.SlotSize; i++)
        {
            DrawTexture(SlotTexture, GetSlotPosition(i) - SlotTexture.GetSize() / 2);
        }
    }

    private Vector2 GetSlotPosition(int slot)
    {
        return Vector2.Down.Rotated(Mathf.Tau / H2AConfig.SlotSize * slot) * new Vector2(radius, radius);
    }

    private void UpdateBoard()
    {
        foreach (Node node in GetChildren())
        {
            if (node.Owner is null) node.QueueFree();
        }

        if (config is null) return;

        for (int src = 0; src < H2AConfig.SlotSize; src++)
        {
            for (int dst = src; dst < H2AConfig.SlotSize; dst++)
            {
                if (!config.connections[(H2AConfig.Slot)src].Contains(dst))
                {
                    continue;
                }

                Line2D line = new Line2D();
                AddChild(line);
                line.AddPoint(GetSlotPosition(src));
                line.AddPoint(GetSlotPosition(dst));
                line.Width = LineTexture.GetSize().Y;
                line.Texture = LineTexture;
                line.TextureMode = Line2D.LineTextureMode.Tile;
                line.DefaultColor = Colors.White;
                line.ShowBehindParent = true;
            }
        }

        for (int slot = 1; slot < H2AConfig.SlotSize; slot++)
        {
            var stone = new H2AStone();
            AddChild(stone);
            stone.TargetSlot = slot;
            stone.CurrentSlot = config.placements[slot];
            stone.Position = GetSlotPosition(stone.CurrentSlot);
            stoneMap[slot] = stone;
            stone.Connect(Interactable.SignalName.interact, Callable.From(() => RequestMove(stone)));
        }
    }

    private void RequestMove(H2AStone stone)
    {
        var available = Enum.GetNames(typeof(H2AConfig.Slot)).ToList();
        foreach (var s in stoneMap.Values)
        {
            available.Remove(Enum.GetName(typeof(H2AConfig.Slot), s.CurrentSlot));
        }
        System.Diagnostics.Debug.Assert(available.Count == 1);
        var availableSlot = (int)Enum.Parse(typeof(H2AConfig.Slot), available.First());
        if (config.connections[(H2AConfig.Slot)stone.CurrentSlot].Contains(availableSlot))
        {
            MoveStone(stone, availableSlot);
        }
    }

    private void MoveStone(H2AStone stone, int slot)
    {
        stone.CurrentSlot = slot;
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(stone, "position", GetSlotPosition(slot), 0.2);
        tween.TweenCallback(Callable.From(Check));
    }

    private void Check()
    {
        foreach (var stone in stoneMap.Values)
        {
            if (stone.CurrentSlot != stone.TargetSlot) return;
        }
        GetNode<Game>("/root/Game").flags.AddFlag("h2a_unlocked");
        GetNode<SceneChanger>("/root/SceneChanger").ChangeScene("res://Src/Scene/H2/H2.tscn");
    }

    public void Reset()
    {
        foreach (var stone in stoneMap.Values)
        {
            MoveStone(stone, config.placements[stone.TargetSlot]);
        }
    }
}
