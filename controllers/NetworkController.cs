using Godot;
using System.Collections.Generic;

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
    }

    private async void SpawnUnit()
    {
        Vector2 pos = GetTree().Root.GetMousePosition();
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

