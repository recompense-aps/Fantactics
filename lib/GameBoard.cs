using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameBoard
{
	private Map map;

	public GameBoard(Map currentMap)
	{
		map = currentMap;
	}

	public GameBoardCell CellAt(int x, int y)
	{
		if(x < 0 || y < 0)
		{
			return null;
		}

		return new GameBoardCell(x, y, map);
	}

	public GameBoardCell CellAt(Vector2 position)
	{
		return CellAt((int)position.x, (int)position.y);
	}

    public GameBoardCell CellAtWorldPosition(Vector2 position)
    {
        return CellAt(map.Tilemaps.Environment.WorldToMap(position - map.Tilemaps.Environment.Position));
    }

    public List<GameBoardCell> GetCellsAround(GameBoardCell cell, int distance)
    {
        List<GameBoardCell> tiles = new List<GameBoardCell>();
        List<Vector2> tranforms = new List<Vector2>(){ new Vector2(1,1), new Vector2(-1,1), new Vector2(1,-1), new Vector2(-1,-1) };
        for(int x = 0; x <= distance; x++)
        {
            for(int y = 0; y <= distance && x + y <= distance; y++)
            {
                if(x == 0 && y == 0) continue;
                tranforms.ForEach(v => 
                {
                    Vector2 position = cell.Position + ( new Vector2(x,y) * v );
                    tiles.Add(CellAt(position));
                });
            }
        }


        return tiles;
    }

    public IEnumerable<Unit> GetUnitsInArea(Vector2 gameBoardPosition, int distance)
    {
        return 
            from    unit in Unit.All
            where   unit.Cell.Position.BoardDistance(gameBoardPosition) <= distance &&
                    unit.Cell.Position != gameBoardPosition
            select  unit;
    }
}

public class GameBoardCell : Godot.Object
{
    private const int INFINITE_COST = 1000;
    public Map Map{get; private set;}
    public Vector2 Position {get; private set;}
    public Vector2 WorldPosition {get; private set;}
	public string TileName {get; private set;}
	public int TileIndex {get; private set;}
    private Vector2 halfCell = new Vector2(32,32);
	public bool HasUnit => Unit != null;
    public bool IsEmpty => GamePiece != null;
    public Unit Unit => Unit.All.Where(unit => unit.Cell.Position == Position).FirstOrDefault();
    public int MovementCost => IsEmpty ? 1 : INFINITE_COST; // TODO: refactor for terrain
    public GamePiece GamePiece => GamePiece.All.Where(piece => piece.Cell.Position == Position).FirstOrDefault();
    
    public GameBoardCell(int x, int y, Map currentMap)
    {
        Map = currentMap;
        Position = new Vector2(x,y);
        WorldPosition = Map.Tilemaps.Environment.MapToWorld(Position) + halfCell + Map.Tilemaps.Environment.Position;
		ExtractMetaData();
    }

    public CellHighlight Highlight
    {
        get
        {
            return Map.GetHighlightAt(Position);
        }
    }

    public bool IsWithin(GameBoardCell otherTile, int distance)
    {
        return Position.DistanceTo(otherTile.Position) <= distance;
    }

    public bool IsSameTile(GameBoardCell otherTile)
    {
        return otherTile.Position == Position;
    }

	private void ExtractMetaData()
	{
		TileIndex = Map.Tilemaps.Environment.GetCell((int)Position.x, (int)Position.y);
        if(TileIndex != -1)
        {
            TileName = Map.Tilesets.Environment.TileGetName(TileIndex);
        }
	}
}
