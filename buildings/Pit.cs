using Godot;
using System;

[Spawnable]
public class Pit : Building
{
    public static SceneWrapper<Pit> Scene = new SceneWrapper<Pit>("res://buildings/Pit.tscn");
}
