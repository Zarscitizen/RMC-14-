// Shared/Components/RmcLifeStateComponent.cs
using Robust.Shared.GameObjects;

namespace Content.Shared.RMC.Components;

/// Состояние жизни для раздела Dead.
[RegisterComponent]
public sealed partial class RmcLifeStateComponent : Component
{
    public bool IsDead;
}
