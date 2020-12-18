using Godot;
using System;

[Spawnable]
public class Zombie : Unit
{
    public override void _Ready()
    {
        base._Ready();
        State.AllowSameChange = false;
    }

    public override void _Process(float delta)
    {

    }
}
