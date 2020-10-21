using Godot;
using System;

public class SceneWrapper<T> where T:Node
{
    private PackedScene scene;
    public SceneWrapper(string path)
    {
        scene = GD.Load<PackedScene>(path);
    }

    public T Instance()
    {
        return scene.Instance() as T;
    }
}
