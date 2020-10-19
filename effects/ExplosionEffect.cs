using Godot;
using System;

public class ExplosionEffect : Node2D
{
    public override void _Ready()
    {
        
    }

    private void _on_AnimatedSprite_animation_finished()
    {
        QueueFree();
    }
}
