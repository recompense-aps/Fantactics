using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Map : Node2D
{
    public GameTile ActiveTile {get; private set;}
    TileMap environmentMap;
    TileMap highlightMap;
    Label debugLabel;
    Vector2 halfCell = new Vector2(32,32);
    public override void _Ready()
    {
        environmentMap = GetNode<TileMap>("EnvironmentMap");
        highlightMap = GetNode<TileMap>("HighlightMap");
        debugLabel = GetNode<Label>("Debug/DebugText");
        Global.ActiveMap = this;
    }

    public override void _Process(float delta)
    {
        Vector2 mouse = GetTree().Root.GetMousePosition();
        Vector2 tile = environmentMap.WorldToMap(mouse);
        Vector2 worldPosition = environmentMap.MapToWorld(tile);
        int cell = environmentMap.GetCell((int)tile.x, (int)tile.y);
        string name = environmentMap.TileSet.TileGetName(cell);

        ActiveTile = new GameTile(worldPosition + halfCell, tile, name);

        debugLabel.Text = string.Format(@"
            Mouse at:   {0}
            TileId:     {1}
            Tile:       {2}
        ", tile, cell, name);
    }

    public Vector2 GetWorldPositionFromCell(Vector2 cellPosition)
    {
        return environmentMap.MapToWorld(cellPosition) + halfCell;
    }

    public Vector2 GetBoardPositionFromWorldPosition(Vector2 worldPosition)
    {
        return environmentMap.WorldToMap(worldPosition);
    }

    public GameTile GetCellAt(Vector2 gameBoardPosition)
    {
        int cell = environmentMap.GetCell((int)gameBoardPosition.x, (int)gameBoardPosition.y);
        string name = environmentMap.TileSet.TileGetName(cell);
        return new GameTile(GetWorldPositionFromCell(gameBoardPosition), gameBoardPosition, name);
    }

    public bool CellHasUnit(Vector2 cell)
    {
        IEnumerable<Unit> units = GetTree().GetNodesInGroup("units").Cast<Unit>();

        IEnumerable<Unit> q = 
            from    unit in units
            where   unit.GameBoardPosition != null && 
                    unit.GameBoardPosition.IsSameTile(GetCellAt(cell))
            select  unit;

        Global.Log(q.Count());
        return q.Count() != 0;
    }

    public void HighlightTiles(Vector2 origin, int distance, CellHighlight highlight)
    {
        origin = environmentMap.WorldToMap(origin);
        for(int x = 0; x <= distance; x++)
        {
            for(int y = 0; y <= distance && x + y <= distance; y++)
            {
                highlightMap.SetCell((int)origin.x + x, (int)origin.y + y, (int)highlight);
                highlightMap.SetCell((int)origin.x - x, (int)origin.y + y, (int)highlight);
                highlightMap.SetCell((int)origin.x + x, (int)origin.y - y, (int)highlight);
                highlightMap.SetCell((int)origin.x - x, (int)origin.y - y, (int)highlight);
            }
        }
    }

    public void UnHighlight(CellHighlight highlight)
    {
        foreach(Vector2 v in highlightMap.GetUsedCellsById((int)highlight))
        {
            highlightMap.SetCell((int)v.x, (int)v.y, -1);
        }
    }

    private void HighlightHoveredCells(Vector2 tile)
    {
        foreach(Vector2 v in highlightMap.GetUsedCellsById((int)CellHighlight.Yellow))
        {
            highlightMap.SetCell((int)v.x, (int)v.y, -1);
        }

        highlightMap.SetCell((int)tile.x, (int)tile.y, (int)CellHighlight.Yellow);
    }

    private void _on_Map_tree_exiting()
    {
        Global.ActiveMap = null;
    }
}

public class GameTile
{
    public Vector2 WorldPosition {get; private set;}
    public Vector2 BoardPosition {get; private set;}
    public string Name {get; set;}
    public GameTile(Vector2 worldPosition, Vector2 boardPosition, string name)
    {
        WorldPosition = worldPosition;
        BoardPosition = boardPosition;
        Name = name;
    }

    public bool IsWithin(GameTile otherTile, int distance)
    {
        return BoardPosition.DistanceTo(otherTile.BoardPosition) <= distance;
    }

    public bool IsSameTile(GameTile otherTile)
    {
        return otherTile.BoardPosition == BoardPosition;
    }
}

public enum CellHighlight {Green, Red, Yellow}