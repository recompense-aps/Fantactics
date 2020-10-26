using Godot;
using System.Text.Json;

public class Scratch : Node2D
{
    public override void _Ready()
    {
        Test_FtRequestData_Json();
    }

    private async void Test_FtRequestData_Json()
    {
        FtRequest request = new FtRequest(FtRequestType.Sync, new FtRequestData()
        {
            SenderGuid = OS.GetUniqueId(),
            SenderName = System.Environment.MachineName,
            UnitActions = new System.Collections.Generic.List<object>()
        });
        string json = "{\"SenderGuid\":\"{c1a50698-37af-11ea-9528-806e6f6e6963}\",\"UnitActions\":[],\"Error\":\"Cannot handle game request. 'Sync' is not a valid request type\"}";
                HttpResponse response = await Global.Http.Request("http://192.168.1.21:3000/game/sync", 5000, request.ToJson());
        Global.Log(response.Body);
        FtRequestData data = JsonSerializer.Deserialize<FtRequestData>(response.Body);
    }
}
