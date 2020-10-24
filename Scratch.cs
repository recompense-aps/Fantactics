using Godot;
using System.Text.Json;

public class Scratch : Node2D
{
    public override void _Ready()
    {
        MoveAction action = new MoveAction(new Unit(), new Vector2(50,50));
        Global.Log(JsonSerializer.Serialize(action));
    }
}
