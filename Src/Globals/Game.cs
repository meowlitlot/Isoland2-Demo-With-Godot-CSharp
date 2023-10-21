using Godot;
using Godot.Collections;
using System;

public partial class Game : Node
{
    public Flags flags;
    public Inventory inventory;
    const string SAVEPATH = "user://data.sav";
    public override void _Ready()
    {
        flags = new Flags();
        inventory = new Inventory();
    }
    public partial class Flags : Node
    {
        [Signal]
        public delegate void ChangedEventHandler();

        Array<string> _flags = new Array<string>();

        public bool HasFlag(string flag)
        {
            return _flags.Contains(flag);
        }

        public void AddFlag(string flag)
        {
            if (_flags.Contains(flag)) return;
            _flags.Add(flag);
            EmitSignal(SignalName.Changed);
        }

        public Dictionary<string, Array<string>> ToDict()
        {
            return new Dictionary<string, Array<string>>()
            {
                { "flags", new Array<string>(_flags) }
            };
        }

        public void FromDict(Dictionary<string, Array<string>> dict)
        {
            _flags = new Array<string>(dict["flags"]);
            EmitSignal(SignalName.Changed);
        }

        public void Reset()
        {
            _flags.Clear();
            EmitSignal(SignalName.Changed);
        }
    }

    public partial class Inventory : Node
    {
        [Signal]
        public delegate void ChangedEventHandler();
        [Signal]
        public delegate void InteractFinishedEventHandler();

        Array<Item> _items = new Array<Item>();
        int currentItemIndex = -1;

        public Item activeItem;

        public bool isInInteractable = false;

        public int GetItemCount()
        {
            return _items.Count;
        }

        public Item GetCurrentItem()
        {
            if (currentItemIndex == -1) return null;
            return _items[currentItemIndex];
        }

        public void AddItem(Item item)
        {
            if (_items.Contains(item)) return;
            _items.Add(item);
            currentItemIndex = _items.Count - 1;
            EmitSignal(SignalName.Changed);
        }

        public void RemoveItem(Item item)
        {
            int index = _items.IndexOf(item);
            if (index == -1) return;
            _items.RemoveAt(index);
            if (currentItemIndex >= _items.Count)
            {
                currentItemIndex = _items.Count == 0 ? -1 : 0;
            }
            EmitSignal(SignalName.Changed);
        }

        public void SelectNext()
        {
            if (currentItemIndex == -1) return;
            currentItemIndex = (currentItemIndex + 1) % _items.Count;
            EmitSignal(SignalName.Changed);
        }

        public void SelectPrev()
        {
            if (currentItemIndex == -1) return;
            currentItemIndex = (currentItemIndex - 1 + _items.Count) % _items.Count;
            EmitSignal(SignalName.Changed);
        }

        public void EmitInteractFinished()
        {
            EmitSignal(SignalName.InteractFinished);
        }

        public Dictionary<string, Variant> ToDict()
        {
            var names = new Array<string>();
            foreach (var item in _items)
            {
                var path = item.ResourcePath;
                names.Add(path.GetFile().GetBaseName());
            }
            return new Dictionary<string, Variant>()
            {
                {"items", names},
                {"current_item_index", currentItemIndex}
            };
        }

        public void FromDict(Dictionary<string, Variant> dict)
        {
            currentItemIndex = dict["current_item_index"].AsInt32();
            _items.Clear();
            foreach (var name in dict["items"].AsGodotArray())
            {
                _items.Add(GD.Load<Item>($"res://Src/Items/{name}.tres"));
            }
            EmitSignal(SignalName.Changed);
        }

        public void Reset()
        {
            _items.Clear();
            currentItemIndex = -1;
        }
    }


    public void SaveGame()
    {
        using var file = FileAccess.Open(SAVEPATH, FileAccess.ModeFlags.Write);
        if (file == null) return;
        var data = new Dictionary<string, Variant>()
        {
            {"inventory", inventory.ToDict()},
            {"flags", flags.ToDict()},
            {"current_scene", GetTree().CurrentScene.SceneFilePath}
        };
        var json = Json.Stringify(data);
        file.StoreString(json);
    }

    public void LoadGame()
    {
        using var file = FileAccess.Open(SAVEPATH, FileAccess.ModeFlags.Read);
        if (file == null) return;
        var json = file.GetAsText();
        var data = Json.ParseString(json).AsGodotDictionary<string, Variant>();
        inventory.FromDict(data["inventory"].AsGodotDictionary<string, Variant>());
        flags.FromDict(data["flags"].AsGodotDictionary<string, Array<string>>());
        GetNode<SceneChanger>("/root/SceneChanger").ChangeScene(data["current_scene"].AsString());
    }

    public void NewGame()
    {
        inventory.Reset();
        flags.Reset();
        GetNode<SceneChanger>("/root/SceneChanger").ChangeScene("res://Src/Scene/H1/H1.tscn");
    }

    public bool HasSaveFile()
    {
        return FileAccess.FileExists(SAVEPATH);
    }

    public void BackToTitle()
    {
        SaveGame();
        GetNode<SceneChanger>("/root/SceneChanger").ChangeScene("res://Src/UI/TitleScreen.tscn");
    }
}

