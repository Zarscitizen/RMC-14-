{
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.RMC.GhostWarp.UI;

public sealed partial class GhostWarpGroup : PanelContainer
{
    private Label GroupLabel => FindChild<Label>("GroupLabel");
    private VBoxContainer EntryContainer => FindChild<VBoxContainer>("EntryContainer");

    public GhostWarpGroup(string name, Color color)
    {
        InitializeComponent();
        GroupLabel.Text = name;
        Modulate = color;
    }

    public void AddEntry(GhostWarpEntry entry)
    {
        EntryContainer.AddChild(entry);
    }
}
namespace Content.Client._RMC14;
