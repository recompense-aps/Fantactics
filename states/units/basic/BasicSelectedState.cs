using Godot;
using System.Linq;
using System;

public class BasicSelectedState : State<Unit>
{
    protected override void OnStateStarted()
    {
        base.OnStateStarted();

        Global.ActiveMap.HighlightTiles(Slave.Position, Slave.Speed, CellHighlight.Green);
        Global.ActiveMap.Board.GetUnitsInArea(Slave.Cell.Position, Slave.Speed + Slave.AttackRange)
              .ToList()
              .ForEach(unit => Global.ActiveMap.HighlightTile(unit.Cell.Position, CellHighlight.Red));
        Global.UnitActionDialogue.Connect(nameof(UnitActionDialogue.SelectedCancel), this, nameof(OnSelectedCancel));
        Global.UnitActionDialogue.Connect(nameof(UnitActionDialogue.SelectedWait), this, nameof(OnSelectedWait));
        Global.UnitActionDialogue.Connect(nameof(UnitActionDialogue.SelectedAttack), this, nameof(OnSelectedAttack));
    }

    protected override void OnStateFinished()
    {
        base.OnStateFinished();
    }

    public override void _Process(float delta)
    {
        if(Input.IsMouseButtonPressed((int)ButtonList.Left) && !Global.UnitActionDialogue.IsOpen)
        {
            Global.UnitActionDialogue.OpenAtMouse(Slave);
        }
    }

    private void UnHighlightCells()
    {
        Global.ActiveMap.UnHighlight(CellHighlight.Green);
        Global.ActiveMap.UnHighlight(CellHighlight.Red);
    }

    private async void MoveUnit(Vector2 position, Action afterMove = null)
    {
        Slave.MoveToAction(position);

        await ToSignal(Slave, nameof(Unit.FinishedMoving));

        // moved, turn is over
        Manager.Change<BasicFinishedTurnState>();
    }

    private void Attack(GameBoardCell actionedTile)
    {
        // Manager.Mutate("tileToAttack", actionedTile);
        // Manager.Change<BasicAttackingState>();
    }

    private void OnSelectedCancel(GameBoardCell actionedTile)
    {
        UnHighlightCells();
        Unit.All.ForEach(unit => unit.State.Revert());
    }
    private void OnSelectedWait(GameBoardCell actionedTile)
    {
        // move here
        UnHighlightCells();
        MoveUnit(actionedTile.WorldPosition);
    }
    private void OnSelectedAttack(GameBoardCell actionedTile)
    {
        // move and attack
        // UnHighlightCells();
        // Attack(actionedTile);
    }
}
