using Godot;
using System;
using System.Collections.Generic;

public class StateManager<T> : Node where T:Node
{
    public State<T> Current {get; private set;}
    public bool AllowSameChange{get; set;} = true;
    private Dictionary<string,object> data = new Dictionary<string, object>();
    private Stack<Type> history = new Stack<Type>();

    private T slave;

    public override void _Ready()
    {
        Name = GetType().Name;
        slave = GetParent() as T;
    }

    public void Change<U>() where U:State<T>, new()
    {
        if(!AllowSameChange && Current is T)
        {
            Global.Error("Unable to change to same state");
            return;
        }

        State<T> state = new U();

        ChangeTo(state);
    }

    public bool Is<U>() where U:State<T>
    {
        return Current is U;
    }

    public void Revert()
    {
        if(history.Count > 0)
        {
            State<T> last = Activator.CreateInstance(history.Pop()) as State<T>;

            if(last == null)
            {
                throw Global.Error("Unable to revert state!!!");
            }

            ChangeTo(last);
        }
    }

    private async void ChangeTo(State<T> state)
    {
        if(Current == null)
        {
            AddNewState(state);
        }
        else
        {
            history.Push(Current.GetType());
            Current.QueueFree();

            await ToSignal(Current, "tree_exited");

            AddNewState(state);
        }
    }

    private void AddNewState(State<T> state)
    {
        Current = state;
        Current.Master(slave, this);
        AddChild(Current);
    }

    public void Mutate(string prop, object value)
    {
        if(data.ContainsKey(prop))
        {
            data[prop] = value;
        }
        else
        {
            data.Add(prop, value);
        }
    }

    public U Data<U>(string prop)
    {
        if(!data.ContainsKey(prop) || data[prop] == null)
        {
            throw Global.Error(string.Format("Cannot access property '{0}' property does not exist", prop));
        }
        
        object value = data[prop];

        if(value is U)
        {
            return (U)value;
        }
        else
        {
            throw Global.Error(string.Format("Unable to cast property '{0}' to type '{1}", prop, nameof(U)));
        }
    }
}
