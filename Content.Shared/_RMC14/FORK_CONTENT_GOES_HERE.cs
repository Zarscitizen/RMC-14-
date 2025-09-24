// Shared/Components/RmcLifeStateComponent.cs
{
using Robust.Shared.GameObjects;

namespace Content.Shared.RMC.Components;

/// Состояние жизни для раздела Dead.
[RegisterComponent]
public sealed partial class RmcLifeStateComponent : Component
{
    public bool IsDead;
}
// Shared/Components/RmcIdentityComponent.cs
using Robust.Shared.GameObjects;
}

namespace Content.Shared.RMC.Components;

/// Универсальный маркер для категоризации и отображаемого имени.
[RegisterComponent]
public sealed partial class RmcIdentityComponent : Component
{
    public string? CustomName;           // Если null, использовать MetaDataComponent для имени
    public RmcFaction? Faction;          // Основная фракция
    public MarineSquad Squad = MarineSquad.None; // Для Marines
    public bool IsNPC;                   // Пометка NPC
    public bool IsVehicle;               // Пометка транспортного средства
}

