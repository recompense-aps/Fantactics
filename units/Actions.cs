using Godot;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public abstract class UnitAction
{
    public string UnitGuid{get; set;}
    public Dictionary<string,object> JsonData {get; set;}

    [JsonIgnore]
    protected Unit ActionUnit { get { return Unit.FromGuid(UnitGuid); } }

    public UnitAction(){}

    public UnitAction(Unit unit)
    {
        UnitGuid = unit.Guid;
        JsonData = new Dictionary<string, object>();
        JsonData.Add("UnitActionType", GetType().Name);
    }
    public abstract SignalAwaiter Replay(bool remote = true);

    public static List<UnitAction> TransformFromJson(List<JsonElement> unitActions)
    {
        List<UnitAction> actions = new List<UnitAction>();
        
        foreach(JsonElement elem in unitActions)
        {
            JsonElement jsonData = elem.GetProperty(nameof(JsonData));
            string unitGuid = jsonData.GetProperty(nameof(UnitGuid)).GetString();
            string unitActionType = jsonData.GetProperty("UnitActionType").GetString(); 
            switch(unitActionType)
            {
                case nameof(MoveAction):
                    actions.Add(MoveAction.FromJson(unitGuid, jsonData));
                    break;
                case nameof(FightAction):
                    actions.Add(FightAction.FromJson(unitGuid, jsonData));
                    break;
                case nameof(SpawnAction):
                    actions.Add(SpawnAction.FromJson(unitGuid, jsonData));
                    break;
                case nameof(DieAction):
                    // Do nothing for now. If a unit dies, it was probably the
                    // result of some other action (fight, etc)
                    // therefore, we don't need to DieAction because it will happen
                    // automatically
                    break;
                default:
                    throw Global.Error(string.Format("{0} is not a valid UnitActionType", unitActionType));
            }
        }

        return null;
    }
}

public class MoveAction : UnitAction
{
    public Vector2 WorldDestination {get; private set;}

    public static MoveAction FromJson(string unitGuid, JsonElement jsonData)
    {
        Unit unit = Unit.FromGuid(unitGuid);
        JsonElement worldDestination = jsonData.GetProperty(nameof(WorldDestination));

        float x = worldDestination.GetProperty("x").GetSingle();
        float y = worldDestination.GetProperty("x").GetSingle();

        return new MoveAction(unit, new Vector2(x,y));
    }

    public MoveAction(Unit unit, Vector2 worldDestination) : base(unit)
    {
        WorldDestination = worldDestination;
        JsonData.Add(nameof(WorldDestination), new {x = WorldDestination.x, y = WorldDestination.y});
    }

    public override SignalAwaiter Replay(bool remote = true)
    {
        ActionUnit.MoveToAction(WorldDestination);

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedMoving));
    }
}

public class FightAction : UnitAction
{
    public string OtherUnitGuid {get; private set;}

    public static FightAction FromJson(string unitGuid, JsonElement jsonData)
    {
        JsonElement otherUnitGuid = jsonData.GetProperty(nameof(OtherUnitGuid));
        Unit unit = Unit.FromGuid(unitGuid);
        Unit otherUnit = Unit.FromGuid(otherUnitGuid.GetString());

        return new FightAction(unit, otherUnit);
    }

    public FightAction(Unit unit, Unit otherUnit) : base(unit)
    {
        OtherUnitGuid = otherUnit.Guid;
        JsonData.Add(nameof(OtherUnitGuid), OtherUnitGuid);
    }

    public override SignalAwaiter Replay(bool remote = true)
    {
        ActionUnit.FightAction(Unit.FromGuid(OtherUnitGuid));

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedFighting));
    }
}

public class SpawnAction : UnitAction
{
    public Vector2 SpawnWorldPosition{get; set;}
    public string UnitType {get; set;}

    public static SpawnAction FromJson(string unitGuid, JsonElement jsonData)
    {
        SpawnAction action = new SpawnAction();

        JsonElement spawnWorldPosition = jsonData.GetProperty(nameof(SpawnWorldPosition));
        JsonElement unitType = jsonData.GetProperty(nameof(SpawnWorldPosition));

        float x = spawnWorldPosition.GetProperty("x").GetSingle();
        float y = spawnWorldPosition.GetProperty("x").GetSingle();

        action.UnitGuid = unitGuid;
        action.SpawnWorldPosition = new Vector2(x,y);
        action.UnitType = unitType.GetString();

        return action;
    }

    public SpawnAction(Unit unit) : base(unit)
    {
        SpawnWorldPosition = new Vector2(unit.GlobalPosition);
        UnitType = unit.GetType().Name;
    }

    public SpawnAction(){}

    public override SignalAwaiter Replay(bool remote = true)
    {
        // little different, need to create the unit
        // when it gets created, spawn action happens
        Unit unit = Unit.SpawnWithUnitName(UnitType, SpawnWorldPosition);

        unit.Guid = UnitGuid;

        // set to remote controlled state
        if(remote)
        {
            unit.State.Change<RemoteControlledState>();
        }

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedSpawning));
    }
}

public class DieAction : UnitAction
{
    public DieAction(Unit unit) : base(unit){}
    public override SignalAwaiter Replay(bool remote = true)
    {
        ActionUnit.DieAction();

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedDying));
    }
}
