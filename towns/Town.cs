using Godot;
using System.Collections.Generic;

public class Town : Node2D
{
    protected List<string> AvaliableUnits = new List<string>(){ nameof(Zombie) };

    public override void _Ready()
    {
        InitializeTown();
    }

    public virtual void InitializeTown()
    {
        AddChild(new GamePieceComponent());
    }

    public virtual void OnClick()
    {

    }
}
