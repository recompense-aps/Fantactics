using Godot;
using System;

public class Zombie : Unit
{
    public override void _Ready()
    {
        base._Ready();
        State.AllowSameChange = false;
        State.Change<BasicIdleOnTurnState>();
    }

    public override void _Process(float delta)
    {

    }
}
