{
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;

namespace Content.Client._RMC14;
    
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
