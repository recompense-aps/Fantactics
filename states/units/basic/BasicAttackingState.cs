using Godot;
using System.Linq;

public class BasicAttackingState : State<Unit>
{
    private GameTile tileToAttack;

    protected override void OnStateStarted()
    {
        base.OnStateStarted();
        
        // state that we need
        tileToAttack = Manager.Data<GameTile>("tileToAttack");

        GameTile tileToMoveTo = Global.ActiveMap.GetTilesAt(tileToAttack.BoardPosition, 1)
                        .Where(v => Slave.CanMoveTo(v.BoardPosition) && Global.ActiveMap.GetUnitAt(v.BoardPosition) == null)
                        .First();
        Attack(tileToMoveTo);
    }

    private async void Attack(GameTile attackPositionTile)
    {
        // first, move into position
        if(tileToAttack.BoardPosition.BoardDistance(Slave.GameBoardPosition.BoardPosition) > Slave.AttackRange)
        {
            Slave.MoveToAction(attackPositionTile.WorldPosition);

            // wait for movement to finish
            await ToSignal(Slave, nameof(Unit.FinishedMoving));
        }

        // then, do the combat
        Unit otherUnit = Global.ActiveMap.GetUnitAt(tileToAttack.BoardPosition);
        Slave.FightAction(otherUnit);

        // wait for the combat to finish
        await ToSignal(Slave, nameof(Unit.FinishedFighting));
        Global.Log("Finished fighting");

        // done now, revert all units state
        Unit.All
            .Where(unit => unit.GetInstanceId() != Slave.GetInstanceId())
            .ToList()
            .ForEach(unit => unit.State.Revert());

        // revert our attacking unit
        Manager.Change<BasicIdleOnTurnState>();
    }
}
