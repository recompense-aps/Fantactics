using Godot;
using System.Linq;
using System;

public class BasicSelectedState : State<Unit>
{
    public override void _Ready()
    {
        base._Ready();
        GetTree().GetNodesInGroup("units")
                .Cast<Unit>()
                .Where(u => u.GetInstanceId() != Slave.GetInstanceId())
                .ToList()
                .ForEach(u => u.State.Change<BasicIdleState>());

        Global.ActiveMap.HighlightTiles(Slave.Position, Slave.Speed, CellHighlight.Green);
        Global.ActiveMap.GetUnitsInArea(Slave.GameBoardPosition.BoardPosition, Slave.Speed + Slave.AttackRange)
              .ToList()
              .ForEach(unit => Global.ActiveMap.HighlightTile(unit.GameBoardPosition.BoardPosition, CellHighlight.Red));
    }
    public override void _Process(float delta)
    {
        if(Input.IsMouseButtonPressed((int)ButtonList.Left))
        {
            if(Global.ActiveMap.ActiveTile.IsSameTile(Slave.GameBoardPosition))
            {
                return;
            }

            Global.ActiveMap.UnHighlight(CellHighlight.Green);
            Global.ActiveMap.UnHighlight(CellHighlight.Red);
            
            if(Global.ActiveMap.ActiveTile.IsWithin(Slave.GameBoardPosition, Slave.Speed) && !Global.ActiveMap.CellHasUnit(Global.ActiveMap.ActiveTile.BoardPosition))
            {
                MoveUnit(Global.ActiveMap.ActiveTile.WorldPosition, () => Manager.Change<BasicIdleOnTurnState>());
            }
            else if(Global.ActiveMap.CellHasUnit(Global.ActiveMap.ActiveTile.BoardPosition) && Global.ActiveMap.ActiveTile.IsWithin(Slave.GameBoardPosition, Slave.Speed + Slave.AttackRange))
            {
                Manager.Mutate("cellToAttack", Global.ActiveMap.ActiveTile);
                Manager.Change<BasicAttackingState>();
            }
            else
            {
                Manager.Change<BasicIdleOnTurnState>();
            }
        }
    }

    private void MoveUnit(Vector2 position, Action afterMove = null)
    {
        Slave.MoveTo(position);
    }
}
