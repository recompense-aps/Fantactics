using Godot;
using System;

public class HpDisplay : Node2D
{
    public static readonly SceneWrapper<HpDisplay> Scene = new SceneWrapper<HpDisplay>("res://units/unit-components/HpDisplay.tscn");
    
    private Label label;
    public override void _Ready()
    {
        Position = new Vector2(16,16);
        label = GetNode<Label>("ColorRect/Label");
    }

    public void SetValue(int value)
    {
        label.Text = value.ToString();
    }
}
