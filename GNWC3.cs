// Client/Systems/RmcRosterClientSystem.cs
using Content.Client.RMC.UI;
using Content.Shared.RMC.Net;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.Player;

namespace Content.Client.RMC.Systems;

public sealed class RmcRosterClientSystem : EntitySystem
{
    [Dependency] private readonly IUserInterfaceManager _ui = default!;
    private RmcRosterControl? _control;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<RmcRosterSnapshotMsg>(OnSnapshot);
        SubscribeNetworkEvent<RmcRosterDeltaMsg>(OnDelta);
    }

    private void EnsureUi()
    {
        if (_control != null) return;
        _control = new RmcRosterControl();
        _control.OnWarpRequested += OnWarpRequested;
        // Вставь _control в существующее Ghost Warp меню или своё окно
        _ui.RootControl.AddChild(_control);
    }

    private void OnSnapshot(RmcRosterSnapshotMsg msg)
    {
        EnsureUi();
        _control!.LoadSnapshot(msg);
    }

    private void OnDelta(RmcRosterDeltaMsg msg)
    {
        EnsureUi();
        _control!.ApplyDelta(msg);
    }

    private void OnWarpRequested(EntityUid target)
    {
        // Клиент шлёт запрос, сервер валидирует (призрак ли)
        var req = new RmcWarpRequestMsg { Target = target };
        RaiseNetworkEvent(req);
    }
}
