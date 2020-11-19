using Godot;
using System;

public class LocalGame : Game
{
    LocalController Player1 {get;} = new LocalController("Player1");
    LocalController Player2 {get;} = new LocalController("Player2");
    public override void InitializeGame()
    {
        base.InitializeGame();

        Player1.HasInitiative = true;
        Player1.Color = Colors.Blue;
        Player2.Color = Colors.Red;

        Unit u = Unit.SpawnWithUnitName(nameof(Zombie), new Vector2(400, 400));
        u.SetController(Player1);
        u.State.Change<BasicIdleOnTurnState>();
    }
}
