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

		SetUpUnits();
		SetUpBuildings();

		Global.Log("checking", Unit.All.Count);

		Player1.StartTurn();

		//Vector2[] path = Global.ActiveMap.PathFinder.FindPath(new Vector2(0,4), new Vector2(4,4), Global.ActiveMap.Board.GetCellsAround())
	}

	private void SetUpUnits()
	{
		Spawner.WithBoardPositions();
		Spawner.Spawn("Zombie")
			   .At(2, 2)
			   .In(Global.Instance)
			   .Then<Unit>(u => u.Controller = Player1)
			   .Spawn("Zombie")
			   .At(4, 4)
			   .In(Global.Instance)
			   .Then<Unit>(u => u.Controller = Player1)
			   .Spawn("Pupil")
			   .At(21, 2)
			   .In(Global.Instance)
			   .Then<Unit>(u => u.Controller = Player2);
	}

	private void SetUpBuildings()
	{
		Spawner.WithBoardPositions();
		Spawner.Spawn("Pit")
			   .At(1, 1)
			   .In(Global.Instance)
			   .Then<Building>(b => b.Controller = Player1)
			   .Spawn("University")
			   .At(21, 1)
			   .In(Global.Instance)
			   .Then<Building>(b => b.Controller = Player2);

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
