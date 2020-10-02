using Godot;
using System;

public class BasicSelectedState : State
{
    public override void _Process(float delta)
    {
        if(Input.IsMouseButtonPressed((int)ButtonList.Left))
        {
            MoveUnit();
        }
    }

    private async void MoveUnit()
    {
        SlaveAs<Unit>().MoveTo(GetViewport().GetMousePosition());
        await ToSignal(Slave, nameof(Unit.FinishedMoving));
        Manager.Change<BasicIdleOnTurnState>();
    }
}
