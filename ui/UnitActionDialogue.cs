using Godot;
using System;

public class UnitActionDialogue : Node2D
{
    [Signal]
    public delegate void SelectedCancel(GameBoardCell actionedTile);
    [Signal]
    public delegate void SelectedWait(GameBoardCell actionedTile);
    [Signal]
    public delegate void SelectedAttack(GameBoardCell actionedTile);
    public bool IsOpen{get; private set;}
    private GameBoardCell actionedTile;
    private Button waitButton;
    private Button attackButton;
    private Button cancelButton;
    public override void _Ready()
    {
        waitButton = FindNode("WaitButton") as Button;
        attackButton = FindNode("AttackButton") as Button;
        cancelButton = FindNode("CancelButton") as Button;
    }

    public void Open(Unit unit)
    {
        IsOpen = Visible = true;

        if(actionedTile != null)
        {
            waitButton.Visible = ShouldShowWaitButton(unit);
            attackButton.Visible = ShouldShowAttackButton(unit);
        }
    }

    public void Close()
    {
        IsOpen = Visible = false;
    }

    public void OpenAtMouse(Unit unit)
    {
        Position = GetTree().Root.GetMousePosition();
        actionedTile = Global.ActiveMap.Board.CellAtWorldPosition(Position);
        Open(unit);
    }

    private bool ShouldShowWaitButton(Unit unit)
    {
        return unit.CanMoveTo(actionedTile) && !actionedTile.HasUnit;
    }

    private bool ShouldShowAttackButton(Unit unit)
    {
        return actionedTile.HasUnit && 
               actionedTile.IsWithin(unit.Cell, unit.Speed + unit.AttackRange) &&
               actionedTile.Unit.HasSameController(unit) == false;
    }

    private void _on_WaitButton_pressed()
    {
        EmitSignal(nameof(SelectedWait), actionedTile);
        Close();
    }

    private void _on_AttackButton_pressed()
    {
        EmitSignal(nameof(SelectedAttack), actionedTile);
        Close();
    }

    private void _on_CancelButton_pressed()
    {
        EmitSignal(nameof(SelectedCancel), actionedTile);
        Close();
    }
}
