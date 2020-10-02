using Godot;
using System;

public class StateManager : Node
{
    public State Current {get; private set;}
    public bool AllowSameChange{get; set;} = true;
    private Node slave;

    public override void _Ready()
    {
        Name = GetType().Name;
        slave = GetParent();
    }

    public void Change<T>() where T:State, new()
    {
        if(!AllowSameChange && Current is T)
        {
            // don't allow changing to the current state
            return;
        }

        State state = new T();

        if(Current == null)
        {
            ChangeTo(state);
        }
        else
        {
            Current.Connect("tree_exited", this, nameof(ChangeTo), new Godot.Collections.Array(){ state });
            Current.QueueFree();
        }
    }

    private void ChangeTo(State state)
    {
        Current = state;
        Current.Master(slave, this);
        AddChild(Current);
    }
}
