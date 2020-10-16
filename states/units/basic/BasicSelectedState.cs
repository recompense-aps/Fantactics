using Godot;
using System;

public class BasicSelectedState : State
{
    private Unit unit;
    public override void _Ready()
    {
        base._Ready();
        unit = SlaveAs<Unit>();
        Global.ActiveMap.HighlightTiles(unit.Position, unit.Speed, CellHighlight.Green);
    }
    public override void _Process(float delta)
    {
        if(Input.IsMouseButtonPressed((int)ButtonList.Left))
        {
            if(Global.ActiveMap.ActiveTile.IsSameTile(unit.GameBoardPosition))
            {
                return;
            }

            Global.ActiveMap.UnHighlight(CellHighlight.Green);
            
            if(Global.ActiveMap.ActiveTile.IsWithin(unit.GameBoardPosition, unit.Speed) && !Global.ActiveMap.CellHasUnit(Global.ActiveMap.ActiveTile.BoardPosition))
            {
                MoveUnit();
            }
            else
            {
                Manager.Change<BasicIdleOnTurnState>();
            }
        }
    }

    private async void MoveUnit()
    {
        unit.MoveTo(Global.ActiveMap.ActiveTile.WorldPosition);
        await ToSignal(Slave, nameof(Unit.FinishedMoving));
        Manager.Change<BasicIdleOnTurnState>();
    }
}
