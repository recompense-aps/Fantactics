using Godot;
using System;

[Spawnable]
public class Abaddon : Unit
{
    public override void _Ready()
    {
        base._Ready();
        State.AllowSameChange = false;
        State.Change<BasicIdleOnTurnState>();
    }

    protected override void SetStats()
    {
        Speed = 5;
        Hp = 8;
    }
}
