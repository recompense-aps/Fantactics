using Godot;
using System.Collections.Generic;

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

        List<SpawnGroup> player1Units = new List<SpawnGroup>()
        {
            new SpawnGroup()
            {
                UnitType = nameof(Zombie),
                BoardPosition = new Vector2(2,2)
            },
            new SpawnGroup()
            {
                UnitType = nameof(Zombie),
                BoardPosition = new Vector2(4,4)
            }
        };

        List<SpawnGroup> player2Units = new List<SpawnGroup>()
        {
            new SpawnGroup()
            {
                UnitType = nameof(Pupil),
                BoardPosition = new Vector2(22,14)
            }
        };

        player1Units.ForEach(group => group.Spawn(Player1));
        player2Units.ForEach(group => group.Spawn(Player2));

        Building pit = Building.SpawnAt(nameof(Pit), new Vector2(1,1));
        pit.Controller = Player1;
        Building uni = Building.SpawnAt(nameof(University), new Vector2(23, 15));
        uni.Controller = Player2;

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
class SpawnGroup
{
    public string UnitType {get; set;}
    public Vector2 BoardPosition{get; set;}

    public void Spawn(Controller c)
    {
        Unit u = Unit.SpawnAt(UnitType, BoardPosition);
        u.SetController(c);
    }

}
