using Godot;
using System;

public class TurnStartScreen : Node2D
{
    private Label label;
    public override void _Ready()
    {
        label = FindNode("Label") as Label;
    }

    public async void Open(string playerName)
    {
        Visible = true;
        label.Text = string.Format("It's {0}'s turn!", playerName);

        await Global.WaitFor(3);

        Close();
    }

    public void Close()
    {
        Visible = false;
    }

}
