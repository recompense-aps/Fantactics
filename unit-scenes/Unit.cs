using Godot;
using System;

public class Unit : Area2D
{
    [Signal]
    public delegate void TurnStarted();
    [Signal]
    public delegate void Clicked();
    [Signal]
    public delegate void StartedMoving();
    [Signal]
    public delegate void FinishedMoving();

    public int Attack{get; protected set;}  = 1;
    public int Defense{get; protected set;} = 1;
    public int Hp{get; protected set;}      = 1;
    public int Speed{get; protected set;}   = 1;
    
    protected StateManager State {get; private set;}

    public override void _Ready()
    {
        State = new StateManager();
        AddChild(State);
        Connect("input_event", this, nameof(OnInputEvent));
    }

    public virtual void HoverEffect(bool toggle)
    {
        float alpha = toggle ? 0.5f : 1;
        Modulate = new Color(1, 1, 1, alpha);
    }

    public virtual void MoveTo(Vector2 point)
    {
        EmitSignal(nameof(StartedMoving));
        Tween t = new Tween();
        Tween t2 = new Tween();
        AddChild(t);
        AddChild(t2);
        t.InterpolateProperty(this, "position", Position, new Vector2(point.x, Position.y), 2);
        t2.InterpolateProperty(this, "position", new Vector2(point.x, Position.y), new Vector2(point.x, point.y), 2, Tween.TransitionType.Linear, Tween.EaseType.InOut, 2.5f);
        t.Start();
        t2.Start();

        t2.Connect("tween_completed", this, nameof(OnMoveTweenFinished));
        //Position = new Vector2(point);
    }

    protected virtual void SetStats(){}

    protected virtual void SetAbilities(){}

    private void OnInputEvent(Node viewport, InputEvent inputEvent, int shape)
    {
        if(inputEvent is InputEventMouseButton)
        {
            InputEventMouseButton mouseEvent = inputEvent as InputEventMouseButton;
            if(mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
            {
                EmitSignal(nameof(Clicked));
            }
        }
    }

    private void OnMoveTweenFinished(Godot.Object @object, NodePath key)
    {
        EmitSignal(nameof(FinishedMoving));
    }
}
