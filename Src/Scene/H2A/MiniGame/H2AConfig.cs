using Godot;
using Godot.Collections;
using System;
using System.Linq;
using Array = Godot.Collections.Array;

[Tool]
public partial class H2AConfig : Resource
{
    public enum Slot
    {
        NULL, TIME, SUN, FISH, HILL, CROSS, CHOICE
    }
    public const int SlotSize = 7;

    public int[] placements;
    public Dictionary<Slot, Array> connections = new Dictionary<Slot, Array>();

    public H2AConfig()
    {
        placements = new int[SlotSize];
        foreach (int i in placements)
        {
            placements[i] = (int)Slot.NULL;
        }
        for (int i = 0; i <= SlotSize; i++)
        {
            connections.Add((Slot)i, new Array());
        }
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = new Array<Dictionary>()
        {
            new Dictionary()
            {
                {"name", "placements"},
                {"type", (int)Variant.Type.PackedInt32Array},
                {"usage", (int)PropertyUsageFlags.Storage}
            },

            new Dictionary()
            {
                {"name", "connections"},
                {"type", (int)Variant.Type.Dictionary},
                {"usage", (int)PropertyUsageFlags.Storage}
            }
        };

        for (int i = 1; i < SlotSize; i++)
        {
            properties.Add(new Dictionary
            {
                {"name", "placements/" + Enum.GetName(typeof(Slot), i)},
                {"type", (int)Variant.Type.Int},
                {"usage", (int)PropertyUsageFlags.Editor},
                {"hint", (int)PropertyHint.Enum},
                {"hint_string", "NULL, TIME, SUN, FISH, HILL, CROSS, CHOICE"}
            });
        }

        for (int i = 0; i < 6; i++)
        {
            System.Collections.Generic.List<string> available = new System.Collections.Generic.List<string>();
            for (int j = 0; j < SlotSize; j++)
            {
                if (j <= i)
                {
                    available.Add("");
                }
                else
                {
                    available.Add(Enum.GetName(typeof(Slot), j));
                }
            }
            properties.Add(new Dictionary
            {
                {"name", "connections/" + Enum.GetName(typeof(Slot), i)},
                {"type", (int)Variant.Type.Int},
                {"usage", (int)PropertyUsageFlags.Editor},
                {"hint", (int)PropertyHint.Flags},
                {"hint_string", available.ToArray().Join()}
            });
        }

        return properties;
    }

    public override Variant _Get(StringName property)
    {
        if (property.ToString().StartsWith("placements/"))
        {
            property = new StringName(property.ToString().TrimPrefix("placements/"));
            int index = (int)Enum.Parse(typeof(Slot), property.ToString());
            return placements[index];
        }

        if (property.ToString().StartsWith("connections/"))
        {
            property = new StringName(property.ToString().TrimPrefix("connections/"));
            int index = (int)Enum.Parse(typeof(Slot), property.ToString());
            int value = 0;
            for (int i = index + 1; i < SlotSize; i++)
            {
                if (connections[(Slot)index].Contains(i))
                {
                    value |= (1 << i);
                }
            }
            return value;
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property.ToString().StartsWith("placements/"))
        {
            property = new StringName(property.ToString().TrimPrefix("placements/"));
            int index = (int)Enum.Parse(typeof(Slot), property.ToString());
            placements[index] = (int)value;
            EmitChanged();
            return true;
        }

        if (property.ToString().StartsWith("connections/"))
        {
            property = new StringName(property.ToString().TrimPrefix("connections/"));
            int index = (int)Enum.Parse(typeof(Slot), property.ToString());
            for (int i = index + 1; i < SlotSize; i++)
            {
                SetConnected(index, i, Convert.ToBoolean(value.AsInt32() & (1 << i)));
            }
            EmitChanged();
            return true;
        }

        return false;
    }

    private void SetConnected(int src, int dst, bool connected)
    {
        Array src_arr = connections[(Slot)src];
        Array dst_arr = connections[(Slot)dst];
        int src_idx = src_arr.IndexOf(dst);
        int dst_idx = dst_arr.IndexOf(src);
        if (connected)
        {
            if (src_idx == -1) src_arr.Add(dst);
            if (dst_idx == -1) dst_arr.Add(src);
        }
        else
        {
            if (src_idx != -1) src_arr.RemoveAt(src_idx);
            if (dst_idx != -1) dst_arr.RemoveAt(dst_idx);
        }

    }
}
