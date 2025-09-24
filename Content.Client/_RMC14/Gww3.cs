ToggleButton.OnPressed += () =>
{
    MarinesContent.Visible = !MarinesContent.Visible;
    ToggleButton.Text = MarinesContent.Visible ? "▼" : "►";
};
