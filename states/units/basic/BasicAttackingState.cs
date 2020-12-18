using Godot;
using System.Linq;

public class BasicAttackingState : State<Unit>
{
    private GameBoardCell tileToAttack;

    protected override void OnStateStarted()
    {
        base.OnStateStarted();
        
        // state that we need
        tileToAttack = Manager.Data<GameBoardCell>("tileToAttack");

        GameBoardCell tileToMoveTo = Global.ActiveMap.Board.GetCellsAround(tileToAttack, 1)
                        .Where(v => Slave.CanMoveTo(v) && v.HasUnit == false)
                        .First();
        Attack(tileToMoveTo);
    }

    private async void Attack(GameBoardCell attackPositionTile)
    {
        // first, move into position
        if(tileToAttack.Position.BoardDistance(Slave.Cell.Position) > Slave.AttackRange)
        {
            Slave.MoveToAction(attackPositionTile.WorldPosition);

            // wait for movement to finish
            await ToSignal(Slave, nameof(Unit.FinishedMoving));
        }

        // then, do the combat
        Unit otherUnit = tileToAttack.Unit;
        Slave.FightAction(otherUnit);

        // wait for the combat to finish
        await ToSignal(Slave, nameof(Unit.FinishedFighting));
        Global.Log("Finished fighting");

        // unit is done with turn
        Manager.Change<BasicFinishedTurnState>();
    }
}
