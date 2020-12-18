using Godot;
using System;

[Spawnable]
public class ColorTag : Node2D
{
    ColorRect colorRect;
    public override void _Ready()
    {
        colorRect = GetNode<ColorRect>("ColorRect");
        Position = new Vector2(-40, 16);
    }

    public void SetColor(Color color)
    {
        colorRect.Color = color;
    }
}
