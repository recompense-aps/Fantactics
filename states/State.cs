using Godot;
using System;

public abstract class State : Node
{
    public Node Slave {get; private set;}
    protected StateManager Manager{get; private set;}

    public override void _Ready()
    {
        Name = GetType().Name;

        if(Slave == null)
        {
            Global.Error(string.Format("{0} cannot be added to Scene Tree before mastering. call '{1}' before adding to scene tree", GetType().Name, nameof(Master)));
        }
        Connect("tree_exiting", this, nameof(OnTreeExiting));
        OnStateStarted();
    }

    public void Master(Node slave, StateManager manager)
    {
        Slave = slave;
        Manager = manager;
    }

    protected T SlaveAs<T>() where T:Node
    {
        return Slave as T;
    }
    protected virtual void OnStateFinished(){ Global.Log(string.Format("{0} finished '{1}' state", Slave.Name, GetType().Name)); }
    protected virtual void OnStateStarted(){ Global.Log(string.Format("{0} started '{1}' state", Slave.Name, GetType().Name)); }

    private void OnTreeExiting(){ OnStateFinished(); }

}
