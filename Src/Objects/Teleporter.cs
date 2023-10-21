using Godot;

[GlobalClass, Tool]
public partial class Teleporter : Interactable
{
    [Export(PropertyHint.File, "*.tscn")]
    string targetPath;

    SceneChanger sceneChanger;
    public override void _Ready()
    {
        base._Ready();
        if (Engine.IsEditorHint()) return;
        sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
    }

    protected override void Interact()
    {
        base.Interact();
        sceneChanger.ChangeScene(targetPath);
        game.inventory.EmitInteractFinished();
    }
}
