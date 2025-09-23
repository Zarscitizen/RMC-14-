{
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;

namespace Content.Client._RMC14;
 
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
