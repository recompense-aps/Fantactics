using Godot;
using System;

public class BasicIdleOnTurnState : State<Unit>
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

    private void OnClicked()
    {
        Manager.Change<BasicSelectedState>();
    }
}
