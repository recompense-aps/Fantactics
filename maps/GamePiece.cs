using Godot;
using System.Linq;
using System.Collections.Generic;

public class GamePiece : Area2D
{
    [Signal]
    public delegate void Clicked();
    [Signal]
    public delegate void ControllerChanged(Controller newController);

    public static List<GamePiece> All => Global.Instance.GetTree().GetNodesInGroup("game_pieces").Cast<GamePiece>().ToList();

    public GameBoard Board {get; private set;}
    public string Guid {get; set;}
    public GameBoardCell Cell => Board.CellAtWorldPosition(Position);
    public Controller Controller 
    {
        get
        {
            return controller;
        }
        set
        {
            controller = value;
            Guid = controller.Guid + "|" + GetInstanceId();
            EmitSignal(nameof(ControllerChanged), controller);
        }
    }

    private bool wasPressed = false;
    private Controller controller;

    public override void _Ready()
    {
        Connect("input_event", this, nameof(OnInputEvent));
        Guid = Guid ?? GetInstanceId().ToString();
        Board = Global.ActiveMap.Board;
        AddToGroup("game_pieces");
    }

    public bool HasSameController(GamePiece otherPiece)
    {
        return Controller == null ||                    // Just for prototype testing
               otherPiece.Controller == Controller;
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
