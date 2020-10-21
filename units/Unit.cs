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
    public delegate void Died();

    public int Attack{get; protected set;}          = 1;
    public int Defense{get; protected set;}         = 1;
    public int Hp{get; protected set;}              = 1;
    public int Speed{get; protected set;}           = 2;
    public int AttackRange{get; protected set;}     = 1;

    public Controller UnitController{get; set;}
    public GameTile GameBoardPosition{get; private set;}
    public StateManager<Unit> State {get; private set;}
    private HpDisplay hpDisplay;
    public static List<Unit> All
    {
        get
        {
            return Global.Instance.GetTree().GetNodesInGroup("units").Cast<Unit>().ToList();
        }
    }

    private bool wasPressed = false;

    public override void _Ready()
    {
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

    public bool HasSameController(Unit otherUnit)
    {
        return UnitController == null ||                    // Just for prototype testing
               otherUnit.UnitController == UnitController;
    }

    public bool CanMoveTo(Vector2 gameBoardPosition)
    {
        return gameBoardPosition.BoardDistance(GameBoardPosition.BoardPosition) <= Speed;
    }

    public virtual void HoverEffect(bool toggle)
    {
        float alpha = toggle ? 0.5f : 1;
        Modulate = new Color(1, 1, 1, alpha);
    }

    public virtual void MoveTo(Vector2 point)
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
        t2.Connect("tween_completed", this, nameof(OnMoveTweenFinished));
    }

    public async virtual void Fight(Unit otherUnit)
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
            Die();
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

    public async virtual void Die()
    {
        await Kill();
        EmitSignal(nameof(Died));
        QueueFree();
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

    private void OnMoveTweenFinished(Godot.Object @object, NodePath key)
    {
        EmitSignal(nameof(FinishedMoving));
    }
}
