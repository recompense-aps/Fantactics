using Godot;
using System;

public class Global : Node
{
    public static Global Instance{get; private set;}
    public static Map ActiveMap{get; set;}
    public static void Log(object message)
    {
        GD.Print(message);
    }
    public static Exception Error(object message)
    {
        GD.PrintErr(message);
        return new Exception(message.ToString());
    }
    public override void _Ready()
    {
        Instance = this;
    }
}
