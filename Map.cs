using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Map : Node2D
{
    public MapTilemaps Tilemaps {get; private set;}
    public MapTilesets Tilesets {get; private set;}
    public GameBoard Board {get; private set;}
    private Label debugLabel;
    private Vector2 halfCell = new Vector2(32,32);
    
    public override void _Ready()
    {
        Tilemaps = new MapTilemaps()
        {
            Environment = GetNode<TileMap>("EnvironmentMap"),
            Highlights = GetNode<TileMap>("HighlightMap")
        };
        Tilesets = new MapTilesets()
        {
            Environment = Tilemaps.Environment.TileSet,
            Highlights = Tilemaps.Highlights.TileSet
        };
        Board = new GameBoard(this);
        debugLabel = GetNode<Label>("Debug/DebugText");
        Global.ActiveMap = this; // TODO decouple this
    }

    public override void _Process(float delta)
    {
        ProcessActiveTile();
    }

    private void ProcessActiveTile()
    {
        Vector2 mouse = GetTree().Root.GetMousePosition();
        GameBoardCell active = Board.CellAtWorldPosition(mouse);

        if(active.TileIndex != -1)
        {
            debugLabel.Text = string.Format(@"
                Mouse at:   {0}
                TileId:     {1}
                Tile:       {2}
            ", active.Position, active.TileIndex, active.TileName);
        }

    }

    public CellHighlight GetHighlightAt(Vector2 gameBoardPosition)
    {
        return (CellHighlight)Tilemaps.Highlights.GetCell((int)gameBoardPosition.x, (int)gameBoardPosition.y);
    }

    public void HighlightTiles(Vector2 origin, int distance, CellHighlight highlight)
    {
        origin = Tilemaps.Environment.WorldToMap(origin);
        for(int x = 0; x <= distance; x++)
        {
            for(int y = 0; y <= distance && x + y <= distance; y++)
            {
                if(x == 0 && y == 0) continue;

                Tilemaps.Highlights.SetCell((int)origin.x + x, (int)origin.y + y, (int)highlight);
                Tilemaps.Highlights.SetCell((int)origin.x - x, (int)origin.y + y, (int)highlight);
                Tilemaps.Highlights.SetCell((int)origin.x + x, (int)origin.y - y, (int)highlight);
                Tilemaps.Highlights.SetCell((int)origin.x - x, (int)origin.y - y, (int)highlight);
            }
        }
    }

    public void HighlightTile(Vector2 gameBoardPosition, CellHighlight highlight)
    {
        Tilemaps.Highlights.SetCell((int)gameBoardPosition.x, (int)gameBoardPosition.y, (int)highlight);
    }

    public void UnHighlight(CellHighlight highlight)
    {
        foreach(Vector2 v in Tilemaps.Highlights.GetUsedCellsById((int)highlight))
        {
            Tilemaps.Highlights.SetCell((int)v.x, (int)v.y, -1);
        }
    }

    private void HighlightHoveredCells(Vector2 tile)
    {
        foreach(Vector2 v in Tilemaps.Highlights.GetUsedCellsById((int)CellHighlight.Yellow))
        {
            Tilemaps.Highlights.SetCell((int)v.x, (int)v.y, -1);
        }

        Tilemaps.Highlights.SetCell((int)tile.x, (int)tile.y, (int)CellHighlight.Yellow);
    }

    private void _on_Map_tree_exiting()
    {
        Global.ActiveMap = null;
    }
}

public class MapTilemaps
{
    public TileMap Environment {get; set;}
    public TileMap Highlights {get; set;}
}

public class MapTilesets
{
    public TileSet Environment {get; set;}
    public TileSet Highlights {get; set;}
}

public enum CellHighlight {Green, Red, Yellow}