public sealed partial class GhostWarpWindow;
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
