using Godot;
using System;

public class GamePieceComponent : Node
{
    public GameTile Tile {get; private set;}
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
        // get closest board position
        Vector2 v = map.GetBoardPositionFromWorldPosition(slave.Position);

        // set the world position to corresponding board position
        slave.Position = map.GetWorldPositionFromCell(v);

        // set the tile at that position
        Tile = map.GetCellAt(v);
    }

    private void OnMapChanged(Map newMap)
    {
        map = newMap;
        SnapToGrid();
    }

}
