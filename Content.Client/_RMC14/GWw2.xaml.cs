using System.Collections.Generic;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._RMC14;

public sealed partial class GhostWarpWindow : DefaultWindow
{
    private BoxContainer GroupContainer => GetChild<BoxContainer>("GroupContainer");

    public void AddGroup(string name, Color color, List<EntityUid> players)
    {
        var group = new GhostWarpGroup(name, color);

        foreach (var player in players)
        {
            var entry = new GhostWarpEntry(player);
            group.AddEntry(entry);
        }

        GroupContainer.AddChild(group);
    }
}
