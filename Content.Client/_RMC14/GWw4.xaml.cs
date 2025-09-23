using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._RMC14;

public sealed partial class GhostWarpGroup : PanelContainer
{
    private Label GroupLabel => GetChild<Label>("GroupLabel");
    private BoxContainer EntryContainer => GetChild<BoxContainer>("EntryContainer");

    public GhostWarpGroup(string name, Color color)
    {
        RobustXamlLoader.Load(this);
        GroupLabel.Text = name;
        Modulate = color;
    }

    public void AddEntry(GhostWarpEntry entry)
    {
        EntryContainer.AddChild(entry);
    }
}
