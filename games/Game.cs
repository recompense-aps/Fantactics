using Godot;

public class Game : Node2D
{
    public Controller LocalController {get; protected set;}
    public StateManager<Game> State;
    public static GameService Service {get; private set;} = new GameService();
    public override void _Ready()
    {
        InitializeGame();
    }

    public virtual void InitializeGame()
    {
        LocalController = Global.LocalController;
        Global.LocalController = null;
        AddChild(LocalController);

        State = new StateManager<Game>();
        State.Change<GameSetupState>();
    }
}
