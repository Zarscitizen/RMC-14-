public sealed partial class GhostWarpEntry : Button
{
    public GhostWarpEntry(EntityUid target)
    {
        Text = MetaData(target).EntityName;
        OnPressed += _ =>
        {
            var ghost = IoCManager.Resolve<IPlayerManager>().LocalPlayer?.ControlledEntity;
            if (ghost != null)
                EntityManager.GetSystem<TeleportationSystem>().TryTeleport(ghost.Value, target);
        };
    }
}