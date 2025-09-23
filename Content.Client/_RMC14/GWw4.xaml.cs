public sealed partial class GhostWarpGroup : PanelContainer
{
    private Label GroupLabel => GetChild<VBoxContainer>("EntryContainer").GetChild<Label>("GroupLabel");
    private VBoxContainer EntryContainer => GetChild<VBoxContainer>("EntryContainer");

    public GhostWarpGroup(string name, Color color)
    {
        GroupLabel.Text = name;
        Modulate = color;
    }

    public void AddEntry(GhostWarpEntry entry)
    {
        EntryContainer.AddChild(entry);
    }
}