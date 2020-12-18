using Godot;
using System;

public class Building : GamePiece
{
    public Controller Controller {get; set;}

    public override void _Ready()
    {
        Connect(nameof(Clicked), this, nameof(OnClick));
    }

    ///////////////////////////////////
    //  Public virtual methods
    ///////////////////////////////////
    public virtual void ShowMenu()
    {
        Global.Log("Here we goooo.");
        BasicPopup.Notify("Building clicked");
    }

    public virtual void HoverEffect(bool toggle)
    {
        float alpha = toggle ? 0.5f : 1;
        Modulate = new Color(1, 1, 1, alpha);
    }

    ///////////////////////////////////
    //  Signal Handlers
    //////////////////////////////////
    private void OnClick()
    {
        ShowMenu();
    }
}
