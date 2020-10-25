using Godot;
using System;
using System.Text.Json;

public class SimpleLobby : Node2D
{
    private RichTextLabel debugText;
    public override void _Ready()
    {
        debugText = GetNode<RichTextLabel>("RichTextLabel");
        BasicPopup.Notify("Testing");
    }

    private void _on_StartGameButton_pressed()
    {
        CreateGame();
    }

    private void _on_JoinGameButton_pressed()
    {
        JoinGame();
    }

    private async void CreateGame()
    {
        FtRequestData data = await Game.Service.CreateGame();
        if(!string.IsNullOrEmpty(data.Success))
        {
            debugText.Text = data.Success + "\n Starting game...";
            await Global.WaitFor(2);

            Global.LocalController = new NetworkController()
            {
                HasInitiative = true
            };

            GetTree().ChangeScene("res://games/SimpleGame.tscn");
        }
        else
        {
            debugText.Text = data.Error;
        }
    }

    private async void JoinGame()
    {
        FtRequestData data = await Game.Service.JoinGame();
        if(!string.IsNullOrEmpty(data.Success))
        {
            debugText.Text = data.Success + "\n Joining game...";
            await Global.WaitFor(2);

            Global.LocalController = new NetworkController()
            {
                HasInitiative = false
            };

            GetTree().ChangeScene("res://games/SimpleGame.tscn");
        }
        else
        {
            debugText.Text = data.Error;
        }
    }
}
