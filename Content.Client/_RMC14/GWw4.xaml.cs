{
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
