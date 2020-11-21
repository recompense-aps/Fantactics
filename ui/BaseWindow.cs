using Godot;
using System;

public class BaseWindow : Node2D
{
    [Signal]
    public delegate void Opened(BaseWindow window);
    [Signal]
    public delegate void Closed(BaseWindow window);

    public virtual float Width => 500;
    public virtual float Height => 700;
    public Control Content {get; protected set;}

    public override void _Ready()
    {
        // Each UI component will be added to the global singleton instance,
        // so they need to be hidden by default
        Hide();

        // Content is the root of the actual UI.
        // Height and width are constant
        Content = new Control();
        Content.RectSize = new Vector2(Width, Height);
        AddChild(Content);
        InitializeContent();
    }

    public virtual void InitializeContent(){}

    /// <summary>
    /// Opens the window and returns a signal awaiter object that can
    /// be awaited in an async method.
    /// </summary>
    public virtual SignalAwaiter Open(object data = null)
    {
        EmitSignal(nameof(Opened), this);
        Show();

        return AnimateOpening();
    }

    /// <summary>
    /// Opens the window at the mouse position and returns a signal awaiter object that can
    /// be awaited in an async method.
    /// </summary>
    public SignalAwaiter OpenAtMouse(object data = null)
    {
        Position = new Vector2(GetTree().Root.GetMousePosition());

        return Open(data);
    }

    /// <summary>
    /// Opens the window at the center of the screen and returns a signal awaiter object that can
    /// be awaited in an async method.
    /// </summary>
    public SignalAwaiter OpenAtCenter(object data = null)
    {
        Position = new Vector2
        (
            ( Global.Config.WindowWidth - Width ) / 2,
            ( Global.Config.WindowHeight - Height ) / 2
        );

        return Open(data);
    }

    public async void Close()
    {
        EmitSignal(nameof(Closed), this);
        await AnimateClosing();
        Hide();
    }

    protected virtual SignalAwaiter AnimateOpening()
    {
        Tween openTween = new Tween();
        openTween.InterpolateProperty(Content, "modulate", new Color(Modulate.r, Modulate.g, Modulate.b, 0), new Color(Modulate.r, Modulate.g, Modulate.b, 1), 1);
        AddChild(openTween);
        openTween.Start();
        return ToSignal(openTween, "tween_completed");
    }

    protected virtual SignalAwaiter AnimateClosing()
    {
        Tween openTween = new Tween();
        openTween.InterpolateProperty(Content, "modulate", new Color(Modulate.r, Modulate.g, Modulate.b, 1), new Color(Modulate.r, Modulate.g, Modulate.b, 0), 1);
        AddChild(openTween);
        openTween.Start();
        return ToSignal(openTween, "tween_completed");
    }
}
