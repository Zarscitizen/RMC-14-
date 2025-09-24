// Shared/Net/RmcRosterDtos.cs
using Robust.Shared.Serialization;

namespace Content.Shared.RMC.Net;

[Serializable, NetSerializable]
public sealed class RmcRosterCategoryDto
{
    public string CategoryId = default!;      // например "marines", "xeno", ...
    public string CategoryName = default!;    // локализованный заголовок
    public string HexColor = "#FFFFFF";
    public List<RmcRosterGroupDto> Groups = new();
}

[Serializable, NetSerializable]
public sealed class RmcRosterGroupDto
{
    public string GroupId = default!;         // например "alpha", "bravo"
    public string GroupName = default!;       // локализованный заголовок или счётчик
    public List<RmcRosterItemDto> Items = new();
}

[Serializable, NetSerializable]
public sealed class RmcRosterItemDto
{
    public EntityUid Uid;
    public string DisplayName = default!;
    public bool IsDead;
}

[Serializable, NetSerializable]
public sealed class RmcRosterSnapshotMsg : EntityEventArgs
{
    public List<RmcRosterCategoryDto> Categories = new();
}

[Serializable, NetSerializable]
public sealed class RmcRosterDeltaMsg : EntityEventArgs
{
    public List<RmcRosterCategoryDto> Upserts = new(); // категории/группы/элементы для обновления/добавления
    public List<(string CategoryId, string? GroupId, EntityUid Uid)> Removes = new();
}

[Serializable, NetSerializable]
public sealed class RmcWarpRequestMsg : EntityEventArgs
{
    public EntityUid Target;
}
