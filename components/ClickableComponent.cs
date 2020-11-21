using Godot;
using System;

public class ClickableComponent : Node
{
    [Signal]
    public delegate void Clicked();

    private bool wasPressed = false;
    private Area2D slave;

    public override void _Ready()
    {
        
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
                string signal = nameof(Clicked);
                EmitSignal(signal);

                // pump the signal to the slave
                if(slave.HasUserSignal(signal))
                {
                    slave.EmitSignal(signal);
                }
            }
        }
    }

}