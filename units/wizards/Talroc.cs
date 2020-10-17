using Godot;
using System;

public class Talroc : Unit
{
    public override void _Ready()
    {
        base._Ready();
        State.AllowSameChange = false;
        State.Change<BasicIdleOnTurnState>();
    }
}
