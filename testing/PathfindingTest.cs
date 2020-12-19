using Godot;
using System.Linq;
using System.Collections.Generic;

public class PathfindingTest : Node2D
{
    RichTextLabel output;
    public override void _Ready()
    {
        output = GetNode<RichTextLabel>("Output");
        //Test1();
        Test2();
    }

    private void Test1()
    {
        Map map = Global.ActiveMap;
        GameBoard board = map.Board;
        List<GameBoardCell> cells = board.GetCellsAround(board.CellAt(3,3), 10);
        cells.Add(board.CellAt(3,3));

        Global.Log("--MAP--");
        //cells.ForEach(c => Global.Log(c.Position));

        Vector2[] path = map.PathFinder.FindPath(
            new Vector2(3,3),
            new Vector2(5,7),
            cells,
            20
        );

        Global.Log("--PATH--");
        List<Vector2> list = path.ToList();

        list.ForEach(v => map.HighlightCell(v, CellHighlight.Yellow));
    }

    private void Test2()
    {
        Timer t = new Timer();
        t.WaitTime = 1;
        AddChild(t);
        t.Start();
        t.Connect("timeout", this, nameof(PathToMouse));
    }

    private void PathToMouse()
    {
        Map map = Global.ActiveMap;
        GameBoard board = map.Board;
        map.UnHighlight(CellHighlight.Green);
        map.UnHighlight(CellHighlight.Red);

        BFS search = new BFS(new Vector2(5,5), board);
        search.Search();

        var path = search.PathTo(board.CellAtWorldPosition(GetTree().Root.GetMousePosition()).Position);

        map.HighlightCell(new Vector2(5,5), CellHighlight.Red);
        path.ForEach(c => map.HighlightCell(c, CellHighlight.Green));
    }
}
