using Godot;
using System.Text.Json;

public class FtRequest
{
    public string Type {get; set;}
    public FtRequestData Data {get; set;}

    public FtRequest(FtRequestType type, FtRequestData data)
    {
        Type = type.ToString();
        Data = data;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize<object>(this, new JsonSerializerOptions()
        {
            WriteIndented = true,
            MaxDepth = 50
        });
    }
}

public enum FtRequestType{ CreateGame, SyncUnits }
