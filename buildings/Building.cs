using Godot;
using System;

public class Building : Area2D
{
    public Controller Controller {get; set;}
    private GamePieceComponent gamePiece;
    private ClickableComponent clickable;

    public override void _Ready()
    {
        gamePiece = new GamePieceComponent();
        clickable = new ClickableComponent();

        AddChild(gamePiece);
        AddChild(clickable);

        gamePiece.SnapToGrid();
        clickable.Connect(nameof(ClickableComponent.Clicked), this, nameof(OnClick));
    }

    ///////////////////////////////////
    //  Public virtual methods
    ///////////////////////////////////
    public virtual void ShowMenu()
    {
        BasicPopup.Notify("Building clicked");
    }

    ///////////////////////////////////
    //  Signal Handlers
    //////////////////////////////////
    private void OnClick()
    {
        ShowMenu();
    }
}
