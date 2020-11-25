using Godot;
using System;

public class Building : Area2D
{
    public Controller Controller {get; set;}
    private GamePieceComponent gamePiece;
    private ClickableComponent clickable;

    public static Building Spawn(Building building, Vector2 worldPosition)
    {
        building.GlobalPosition = worldPosition;
        Global.Instance.AddChild(building);
        return building;
    }

    public static Building SpawnWithName(string buildingName, Vector2 worldPosition)
    {
        Building building = null;
        switch(buildingName)
        {
            case nameof(University):
                building = University.Scene.Instance();
                break;
            case nameof(Pit):
                building = Pit.Scene.Instance();
                break;
        }

        if(building != null)
        {
            return Spawn(building, worldPosition);
        }

        return building;
    }

    public static Building SpawnAt(string unitName, Vector2 boardPosition)
    {
        Vector2 worldPosition = Global.ActiveMap.GetWorldPositionFromCell(boardPosition);
        return SpawnWithName(unitName, worldPosition);
    }

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
