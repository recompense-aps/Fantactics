using Godot;
using System;

public class GamePiece : Area2D
{
    public GameBoard Board {get; private set;}
    public GameBoardCell Cell {get; private set;}
    public override void _Ready()
    {
        
    }
}
