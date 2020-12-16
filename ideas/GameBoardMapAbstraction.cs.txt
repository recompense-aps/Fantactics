public class GameBoard
{
	public PathFinder Path {get;} = null;
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
		return CellAt(position.x, position.y);
	}
}

public class GameCell : Godot.Object
{
	public Vector2 Position {get; private set;}
    public Vector2 WorldPosition {get; private set;}
	public string TileName {get; private set;}
	public int TileIndex {get; private set;}
    
	public bool HasUnit
    {
        get
        {
            return Global.ActiveMap.CellHasUnit(BoardPosition);
        }
    }
    public Unit Unit
    {
        get
        {
            if(HasUnit)
            {
                return Global.ActiveMap.GetUnitAt(BoardPosition);
            }
            else
            {
                return null;
            }
        }
    }
    
    public GameTile(int x, int y, Map currentMap)
    {
        Position = new Vector2(x,y);
		map = currentMap;
		ExtractMetaData();
    }

    public CellHighlight Highlight
    {
        get
        {
            return Global.ActiveMap.GetHighlightAt(BoardPosition);
        }
    }

    public bool IsWithin(GameTile otherTile, int distance)
    {
        return BoardPosition.DistanceTo(otherTile.BoardPosition) <= distance;
    }

    public bool IsSameTile(GameTile otherTile)
    {
        return otherTile.BoardPosition == BoardPosition;
    }

	private void ExtractMetaData()
	{
		TileIndex = map.TileMaps.Environment.GetCell((int)gameBoardPosition.x, (int)gameBoardPosition.y);
        Name = map.TileSets.Environment.TileGetName(TileIndex);
	}
    
}