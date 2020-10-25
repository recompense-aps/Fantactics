using Godot;
using System;

public class BasicPopup : AcceptDialog
{
    public static void Notify(string text)
    {
        BasicPopup p = Global.Instance.FindNode("BasicPopup") as BasicPopup;
        p.DialogText = text;
        p.Show();
    }
    public override void _Ready()
    {
        
    }
    
}
