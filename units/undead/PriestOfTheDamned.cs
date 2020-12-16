using Godot;
using System;

[Spawnable]
public class PriestOfTheDamned : Unit
{
    public override void _Ready()
    {
        base._Ready();
        State.AllowSameChange = false;
        State.Change<BasicIdleOnTurnState>();
    }


}
