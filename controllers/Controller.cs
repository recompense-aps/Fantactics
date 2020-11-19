using Godot;
using System.Collections.Generic;
using System.Linq;

public class Controller : Node
{
    [Signal]
    public delegate void TurnEnded();
    public bool HasInitiative {get; set;} = false;
    public Color Color {get; set;}
    public string Guid {get; protected set;}
    public string ControllerName {get; set;}
    public RaceType Race {get; set;}
    public List<Unit> Units
    {
        get
        {
            return Unit.All
                       .Where(unit => unit.UnitController == this)
                       .ToList();
        }
    }
    public override void _Ready()
    {
        InitializeController();
    }

    public override void _Process(float delta)
    {
        if(HasInitiative && Units.All(unit => unit.State.Is<BasicFinishedTurnState>()))
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        Global.TurnStartScreen.Open(ControllerName);
        Units.ForEach(unit => unit.State.Change<BasicIdleOnTurnState>());
        HasInitiative = true;
    }

    public void EndTurn()
    {
        Units.ForEach(unit => unit.State.Change<BasicIdleState>());
        EmitSignal(nameof(TurnEnded));
        HasInitiative = false;
    }

    public virtual void InitializeController(string guid = "")
    {
        Guid = OS.GetUniqueId() + guid;
    }

    protected List<object> GetAllUnitActions()
    {
        List<object> actions = new List<object>();
        Units.ForEach(unit => {
            Global.Log(unit.Actions.Count);
            actions.AddRange(unit.Actions);
            unit.FlushActions();
        });
        Global.Log(actions.Count);
        return actions;
    }
}
