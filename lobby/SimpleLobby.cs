using Godot;
using System;
using System.Text.Json;

public class SimpleLobby : Node2D
{
    private RichTextLabel debugText;
    public override void _Ready()
    {
        debugText = GetNode<RichTextLabel>("RichTextLabel");
    }

    private void _on_StartGameButton_pressed()
    {
        StartGame();
    }

    private void _on_JoinGameButton_pressed()
    {

    }

    private async void StartGame()
    {
        FtRequest request = new FtRequest(FtRequestType.CreateGame, new FtRequestData()
        {
            SenderGuid = OS.GetUniqueId(),
            SenderName = System.Environment.MachineName
        });

        HttpResponse response = await Global.Http.Request("http://192.168.1.21:3000/game/create-game", 5000, request.ToJson());
        FtRequestData data = JsonSerializer.Deserialize<FtRequestData>(response.Body);
        debugText.Text = data.Success;
    }
}
