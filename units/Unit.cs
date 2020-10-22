using Godot;
using System.Linq;
using System.Collections.Generic;

public class Unit : Area2D
{
    [Signal]
    public delegate void TurnStarted();
    [Signal]
    public delegate void Clicked();
    [Signal]
    public delegate void StartedMoving();
    [Signal]
    public delegate void FinishedMoving();
    [Signal]
    public delegate void StartedFighting();
    [Signal]
    public delegate void FinishedFighting();
    [Signal]
    public delegate void StartedDying();
    [Signal]
    public delegate void FinishedDying();

    public int Attack{get; protected set;}          = 1;
    public int Defense{get; protected set;}         = 1;
    public int Hp{get; protected set;}              = 1;
    public int Speed{get; protected set;}           = 2;
    public int AttackRange{get; protected set;}     = 1;

    public string Guid{get; private set;}
    public Controller UnitController{get; set;}
    public GameTile GameBoardPosition{get; private set;}
    public StateManager<Unit> State {get; private set;}
    private HpDisplay hpDisplay;
    private List<UnitAction> actions = new List<UnitAction>();
    public static List<Unit> All
    {
        get
        {
            return Global.Instance.GetTree().GetNodesInGroup("units").Cast<Unit>().ToList();
        }
    }

    public static Unit FromGuid(string guid)
    {
        return All.Where(unit => unit.Guid == guid).FirstOrDefault();
    }

    private bool wasPressed = false;

    public override void _Ready()
    {
        Guid = GetInstanceId().ToString();
        State = new StateManager<Unit>();
        hpDisplay = HpDisplay.Scene.Instance();
        AddChild(State);
        AddChild(hpDisplay);
        Connect("input_event", this, nameof(OnInputEvent));
        SetStats();
        SetAbilities();

        if(GameBoardPosition == null)
        {
            Vector2 v = Global.ActiveMap.GetBoardPositionFromWorldPosition(Position);
            SetGamePosition(v);
        }

        hpDisplay.SetValue(Hp);
        AddToGroup("units");
    }

    public void SetGamePosition(Vector2 position)
    {
        Position = Global.ActiveMap.GetWorldPositionFromCell(position);
        GameBoardPosition = Global.ActiveMap.GetCellAt(position);
    }

    public void SetController(Controller controller)
    {
        Guid = controller.Guid + "|" + GetInstanceId();
    }

    public bool HasSameController(Unit otherUnit)
    {
        return UnitController == null ||                    // Just for prototype testing
               otherUnit.UnitController == UnitController;
    }

    public bool CanMoveTo(Vector2 gameBoardPosition)
    {
        return gameBoardPosition.BoardDistance(GameBoardPosition.BoardPosition) <= Speed;
    }

    public void RecordAction(UnitAction action)
    {
        actions.Add(action);
        Global.Log(ExportActionsAsJson());
    }

    public void RecordAction(string jsonAction){}

    public SignalAwaiter ReplayAction(UnitAction action)
    {
        return action.Replay();
    }

    public void ReplayAllActions()
    {
        actions.ForEach(async action => await action.Replay());
    }

    public void FlushActions()
    {
        actions = new List<UnitAction>();
    }

    public void LoadActionsFromJson(List<string> actionsJson)
    {
        actionsJson.ForEach(RecordAction);
    }

    public string ExportActionsAsJson()
    {
        List<string> jsonActions = new List<string>();
        actions.ForEach(action => jsonActions.Add(action.ToJson()));
        return "[" + string.Join(",", jsonActions) + "]";
    }

    ///////////////////////////////////////////////////////////////////
    //   Actions that the unit can do that are repeatable and recorded
    ////////////////////////////////////////////////////////////////////
    public async virtual void MoveToAction(Vector2 point)
    {
        EmitSignal(nameof(StartedMoving));
        Tween t = new Tween();
        Tween t2 = new Tween();
        AddChild(t);
        AddChild(t2);
        t.InterpolateProperty(this, "position", Position, new Vector2(point.x, Position.y), 0.25f);
        t2.InterpolateProperty(this, "position", new Vector2(point.x, Position.y), new Vector2(point.x, point.y), 0.25f, Tween.TransitionType.Linear, Tween.EaseType.InOut, 0.26f);
        t.Start();
        t2.Start();
        GameBoardPosition = Global.ActiveMap.GetCellAt(Global.ActiveMap.GetBoardPositionFromWorldPosition(point));
        await ToSignal(t2, "tween_completed");
        EmitSignal(nameof(FinishedMoving));
        RecordAction(new MoveAction(this, point));
    }

    public async virtual void FightAction(Unit otherUnit)
    {
        EmitSignal(nameof(StartedFighting));

        // 1 second delay
        Timer t = new Timer();
        t.WaitTime = 1;
        t.Autostart = true;
        AddChild(t);
        await ToSignal(t, "timeout");

        // attack
        await ApplyCombatEffect(otherUnit);
        ApplyAttack(otherUnit);

        // wait a bit beteen
        t.Start();
        await ToSignal(t, "timeout");

        // defense
        await otherUnit.ApplyCombatEffect(this);
        otherUnit.ApplyDefense(this);

        // clean up after a fight
        otherUnit.ResolvePostCombat();
        ResolvePostCombat();

        // fighting done
        EmitSignal(nameof(FinishedFighting));
        t.QueueFree();
        RecordAction(new FightAction(this, otherUnit));
    }
    
    public async virtual void DieAction()
    {
        RecordAction(new DieAction(this));
        EmitSignal(nameof(StartedDying));
        await Kill();
        EmitSignal(nameof(FinishedDying));
        QueueFree();
    }
    
    ////////////////////////////////////////////////////////////////////
    //  Other public virtual methods
    ////////////////////////////////////////////////////////////////////
    public virtual SignalAwaiter ApplyCombatEffect(Unit otherUnit)
    {
        // default filler thing
        ExplosionEffect explosion = Scenes.Instance<ExplosionEffect>("Explosion");
        Global.Instance.AddChild(explosion);
        explosion.GlobalPosition = otherUnit.GlobalPosition;

        return ToSignal(explosion, "tree_exited");
    }

    public virtual void ApplyAttack(Unit otherUnit)
    {
        otherUnit.Hp -= Attack;
    }

    public virtual void ApplyDefense(Unit otherUnit)
    {
        otherUnit.Hp -= Defense;
    }

    public virtual void ResolvePostCombat()
    {
        hpDisplay.SetValue(Hp);
        if(Hp <= 0)
        {
            DieAction();
        }
    }

    public virtual SignalAwaiter Kill()
    {
        Tween t = new Tween();
        t.InterpolateProperty(this, "rotation", Rotation, Mathf.Pi/2, 1);
        AddChild(t);
        t.Start();
        return ToSignal(t, "tween_completed");
    }

    public virtual void HoverEffect(bool toggle)
    {
        float alpha = toggle ? 0.5f : 1;
        Modulate = new Color(1, 1, 1, alpha);
    }

    protected virtual void SetStats(){}

    protected virtual void SetAbilities(){}

    private void OnInputEvent(Node viewport, InputEvent inputEvent, int shape)
    {
        if(inputEvent is InputEventMouseButton)
        {
            InputEventMouseButton mouseEvent = inputEvent as InputEventMouseButton;
            if(mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
            {
                wasPressed = true;
            }
            if(!mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
            {
                wasPressed = false;
                EmitSignal(nameof(Clicked));
            }
        }
    }
}

public abstract class UnitAction : Godot.Object
{
    private string unitGuid;
    protected Unit ActionUnit { get { return Unit.FromGuid(unitGuid); } }
    protected Dictionary<string,object> serializationData = new Dictionary<string, object>();
    public UnitAction(Unit unit)
    {
        unitGuid = unit.Guid;
        serializationData.Add("unitGuid", unitGuid);
        serializationData.Add("type", GetType().Name);
    }
    public abstract SignalAwaiter Replay();

    public virtual string ToJson()
    {
        return JSON.Print(serializationData);
    }
}

public class MoveAction : UnitAction
{
    public Vector2 WorldDestination {get; private set;}
    public MoveAction(Unit unit, Vector2 worldDestination) : base(unit)
    {
        WorldDestination = worldDestination;
    }

    public override SignalAwaiter Replay()
    {
        ActionUnit.MoveToAction(WorldDestination);

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedMoving));
    }
}

public class FightAction : UnitAction
{
    public string OtherUnitGuid {get; private set;}

    public FightAction(Unit unit, Unit otherUnit) : base(unit)
    {
        OtherUnitGuid = otherUnit.Guid;
    }

    public override SignalAwaiter Replay()
    {
        ActionUnit.FightAction(Unit.FromGuid(OtherUnitGuid));

        return ActionUnit.ToSignal(ActionUnit, nameof(Unit.FinishedFighting));
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