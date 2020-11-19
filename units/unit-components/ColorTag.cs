using Godot;
using System;

public class ColorTag : Node2D
{
    public static readonly SceneWrapper<ColorTag> Scene = new SceneWrapper<ColorTag>("res://units/unit-components/ColorTag.tscn");

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
