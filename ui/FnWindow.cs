using Godot;
using System;

public class FnWindow : Node2D
{
    private float width = 900;
    private float height = 600;
    public override void _Ready()
    {
        Center();
        Hide();
    }

    public void SetContent(Node node)
    {

    }

    private void Center()
    {
        float x = (Global.Config.WindowWidth - width) / 2;
        float y = (Global.Config.WindowHeight - height) / 2;

        Position = new Vector2(x, y);
    }

    private void _on_CloseButton_pressed()
    {
        Visible = false;
    }
}
