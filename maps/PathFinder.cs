using Godot;
using System.Linq;
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

            ConnectPoints(index, upIndex, fstar);
            ConnectPoints(index, downIndex, fstar);
            ConnectPoints(index, leftIndex, fstar);
            ConnectPoints(index, rightIndex, fstar);
        });

        Vector2[] results = fstar.GetPointPath(fromIndex, toIndex);
        return results;
    }

    private void ConnectPoints(int fromId, int toId, FStar fstar)
    {
        if(fstar.HasPoint(toId) && !fstar.ArePointsConnected(fromId,toId)) 
        {
            fstar.ConnectPoints(fromId,toId);
        }
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
        return Mathf.Abs(fromId - toId);
    }

    public override float _EstimateCost(int fromId, int toId)
    {
        return base._ComputeCost(fromId, toId);
    }
}

public class BFS
{
    private Dictionary<string,Vector2> cameFrom = new Dictionary<string, Vector2>();
    private Queue<GameBoardCell> frontier = new Queue<GameBoardCell>();
    private Vector2 origin;
    private GameBoard board;

    public BFS(Vector2 startPoint, GameBoard gameBoard)
    {
        origin = startPoint;
        board = gameBoard;
    }

    public void Search()
    {
        GameBoardCell startCell = board.CellAt(origin);
        frontier.Enqueue(startCell);
        cameFrom.Add(startCell.Position.ToString(), startCell.Position);

        int iters = 0;

        while(frontier.Count() > 0)
        {
            iters++;
            GameBoardCell current = frontier.Dequeue();
            current.Neighbors.ForEach(next => {

                if(!cameFrom.ContainsKey(next.Position.ToString()))
                {
                    frontier.Enqueue(next);
                    cameFrom.Add(next.Position.ToString(), current.Position);
                }
            });
        }
    }

    public List<Vector2> PathTo(Vector2 destination)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 goal = cameFrom.Values.Where(c => c == destination).FirstOrDefault();
        Vector2 current = goal;

        while(current != origin)
        {
            path.Add(current);
            current = cameFrom[current.ToString()];
        }
        
        return path;
    }
}
