using Godot;
using System;

[Spawnable]
public class HpDisplay : Node2D
{    
    public override void _Ready()
    {
        Position = new Vector2(16,16);
    }

    public void SetValue(int value)
    {
        GetNode<Label>("ColorRect/Label").Text = value.ToString();
    }
}
