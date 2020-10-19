using Godot;
using System;

public static class Extensions
{
    public static int BoardDistance(this Vector2 vector, Vector2 otherVector)
    {
        return (int)Mathf.Abs(vector.x - otherVector.x) +
                (int)Mathf.Abs(vector.y - otherVector.y);
    }
}
