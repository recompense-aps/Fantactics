using Godot;
using Fantactics;
using System;
using System.Linq;
using System.Collections.Generic;

public static class Spawner
{
    private static Dictionary<string, PackedScene> scenes = new Dictionary<string, PackedScene>();
    public static void Initialize()
    {
        Global.Log("Spawner", "Initializing...");

        List<string> types =
            (from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(Spawnable), true)
            where attributes != null && attributes.Length > 0
            select t.Name).ToList();

        types.ForEach(type => Global.Log("Spawner", string.Format("Discovered spawnable type '{0}'.", type)));

        List<string> allFiles = new FileHelper().IgnoreGit()
                                                .IgnoreImports()
                                                .IgnoreMono()
                                                .Ignore("assets")
                                                .OpenDirectory("res://")
                                                .WithExtension("tscn")
                                                .WithFileNameFilter(types)
                                                .Files;
        allFiles.ForEach(file => {
            string key = file.Split(".").First().Split("/").Last();
            scenes.Add(key, GD.Load<PackedScene>(file));
            Global.Log("Spawner", string.Format("Added scene '{0}' with id '{1}'", file, key));
        });

        Global.Log("Spawner", "Initialization complete");
    }

    public static void WithBoard(object board)
    {
    }

    public static T Spawn<T>(string id) where T:Node
    {
        if(!scenes.ContainsKey(id))
        {
            throw new ArgumentException($"Cannot spawn with id '{id}'. Scene with id does not exist");
        }

        T result = scenes[id].Instance() as T;
        
        if(result == null)
        {
            throw new ArgumentException($"Could not convert to type '{nameof(T)}'");
        }

        return result;
    }

    public static SpawnResult Spawn(string id)
    {
        return new SpawnResult(Spawn<Node>(id));
    }
}

public class Spawnable: System.Attribute{}

public class SpawnResult
{
    public Node Node {get; private set;}

    public SpawnResult(Node node)
    {
        Node = node;
    }

    public SpawnResult In(Node parent)
    {
        parent.CallDeferred("add_child", Node);
        return this;
    }

    public SpawnResult At(Vector2 position)
    {
        if(Node is Node2D)
        {
            (Node as Node2D).Position = new Vector2(position);
        }
        else
        {
            throw new ArgumentException("Cannot set nodes position. It is not a Node2D");
        }

        return this;
    }

    public SpawnResult At(float x, float y)
    {
        return At(new Vector2(x, y));
    }

    public SpawnResult Then<T>(Action<T> action) where T:Node
    {
        action(Node as T);
        return this;
    }

    public T As<T>() where T:Node
    {
        if(Node is T)
        {
            return Node as T;
        }
        else
        {
            throw new ArgumentException($"Cannot convert Node to type '{nameof(T)}'");
        }
    }

    public SpawnResult Spawn(string id)
    {
        return Spawner.Spawn(id);
    }
}
