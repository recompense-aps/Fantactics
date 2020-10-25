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
            SendData();
        }
    }

    private async void SendData()
    {
        FtRequest request = new FtRequest(FtRequestType.SyncUnits, new FtRequestData()
        {
            SenderGuid = Guid,
            UnitActions = GetAllUnitActions()
        });
        string data = request.ToJson();
        Global.Log(data);
        HttpResponse response = await Global.Http.Request("http://192.168.1.21:3000/game/sync-units", 5000, data);
        UnitAction.FromJson(response.Body);
    }

    private async void SpawnUnit()
    {
        Vector2 pos = GetTree().Root.GetMousePosition();
        Zombie z = Zombie.Scene.Instance();
        z.Position = pos;
        z.SetController(this);
        Global.Instance.AddChild(z);

        await ToSignal(z, nameof(Unit.FinishedSpawning));

        z.State.Change<BasicIdleOnTurnState>();
    }
}

