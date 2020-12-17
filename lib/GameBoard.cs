using Godot;
using System;

public class GameBoard
{
	private Map map;

	public GameBoard(Map currentMap)
	{
		map = currentMap;
	}

	public GameBoardCell CellAt(int x, int y)
	{
		string error;

		if(x < 0 || y < 0)
		{
			error = string.Format("Invalid cell position [{0},{1}]. Arguments must be >= 0");
			throw Global.Error(error);
		}

		return new GameBoardCell(x, y, map);
	}

	public GameBoardCell CellAt(Vector2 position)
	{
		return CellAt((int)position.x, (int)position.y);
	}
}

public class GameBoardCell
{
    public Map Map{get; private set;}
    public Vector2 Position {get; private set;}
    public Vector2 WorldPosition {get; private set;}
	public string TileName {get; private set;}
	public int TileIndex {get; private set;}
    
	public bool HasUnit
    {
        get
        {
            return Map.CellHasUnit(Position);
        }
    }
    public Unit Unit
    {
        get
        {
            if(HasUnit)
            {
                return Map.GetUnitAt(Position);
            }
            else
            {
                return null;
            }
        }
    }
    
    public GameBoardCell(int x, int y, Map currentMap)
    {
        Position = new Vector2(x,y);
		Map = currentMap;
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
		// TileIndex = Map.TileMaps.Environment.GetCell((int)Position.x, (int)Position.y);
        // TileName = Map.TileSets.Environment.TileGetName(TileIndex);
	}
}
