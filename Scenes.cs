using Godot;
using System.Collections.Generic;

public static class Scenes
{
    private static Dictionary<string,PackedScene> scenes = new Dictionary<string, PackedScene>()
    {
        { "Explosion", GD.Load<PackedScene>("res://effects/ExplosionEffect.tscn") }
    };

    public static T Instance<T>(string id) where T:Node
    {
        return scenes[id].Instance() as T;
    }
}
