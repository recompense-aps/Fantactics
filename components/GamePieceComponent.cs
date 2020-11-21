using Godot;
using System;

public class GamePieceComponent : Node
{
    public GameTile Tile 
    {
        get
        {
            return Global.ActiveMap.GetTileAt(Global.ActiveMap.GetBoardPositionFromWorldPosition(slave.Position));
        }
    }
    private Node2D slave;
    private Map map;

    public override void _Ready()
    {
        Node2D parent = GetParent() as Node2D;

        if(parent == null)
        {
            throw Global.Error("Parent must be a Node2D");
        }

        slave = parent;
        Global.Bus.On(nameof(Map.MapChanged), this, nameof(OnMapChanged));
    }

    public void SnapToGrid()
    {
        slave.Position = new Vector2(Tile.WorldPosition);
    }

    private void OnMapChanged(Map newMap)
    {
        map = newMap;
        SnapToGrid();
    }

}
