using Godot;

public class Game : Node2D
{
    public StateManager<Game> State;
    public static GameService Service {get; private set;} = new GameService();
    public override void _Ready()
    {
        InitializeGame();
    }

    public virtual void InitializeGame()
    {
        State = new StateManager<Game>();
        State.Change<GameSetupState>();
    }
}
