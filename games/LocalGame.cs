using Godot;
using System;

public class LocalGame : Game
{
    LocalController Player1 {get;} = new LocalController("Player1");
    LocalController Player2 {get;} = new LocalController("Player2");
    public override void InitializeGame()
    {
        base.InitializeGame();

        Player1.Color = Colors.Blue;
        Player2.Color = Colors.Red;
        Player1.ControllerName = "Player 1";
        Player2.ControllerName = "Player 2";
        Player1.Connect(nameof(Controller.TurnEnded), this, nameof(OnPlayer1FinishedTurn));
        Player2.Connect(nameof(Controller.TurnEnded), this, nameof(OnPlayer2FinishedTurn));

        Global.Instance.AddChild(Player1);
        Global.Instance.AddChild(Player2);

        Unit u = Unit.SpawnWithUnitName(nameof(Zombie), new Vector2(400, 400));
        u.SetController(Player1);

        Unit u1 = Unit.SpawnWithUnitName(nameof(Zombie), new Vector2(200, 200));
        u1.SetController(Player2);

        Player1.StartTurn();
    }

    private async void OnPlayer1FinishedTurn()
    {
        await Global.WaitFor(2);
        Player2.StartTurn();
    }

    private async void OnPlayer2FinishedTurn()
    {
        await Global.WaitFor(2);
        Player1.StartTurn();
    }
}
