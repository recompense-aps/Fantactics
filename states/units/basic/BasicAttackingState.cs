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
                        .Where(v => Slave.CanMoveTo(v.BoardPosition))
                        .First();

        Slave.MoveTo(tileToMoveTo.WorldPosition);
        PackedScene effect = GD.Load<PackedScene>("res://effects/ExplosionEffect.tscn");
        Node2D effectNode = effect.Instance() as Node2D;
        Global.Instance.AddChild(effectNode);
        effectNode.GlobalPosition = Global.ActiveMap.ActiveTile.WorldPosition;
        Manager.Change<BasicAttackingState>();
    }
}
