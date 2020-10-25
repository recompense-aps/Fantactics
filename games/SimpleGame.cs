using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

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

public class SimpleGame : Game
{

}

public class GameService
{
    private const string baseServiceRoute = "http://192.168.1.21:3000/game/";
    public async Task<FtRequestData> CreateGame()
    {
        FtRequest request = new FtRequest(FtRequestType.CreateGame, new FtRequestData()
        {
            SenderGuid = OS.GetUniqueId(),
            SenderName = System.Environment.MachineName
        });

        HttpResponse response = await Global.Http.Request(Route("create-game"), 5000, request.ToJson());
        FtRequestData data = JsonSerializer.Deserialize<FtRequestData>(response.Body);

        return data;
    }

    public async Task<FtRequestData> JoinGame()
    {
        FtRequest request = new FtRequest(FtRequestType.CreateGame, new FtRequestData()
        {
            SenderGuid = OS.GetUniqueId(),
            SenderName = System.Environment.MachineName
        });

        HttpResponse response = await Global.Http.Request(Route("join-game"), 5000, request.ToJson());
        FtRequestData data = JsonSerializer.Deserialize<FtRequestData>(response.Body);

        return data;
    }

    public async Task<FtRequestData> Sync(List<object> unitActions)
    {
        FtRequest request = new FtRequest(FtRequestType.Sync, new FtRequestData()
        {
            SenderGuid = OS.GetUniqueId(),
            SenderName = System.Environment.MachineName,
            UnitActions = unitActions
        });

        HttpResponse response = await Global.Http.Request(Route("sync"), 5000, request.ToJson());
        Global.Log(response.Body);
        FtRequestData data = JsonSerializer.Deserialize<FtRequestData>(response.Body);

        return data;
    }

    private string Route(string endpoint)
    {
        return baseServiceRoute + endpoint;
    }
}
