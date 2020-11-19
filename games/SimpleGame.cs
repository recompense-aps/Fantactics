using Godot;

public class SimpleGame : Game
{
    public Controller LocalController {get; protected set;}
    public override void InitializeGame()
    {
        base.InitializeGame();

        LocalController = Global.LocalController;
        Global.LocalController = null;
        AddChild(LocalController);
    }
}
