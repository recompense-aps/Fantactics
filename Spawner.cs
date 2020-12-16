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

    public static T Spawn<T>(string id) where T:Node
    {
        return scenes[id].Instance() as T;
    }
}

public class Spawnable: System.Attribute{}
