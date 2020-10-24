using Godot;
using System;

public class Zombie : Unit
{
    public static SceneWrapper<Zombie> Scene = new SceneWrapper<Zombie>("res://units/undead/Zombie.tscn");
    public override void _Ready()
    {
        base._Ready();
        State.AllowSameChange = false;
    }

    public override void _Process(float delta)
    {

    }
}
