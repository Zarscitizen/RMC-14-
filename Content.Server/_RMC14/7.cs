// Client/UI/RmcRosterControl.cs
using Content.Shared.RMC.Net;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.RMC.UI;

public sealed class RmcRosterControl : Control
{
    private VBox _categoryList = default!;
    public event Action<EntityUid>? OnWarpRequested;

    public override void EnteredTree()
    {
        _categoryList = FindChild<VBox>("CategoryList");
    }

    public void LoadSnapshot(RmcRosterSnapshotMsg msg)
    {
        _categoryList.DisposeAllChildren();
        foreach (var cat in msg.Categories)
            _categoryList.AddChild(BuildCategory(cat));
    }

    public void ApplyDelta(RmcRosterDeltaMsg msg)
    {
        foreach (var up in msg.Upserts)
            UpsertCategory(up);

        foreach (var rem in msg.Removes)
            RemoveItem(rem.CategoryId, rem.GroupId, rem.Uid);
    }

    private Control BuildCategory(RmcRosterCategoryDto cat)
    {
        var header = new Label { Text = cat.CategoryName };
        header.Modulate = Color.FromHex(cat.HexColor);

        var collapsible = new CollapsiblePanel
        {
            Header = header,
            Collapsed = false
        };

        var groupsBox = new VBox { Separation = 2 };
        collapsible.SetBody(groupsBox);

        foreach (var grp in cat.Groups)
            groupsBox.AddChild(BuildGroup(cat.CategoryId, grp));

        return collapsible;
    }

    private Control BuildGroup(string catId, RmcRosterGroupDto grp)
    {
        var outer = new VBox();
        var title = new Label { Text = grp.GroupName };
        title.AddStyleClass("RosterGroupTitle");
        outer.AddChild(title);

        var list = new VBox { Separation = 1 };
        foreach (var item in grp.Items)
            list.AddChild(BuildItem(catId, grp.GroupId, item));

        outer.AddChild(list);
        return outer;
    }

    private Control BuildItem(string catId, string? groupId, RmcRosterItemDto item)
    {
        var row = new HBox { Separation = 6 };
        var name = new Label { Text = item.DisplayName };
        if (item.IsDead)
            name.AddStyleClass("RosterDeadItem");

        var warpBtn = new Button { Text = "Warp" };
        warpBtn.OnPressed += _ => OnWarpRequested?.Invoke(item.Uid);

        row.AddChild(name);
        row.AddChild(new Control { HorizontalExpand = true }); // spacer
        row.AddChild(warpBtn);
        row.Tooltip = $"{catId}/{groupId}";

        return row;
    }

    private void UpsertCategory(RmcRosterCategoryDto cat)
    {
        // ищем существующую категорию по Header.Text
        foreach (var child in _categoryList.Children)
        {
            if (child is CollapsiblePanel cp && cp.Header is Label hdr && hdr.Text.StartsWith(cat.CategoryName.Split(" (")[0]))
            {
                // перезагрузить тело
                cp.GetBody().DisposeAllChildren();
                var groupsBox = new VBox { Separation = 2 };
                cp.SetBody(groupsBox);
                foreach (var grp in cat.Groups)
                    groupsBox.AddChild(BuildGroup(cat.CategoryId, grp));
                hdr.Text = cat.CategoryName;
                hdr.Modulate = Color.FromHex(cat.HexColor);
                return;
            }
        }
        _categoryList.AddChild(BuildCategory(cat));
    }

    private void RemoveItem(string catId, string? groupId, EntityUid uid)
    {
        foreach (var catCtrl in _categoryList.Children)
        {
            if (catCtrl is not CollapsiblePanel cp) continue;
            var groupsBox = cp.GetBody() as VBox;
            if (groupsBox == null) continue;

            foreach (var grpCtrl in groupsBox.Children)
            {
                if (grpCtrl is not VBox grpBox) continue;
                foreach (var row in grpBox.Children.ToArray())
                {
                    if (row is not HBox) continue;
                    // У нас нет прямых меток UID в UI, можно хранить в Tag:
                    if (row.UserData is EntityUid rUid && rUid == uid)
                        row.Orphan();
                }
            }
        }
    }
}
