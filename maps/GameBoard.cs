using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameBoard
{
    public Vector2 Size {get;} = new Vector2(23,14);
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

                    if(position.x >= 0 && position.y >= 0)
                    {
                        if(tiles.Where(c => c.Position == position).FirstOrDefault() == null)
                        {
                            tiles.Add(CellAt(position));
                        }
                    }
                    
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
    public List<GameBoardCell> Neighbors => GetNeighbors();
    
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

    private List<GameBoardCell> GetNeighbors()
    {
        List<GameBoardCell> neighbors = new List<GameBoardCell>();
        int nextX,nextY;
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if( (i == 0 && j == 0) || ( Math.Abs(i) + Math.Abs(j) > 1 ) ) continue;
                nextX = (int)Position.x + i;
                nextY = (int)Position.y + j;
                if(nextX > 0 && nextY > 0 && nextX <= Map.Board.Size.x && nextY <= Map.Board.Size.y)
                {
                    neighbors.Add(Map.Board.CellAt(nextX,nextY));
                }
            }
        }

        return neighbors;
    }
}
