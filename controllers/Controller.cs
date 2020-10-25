using Godot;
using System.Collections.Generic;
using System.Linq;

public class Controller : Node
{
    public string Guid {get; protected set;}
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
        Guid = OS.GetUniqueId();
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
