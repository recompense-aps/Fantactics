using Godot;
using System;

public class Global : Node
{
    public static Global Instance{get; private set;}
    public static void Log(object message)
    {
        GD.Print(message);
    }
    public static void Error(object message)
    {
        GD.PrintErr(message);
    }
    public override void _Ready()
    {
        Instance = this;
    }
}
