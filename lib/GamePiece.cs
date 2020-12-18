using Godot;
using System;

public class GamePiece : Area2D
{
    [Signal]
    public delegate void Clicked();

    public GameBoard Board {get; private set;}
    public GameBoardCell Cell => Board.CellAtWorldPosition(Position);

    private bool wasPressed = false;

    public override void _Ready()
    {
        Connect("input_event", this, nameof(OnInputEvent));
    }

    private void OnInputEvent(Node viewport, InputEvent inputEvent, int shape)
    {
        if(inputEvent is InputEventMouseButton)
        {
            InputEventMouseButton mouseEvent = inputEvent as InputEventMouseButton;
            if(mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
            {
                wasPressed = true;
            }
            if(!mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
            {
                wasPressed = false;
                EmitSignal(nameof(Clicked));
            }
        }
    }
}
