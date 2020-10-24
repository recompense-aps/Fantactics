using Godot;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public abstract class UnitAction
{
    public string UnitGuid{get; private set;}
    public Dictionary<string,object> JsonData {get; set;}
    [JsonIgnore]
    protected Unit ActionUnit { get { return Unit.FromGuid(UnitGuid); } }
    public UnitAction(Unit unit)
    {
        UnitGuid = unit.Guid;
        JsonData = new Dictionary<string, object>();
        JsonData.Add("UnitActionType", GetType().Name);
    }
    public abstract SignalAwaiter Replay();

    public static List<UnitAction> FromJson(string json)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string,object>>(json);
        
        if(dict.ContainsKey("unitActions"))
        {
            // List<object> unitActions = (List<object>)dict["unitActions"];
            // Global.Log(unitActions.Count);
            Global.Log(dict["unitActions"].GetType().Name);
            JsonElement e = (JsonElement)dict["unitActions"];
            
        }
        else
        {
            Global.Log("Not unit actions in json");
        }

        return null;
    }
}

public class MoveAction : UnitAction
{
    public Vector2 WorldDestination {get; private set;}
    public MoveAction(Unit unit, Vector2 worldDestination) : base(unit)
    {
        WorldDestination = worldDestination;
        JsonData.Add(nameof(WorldDestination), new {x = WorldDestination.x, y = WorldDestination.y});
    }

    public override SignalAwaiter Replay()
    {
        ActionUnit.MoveToAction(WorldDestination);

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedMoving));
    }

    // Recieves JsonData
    public static MoveAction FromJson(Unit unit, JsonElement jsonData)
    {
        
        return null;
    }
}

public class FightAction : UnitAction
{
    public string OtherUnitGuid {get; private set;}

    public FightAction(Unit unit, Unit otherUnit) : base(unit)
    {
        OtherUnitGuid = otherUnit.Guid;
        JsonData.Add(nameof(OtherUnitGuid), OtherUnitGuid);
    }

    public override SignalAwaiter Replay()
    {
        ActionUnit.FightAction(Unit.FromGuid(OtherUnitGuid));

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedFighting));
    }
}

public class SpawnAction : UnitAction
{
    public Vector2 SpawnWorldPosition{get; set;}
    public SpawnAction(Unit unit) : base(unit){}

    public override SignalAwaiter Replay()
    {
        ActionUnit.SpawnAction();

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedSpawning));
    }
}

public class DieAction : UnitAction
{
    public DieAction(Unit unit) : base(unit){}
    public override SignalAwaiter Replay()
    {
        ActionUnit.DieAction();

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedDying));
    }
}
