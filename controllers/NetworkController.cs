using Godot;
using System;

public class NetworkController : Controller
{
    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("ui_accept"))
        {
            Vector2 pos = GetTree().Root.GetMousePosition();
            Zombie z = Zombie.Scene.Instance();
            z.Position = pos;
            Global.Instance.AddChild(z);
            z.SetController(this);
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
        //HttpResponse response = await Global.Http.Request("http://localhost:3000/game", 5000, data);
    }
}

