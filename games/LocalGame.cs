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

		Player1.StartTurn();
	}

	private void SetUpUnits()
	{
		Spawner.WithBoard(new object());
		Spawner.Spawn("Zombie")
			   .At(2, 2)
			   .In(Global.Instance)
			   .Then<Unit>(u => u.SetController(Player1))
			   .Spawn("Zombie")
			   .At(4, 4)
			   .In(Global.Instance)
			   .Then<Unit>(u => u.SetController(Player1))
			   .Spawn("Pupil")
			   .At(22, 14)
			   .In(Global.Instance)
			   .Then<Unit>(u => u.SetController(Player2));
	}

	private void SetUpBuildings()
	{
		Spawner.WithBoard(new object());
		Spawner.Spawn("Pit")
			   .At(1, 1)
			   .In(Global.Instance)
			   .Then<Building>(b => b.Controller = Player1)
			   .Spawn("University")
			   .At(23, 15)
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
