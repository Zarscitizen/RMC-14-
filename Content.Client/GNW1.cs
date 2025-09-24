// Shared/FactionTypes.cs
namespace Content.Shared.RMC.Factions;

public enum RmcFaction
{
    Marines,
    Xenomorphs,
    Survivors,
    UPP,
    WeylandYutani,
    SecurityForces,
    Vehicles,
    Dead,     // используется как категория вывода, но заполняется из состояния смерти
    NPCs
}

public enum MarineSquad
{
    None,
    Alpha,
    Bravo,
    Charlie,
    Delta
}
