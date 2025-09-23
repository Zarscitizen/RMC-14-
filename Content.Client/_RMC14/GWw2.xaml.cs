public sealed partial class GhostWarpWindow : Window
{
    private VBoxContainer GroupContainer => GetChild<VBoxContainer>("GroupContainer");

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