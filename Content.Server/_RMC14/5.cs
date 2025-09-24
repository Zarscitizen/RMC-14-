// Server/Systems/RmcRosterSystem.cs
using Content.Shared.RMC.Components;
using Content.Shared.RMC.Factions;
using Content.Shared.RMC.Net;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Server.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.RMC.Systems;

public sealed class RmcRosterSystem : EntitySystem
{
    private readonly Dictionary<ICommonSession, List<RmcRosterCategoryDto>> _perClientCache = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RmcIdentityComponent, ComponentInit>(OnIdentityInit);
        SubscribeLocalEvent<RmcIdentityComponent, ComponentRemove>(OnIdentityRemove);
        SubscribeLocalEvent<RmcLifeStateComponent, ComponentInit>(OnLifeInit);
        SubscribeLocalEvent<RmcLifeStateComponent, ComponentRemove>(OnLifeRemove);
        // Подпишитесь на свои события смены фракции/сквада, смерти/ревайва
    }

    private void OnIdentityInit(EntityUid uid, RmcIdentityComponent comp, ComponentInit args)
        => BroadcastUpsertFor(uid);

    private void OnIdentityRemove(EntityUid uid, RmcIdentityComponent comp, ComponentRemove args)
        => BroadcastRemoveFor(uid);

    private void OnLifeInit(EntityUid uid, RmcLifeStateComponent comp, ComponentInit args)
        => BroadcastUpsertFor(uid);

    private void OnLifeRemove(EntityUid uid, RmcLifeStateComponent comp, ComponentRemove args)
        => BroadcastUpsertFor(uid);

    public override void OnPlayerAttached(EntityUid playerEnt, ICommonSession session)
    {
        base.OnPlayerAttached(playerEnt, session);
        SendFullSnapshot(session);
    }

    private void SendFullSnapshot(ICommonSession session)
    {
        var categories = BuildCategoriesForSession(session);
        _perClientCache[session] = categories;
        var msg = new RmcRosterSnapshotMsg { Categories = categories };
        RaiseNetworkEvent(msg, session);
    }

    private void BroadcastUpsertFor(EntityUid uid)
    {
        var sessions = GetSessions();
        foreach (var session in sessions)
        {
            var upserts = BuildUpsertsForEntity(session, uid);
            if (upserts.Count == 0) continue;
            var msg = new RmcRosterDeltaMsg { Upserts = upserts };
            RaiseNetworkEvent(msg, session);
        }
    }

    private void BroadcastRemoveFor(EntityUid uid)
    {
        var sessions = GetSessions();
        foreach (var session in sessions)
        {
            // Определи прежнюю категорию/группу из кэша при необходимости.
            var removes = FindRemovesFromCache(session, uid);
            if (removes.Count == 0) continue;
            var msg = new RmcRosterDeltaMsg { Removes = removes };
            RaiseNetworkEvent(msg, session);
        }
    }

    private IEnumerable<ICommonSession> GetSessions()
        => EntityManager.EntityQuery<IPlayerManager>().First().Sessions;

    private List<RmcRosterCategoryDto> BuildCategoriesForSession(ICommonSession session)
    {
        // Собираем все сущности с RmcIdentityComponent
        var list = new List<RmcRosterCategoryDto>();
        var byCat = new Dictionary<string, RmcRosterCategoryDto>();

        foreach (var (uid, idComp) in EntityQuery<RmcIdentityComponent>())
        {
            if (!IsVisibleToSession(session, uid)) continue;

            var (catId, catName, catColor) = MapToCategory(idComp, uid);
            var (groupId, groupName) = MapToGroup(idComp);

            var item = new RmcRosterItemDto
            {
                Uid = uid,
                DisplayName = ResolveDisplayName(uid, idComp),
                IsDead = TryComp<RmcLifeStateComponent>(uid, out var life) && life.IsDead
            };

            var cat = byCat.GetOrNew(catId, () => new RmcRosterCategoryDto
            {
                CategoryId = catId,
                CategoryName = Loc.GetString(catName),
                HexColor = catColor
            });

            var group = cat.Groups.FirstOrDefault(g => g.GroupId == groupId);
            if (group == null)
            {
                group = new RmcRosterGroupDto
                {
                    GroupId = groupId,
                    GroupName = Loc.GetString(groupName)
                };
                cat.Groups.Add(group);
            }

            group.Items.Add(item);
        }

        // Счётчики в заголовках (например "Alpha (7)")
        foreach (var cat in byCat.Values)
        {
            foreach (var group in cat.Groups)
                group.GroupName = $"{group.GroupName} ({group.Items.Count})";

            // Итоговый счётчик категории
            var total = cat.Groups.Sum(g => g.Items.Count);
            cat.CategoryName = $"{cat.CategoryName} ({total})";
        }

        list.AddRange(byCat.Values);
        // Отсортируй по желаемому порядку:
        list = SortCategories(list);
        return list;
    }

    private List<RmcRosterCategoryDto> BuildUpsertsForEntity(ICommonSession session, EntityUid uid)
    {
        if (!TryComp(uid, out RmcIdentityComponent? idComp)) return new();
        if (!IsVisibleToSession(session, uid)) return new();
        var cat = BuildSingleCategoryFor(uid, idComp);
        return cat is null ? new() : new() { cat };
    }

    private List<(string CategoryId, string? GroupId, EntityUid Uid)> FindRemovesFromCache(ICommonSession session, EntityUid uid)
    {
        var removes = new List<(string, string?, EntityUid)>();
        if (!_perClientCache.TryGetValue(session, out var cats)) return removes;
        foreach (var cat in cats)
        {
            foreach (var group in cat.Groups)
            {
                if (group.Items.Any(i => i.Uid == uid))
                    removes.Add((cat.CategoryId, group.GroupId, uid));
            }
        }
        return removes;
    }

    private (string catId, string locKey, string hex) MapToCategory(RmcIdentityComponent id, EntityUid uid)
    {
        // Dead переопределяет основную категорию
        if (TryComp<RmcLifeStateComponent>(uid, out var life) && life.IsDead)
            return ("dead", "rmc-cat-dead", "#E24A4A");

        if (id.IsVehicle) return ("vehicles", "rmc-cat-vehicles", "#88C0D0");
        if (id.IsNPC) return ("npcs", "rmc-cat-npcs", "#D08770");

        return id.Faction switch
        {
            RmcFaction.Marines => ("marines", "rmc-cat-marines", "#A3BE8C"),
            RmcFaction.Xenomorphs => ("xeno", "rmc-cat-xeno", "#B48EAD"),
            RmcFaction.Survivors => ("survivors", "rmc-cat-survivors", "#EBCB8B"),
            RmcFaction.UPP => ("upp", "rmc-cat-upp", "#5E81AC"),
            RmcFaction.WeylandYutani => ("weyland", "rmc-cat-weyland", "#81A1C1"),
            RmcFaction.SecurityForces => ("secforces", "rmc-cat-secforces", "#8FBCBB"),
            _ => ("npcs", "rmc-cat-npcs", "#D08770")
        };
    }

    private (string groupId, string locKey) MapToGroup(RmcIdentityComponent id)
    {
        if (id.Faction == RmcFaction.Marines)
        {
            return id.Squad switch
            {
                MarineSquad.Alpha => ("alpha", "rmc-group-alpha"),
                MarineSquad.Bravo => ("bravo", "rmc-group-bravo"),
                MarineSquad.Charlie => ("charlie", "rmc-group-charlie"),
                MarineSquad.Delta => ("delta", "rmc-group-delta"),
                _ => ("company", "rmc-group-company") // общий раздел
            };
        }

        // Одногрупповые категории
        return ("prime", "rmc-group-prime");
    }

    private string ResolveDisplayName(EntityUid uid, RmcIdentityComponent id)
    {
        if (!string.IsNullOrWhiteSpace(id.CustomName)) return id.CustomName!;
        if (TryComp(uid, out MetaDataComponent? meta)) return meta.EntityName;
        return Loc.GetString("rmc-unnamed-entity");
    }

    private RmcRosterCategoryDto? BuildSingleCategoryFor(EntityUid uid, RmcIdentityComponent id)
    {
        var (catId, catKey, catColor) = MapToCategory(id, uid);
        var (groupId, groupKey) = MapToGroup(id);
        var cat = new RmcRosterCategoryDto
        {
            CategoryId = catId,
            CategoryName = Loc.GetString(catKey),
            HexColor = catColor,
            Groups = new()
            {
                new RmcRosterGroupDto
                {
                    GroupId = groupId,
                    GroupName = Loc.GetString(groupKey),
                    Items = new()
                    {
                        new RmcRosterItemDto
                        {
                            Uid = uid,
                            DisplayName = ResolveDisplayName(uid, id),
                            IsDead = TryComp<RmcLifeStateComponent>(uid, out var life) && life.IsDead
                        }
                    }
                }
            }
        };
        return cat;
    }

    private bool IsVisibleToSession(ICommonSession session, EntityUid target)
    {
        // Разрешение: призраки видят всех; живые — ограниченно (пример).
        var viewer = session.AttachedEntity;
        if (viewer == null) return false;

        if (HasComp<GhostComponent>(viewer.Value)) return true;

        // Живые видят только свою фракцию/сквад и союзных, без Dead.
        if (TryComp(target, out RmcIdentityComponent? id))
        {
            var sameFaction = TryComp(viewer.Value, out RmcIdentityComponent? vId) &&
                              vId.Faction == id.Faction;
            var notDead = !TryComp<RmcLifeStateComponent>(target, out var life) || !life.IsDead;
            return sameFaction && notDead;
        }

        return false;
    }

    private List<RmcRosterCategoryDto> SortCategories(List<RmcRosterCategoryDto> list)
    {
        var order = new Dictionary<string, int>
        {
            ["marines"] = 0,
            ["xeno"] = 1,
            ["survivors"] = 2,
            ["upp"] = 3,
            ["weyland"] = 4,
            ["secforces"] = 5,
            ["vehicles"] = 6,
            ["dead"] = 7,
            ["npcs"] = 8
        };

        list.Sort((a, b) => order.GetValueOrDefault(a.CategoryId, 999)
            .CompareTo(order.GetValueOrDefault(b.CategoryId, 999)));
        return list;
    }
}
