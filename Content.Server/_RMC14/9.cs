// Server/Systems/RmcWarpSystem.cs
using Content.Shared.RMC.Net;
using Robust.Shared.Player;

namespace Content.Server.RMC.Systems;

public sealed class RmcWarpSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<RmcWarpRequestMsg>(OnWarpRequest);
    }

    private void OnWarpRequest(RmcWarpRequestMsg msg, EntitySessionEventArgs args)
    {
        var session = args.SenderSession;
        var ent = session.AttachedEntity;
        if (ent is null || !HasComp<GhostComponent>(ent.Value))
            return; // отказ: не призрак

        if (!EntityManager.EntityExists(msg.Target)) return;

        // Телепорт камеры/призрака к цели
        Transform(ent.Value).Coordinates = Transform(msg.Target).Coordinates;
    }
}
