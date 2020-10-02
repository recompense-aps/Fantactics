using Godot;
using System;

/**
*   A state for units when their controller has the turn and the unit
*   has not moved yet this turn.
*   
*   Slave Mutations:
*       MUTATES:    Modulate
*       UN-MUTATES: Modulate 
*/
public class BasicIdleOnTurnState : State
{
    protected override void OnStateStarted()
    {
        base.OnStateStarted();
        Slave.Connect("mouse_entered", this, nameof(OnMouseEntered));
        Slave.Connect("mouse_exited", this, nameof(OnMouseExited));
        Slave.Connect(nameof(Unit.Clicked), this, nameof(OnClicked));
    }

    protected override void OnStateFinished()
    {
        base.OnStateFinished();
        SlaveAs<Unit>().HoverEffect(false);
    }

    private void OnMouseEntered()
    {
        SlaveAs<Unit>().HoverEffect(true);
    }

    private void OnMouseExited()
    {
        SlaveAs<Unit>().HoverEffect(false);
    }

    private async void OnClicked()
    {
        Timer t = new Timer();
        t.Autostart = true;
        t.WaitTime = 1;
        AddChild(t);
        await ToSignal(t, "timeout");
        t.QueueFree();
        Manager.Change<BasicSelectedState>();
    }
}
