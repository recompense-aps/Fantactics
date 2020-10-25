using Godot;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FtRequestData
{
    public string SenderGuid {get; set;}

    public string SenderName {get; set;}

    public string Error {get; set;}

    public string Success {get; set;}

    public string Message {get; set;}

    public List<ClientNotification> Notifications {get; set;}

    public List<object> UnitActions {get; set;}

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
    }

}

public class ClientNotification
{
    public string Type {get; set;}
    public Dictionary<string,object> Data {get; set;}

    public ClientNotification(ClientNotificationType type)
    {
        Type = type.ToString();
    }
}

public enum ClientNotificationType { StartedTurn, EndedTurn }
