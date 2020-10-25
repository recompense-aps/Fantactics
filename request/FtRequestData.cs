using Godot;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class FtRequestData
{
    public string SenderGuid {get; set;}

    public string SenderName {get; set;}

    public string Error {get; set;}

    public string Success {get; set;}

    public string Message {get; set;}

    public List<object> UnitActions {get; set;}

}
