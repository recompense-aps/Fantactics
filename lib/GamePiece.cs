using Godot;
using System;

public class GamePiece : Area2D
{
    public GameBoard Board {get; private set;}
    public GameBoardCell Cell 
    {
        get
        {
            return Board.CellAtWorldPosition(Position);
        }
    }
    public override void _Ready()
    {
        
    }
}
