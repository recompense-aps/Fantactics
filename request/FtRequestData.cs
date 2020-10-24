using Godot;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class FtRequestData
{
    public string SenderGuid {get; set;}

    public List<UnitAction> UnitActions {get; set;}

}
