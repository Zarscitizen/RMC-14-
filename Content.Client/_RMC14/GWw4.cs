foreach (var category in entityData.Categories)
{
    foreach (var item in category.Items)
    {
        panel.AddChild(CreateItemLabel(item));
    }
    RootContainer.AddChild(panel);
}
