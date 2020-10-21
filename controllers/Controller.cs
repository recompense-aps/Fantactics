using Godot;
using System.Text;
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

    protected string ExportUnitActionsJson()
    {
        StringBuilder builder = new StringBuilder();
        Units.ForEach(unit => {
            builder.Append(unit.ExportActionsAsJson());
            unit.FlushActions();
        });
        return builder.ToString();
    }

    protected async virtual void SendData()
    {
        HttpResponse data = await Global.Http.Request("http://google.com");
    }
}
