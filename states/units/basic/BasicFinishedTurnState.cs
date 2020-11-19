using Godot;
using System;

public class BasicFinishedTurnState : State<Unit>
{
    protected override void OnStateStarted()
    {
        base.OnStateStarted();
        Slave.Modulate = new Color(Slave.Modulate.r, Slave.Modulate.g, Slave.Modulate.b, 0.6f);
    }

    protected override void OnStateFinished()
    {
        base.OnStateFinished();
        Slave.Modulate = new Color(Slave.Modulate.r, Slave.Modulate.g, Slave.Modulate.b, 1.0f);
    }
}
