foreach (var category in entityData.Categories)
{
    var panel = CreateCategoryPanel(category.Name, category.Count);
    foreach (var item in category.Items)
    {
        panel.AddChild(CreateItemLabel(item));
    }
    RootContainer.AddChild(panel);
}
