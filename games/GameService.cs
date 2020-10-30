using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;


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
        Global.Log(response.Body);
        FtRequestData data = JsonSerializer.Deserialize<FtRequestData>(response.Body);

        return data;
    }

    public async Task<FtRequestData> JoinGame()
    {
        FtRequest request = new FtRequest(FtRequestType.JoinGame, new FtRequestData()
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

        string json = request.ToJson();
        Global.Log(json);

        HttpResponse response = await Global.Http.Request(Route("sync"), 5000, json);
        Global.Log(response.Body);
        FtRequestData data = JsonSerializer.Deserialize<FtRequestData>(response.Body);

        return data;
    }

    private string Route(string endpoint)
    {
        return baseServiceRoute + endpoint;
    }
}
