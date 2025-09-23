using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.GameObjects;
using Robust.Client.Player;

namespace Content.Client._RMC14;

public sealed partial class GhostWarpEntry : Button
{
    public GhostWarpEntry(EntityUid target)
    {
        InitializeComponent();
        Text = MetaData(target).EntityName;

        OnPressed += _ =>
        {
            var ghost = IoCManager.Resolve<IPlayerManager>().LocalPlayer?.ControlledEntity;
            if (ghost != null)
            {
                EntityManager.GetSystem<TeleportationSystem>().TryTeleport(ghost.Value, target);
            }
        };
    }
}
