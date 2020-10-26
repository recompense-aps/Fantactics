using Godot;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

public class NetworkController : Controller
{
    public override void _Ready()
    {
        base._Ready();
    }
    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("ui_accept"))
        {
            SpawnUnit();
        }

        if(Input.IsActionJustPressed("ui_end"))
        {
            Sync();
        }
    }

    private async void Sync()
    {
        FtRequestData response = await Game.Service.Sync(GetAllUnitActions());
        Global.Log(response.ToJson());

        // transform unit actions
        List<UnitAction> actions = UnitAction.TransformFromJson( response.UnitActions.Cast<JsonElement>().ToList() );

        // replay the unit actions
        actions.ForEach(async action => await action.Replay());
    }

    private async void SpawnUnit()
    {
        Vector2 pos = GetTree().Root.GetMousePosition();

        // don't spawn if a unit is already there
        Vector2 gameBoardPosition = Global.ActiveMap.GetBoardPositionFromWorldPosition(pos);
        if(Global.ActiveMap.CellHasUnit(gameBoardPosition)) return;

        Zombie z = Zombie.Scene.Instance();
        z.SetController(this);

        Unit.Spawn(z, pos);

        await ToSignal(z, nameof(Unit.FinishedSpawning));

        if(HasInitiative)
        {
            z.State.Change<BasicIdleOnTurnState>();
        }
        else
        {
            z.State.Change<BasicIdleState>();
        }
    }
}

