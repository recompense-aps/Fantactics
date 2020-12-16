using Godot;
using System;

[Spawnable]
public class Pupil : Unit
{
    public static SceneWrapper<Pupil> Scene = new SceneWrapper<Pupil>("res://units/wizards/Pupil.tscn");
    public override void _Ready()
    {
        base._Ready();
        State.AllowSameChange = false;
    }

    public override void _Process(float delta)
    {

    }
}
