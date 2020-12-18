using Godot;
using System;

[Spawnable]
public class University : Building
{
    public static SceneWrapper<University> Scene = new SceneWrapper<University>("res://buildings/University.tscn");
}
