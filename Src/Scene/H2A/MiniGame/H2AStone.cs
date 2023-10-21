using Godot;

[Tool]
public partial class H2AStone : Interactable
{
    int targetSlot;
    int currentSlot;

    public int TargetSlot
    {
        set
        {
            targetSlot = value;
            UpdateTexture();
        }
        get => targetSlot;
    }

    public int CurrentSlot
    {
        set
        {
            currentSlot = value;
            UpdateTexture();
        }
        get => currentSlot;
    }

    private void UpdateTexture()
    {
        int index = targetSlot;
        if (targetSlot != currentSlot)
        {
            index += H2AConfig.SlotSize - 1;
        }
        string path = $"res://Assets/H2A/SS_{index:D2}.png";
        setTexture(GD.Load<Texture2D>(path));
    }
}
