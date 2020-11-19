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
    public delegate void StartedSpawning();
    [Signal]
    public delegate void FinishedSpawning();
    [Signal]
    public delegate void StartedDying();
    [Signal]
    public delegate void FinishedDying();

    public int Attack{get; protected set;}          = 1;
    public int Defense{get; protected set;}         = 1;
    public int Hp{get; protected set;}              = 1;
    public int Speed{get; protected set;}           = 4;
    public int AttackRange{get; protected set;}     = 1;

    public string Guid{get; set;}
    public Controller UnitController{get; set;}
    public GameTile GameBoardPosition{get; private set;}
    public StateManager<Unit> State {get; private set;}
    private HpDisplay hpDisplay;
    private ColorTag colorTag;
    private Map map;
    private bool wasPressed = false;
    public List<UnitAction> Actions {get; private set;} = new List<UnitAction>();

    ///////////////////////////////////////////////////////////////////
    //   Static utility methods
    ////////////////////////////////////////////////////////////////////

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

    public static Unit Spawn(Unit unit, Vector2 worldPosition)
    {
        unit.GlobalPosition = worldPosition;
        Global.Instance.AddChild(unit);
        return unit;
    }

    public static Unit SpawnWithUnitName(string unitName, Vector2 worldPosition)
    {
        Unit unit = null;
        switch(unitName)
        {
            case nameof(Zombie):
                unit = Zombie.Scene.Instance();
                break;
        }

        if(unit != null)
        {
            return Spawn(unit, worldPosition);
        }

        return unit;
    }

    ///////////////////////////////////////////////////////////////////
    //   Overridden Godot callbacks
    ////////////////////////////////////////////////////////////////////
    public override void _Ready()
    {
        Guid = Guid ?? GetInstanceId().ToString();
        State = new StateManager<Unit>();
        hpDisplay = HpDisplay.Scene.Instance();
        colorTag = ColorTag.Scene.Instance();
        AddChild(State);
        AddChild(hpDisplay);
        AddChild(colorTag);
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

        SpawnAction();
        State.Change<BasicIdleState>();
    }

    ///////////////////////////////////////////////////////////////////
    //   Public non-virtual methods
    ////////////////////////////////////////////////////////////////////
    public void SetGamePosition(Vector2 position)
    {
        Position = Global.ActiveMap.GetWorldPositionFromCell(position);
        GameBoardPosition = Global.ActiveMap.GetTileAt(position);
    }

    public void SetController(Controller controller)
    {
        UnitController = controller;
        Guid = controller.Guid + "|" + GetInstanceId();
        colorTag.SetColor(controller.Color);
    }

    public bool HasSameController(Unit otherUnit)
    {
        return UnitController == null ||                    // Just for prototype testing
               otherUnit.UnitController == UnitController;
    }

    public void RecordAction(UnitAction action)
    {
        Actions.Add(action);
    }

    public void RecordAction(string jsonAction){}

    public SignalAwaiter ReplayAction(UnitAction action)
    {
        return action.Replay();
    }

    public void ReplayAllActions()
    {
        Actions.ForEach(async action => await action.Replay());
    }

    public void FlushActions()
    {
        Actions = new List<UnitAction>();
    }

    public void LoadActionsFromJson(List<string> actionsJson)
    {
        actionsJson.ForEach(RecordAction);
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
        GameBoardPosition = Global.ActiveMap.GetTileAt(Global.ActiveMap.GetBoardPositionFromWorldPosition(point));
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
    
    public async virtual void SpawnAction()
    {
        RecordAction(new SpawnAction(this));
        EmitSignal(nameof(StartedSpawning));
        Vector2 scale = new Vector2(Scale);
        Vector2 scaleTo = new Vector2(scale * 2);

        Tween t1 = new Tween();
        t1.InterpolateProperty(this, "scale", scale, scaleTo, 0.25f);
        AddChild(t1);
        t1.Start();
        
        await ToSignal(t1, "tween_completed");
        t1.QueueFree();

        Tween t2 = new Tween();
        AddChild(t2);
        t2.InterpolateProperty(this, "scale", scaleTo, scale, 0.25f);
        t2.Start();

        await ToSignal(t2, "tween_completed");
        t2.QueueFree();

        EmitSignal(nameof(FinishedSpawning));
    }

    public async virtual void DieAction()
    {
        RecordAction(new DieAction(this));
        EmitSignal(nameof(StartedDying));
        await Kill();
        EmitSignal(nameof(FinishedDying));
        //QueueFree();
        // TODO: Some sort of dead state for later clean up
    }
    
    ////////////////////////////////////////////////////////////////////
    //  Other public virtual methods
    ////////////////////////////////////////////////////////////////////
    public virtual bool CanMoveTo(Vector2 gameBoardPosition)
    {
        return gameBoardPosition.BoardDistance(GameBoardPosition.BoardPosition) <= Speed;
    }

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