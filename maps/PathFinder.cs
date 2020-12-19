using Godot;
using System.Collections.Generic;

public class PathFinder : Node
{
    public override void _Ready()
    {
        
    }

    public Vector2[] FindPath(Vector2 from, Vector2 to, List<GameBoardCell> board, float limit)
    {
        FStar fstar = new FStar();

        // TODO: this probably isn't very efficent AT ALL
        GameBoardCell[,] map = new GameBoardCell[50,50];

        // add all the points to fstar and map
        board.ForEach(cell => {
            int index = board.IndexOf(cell);

            // TODO: compute weights
            fstar.AddPoint(index, cell.Position);
            map[(int)cell.Position.x,(int)cell.Position.y] = cell;
        });

        // get from and to
        int fromIndex = board.IndexOf(GetCellFromMap(map, (int)from.x, (int)from.y));
        int toIndex = board.IndexOf(GetCellFromMap(map, (int)to.x, (int)to.y));

        // make all the point connections
        board.ForEach(cell => {
            GameBoardCell up,down,left,right;
            int index,upIndex,downIndex,leftIndex,rightIndex,x,y;

            index = board.IndexOf(cell);
            x = (int)cell.Position.x;
            y = (int)cell.Position.y;
            
            up = GetCellFromMap(map, x, y - 1);
            down = GetCellFromMap(map, x, y + 1);
            left = GetCellFromMap(map, x - 1, y);
            right = GetCellFromMap(map, x + 1, y);

            upIndex = up == null ? -1 : board.IndexOf(up);
            downIndex = up == null ? -1 : board.IndexOf(down);
            leftIndex = up == null ? -1 : board.IndexOf(left);
            rightIndex = up == null ? -1 : board.IndexOf(right);

            if(up != null) fstar.ConnectPoints(index,upIndex,false);
            if(down != null) fstar.ConnectPoints(index,downIndex,false);
            if(left != null) fstar.ConnectPoints(index,leftIndex,false);
            if(right != null) fstar.ConnectPoints(index,rightIndex,false);
        });

        return fstar.GetPointPath(fromIndex, toIndex);
    }

    private GameBoardCell GetCellFromMap(GameBoardCell[,] map, int x, int y)
    {
        if(x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
        {
            return null;
        }
        return map[x,y];
    }
}

public class FStar : AStar2D
{
    public override float _ComputeCost(int fromId, int toId)
    {
        return base._ComputeCost(fromId, toId);
    }

    public override float _EstimateCost(int fromId, int toId)
    {
        return base._EstimateCost(fromId, toId);
    }
}
