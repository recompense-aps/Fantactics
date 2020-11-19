using Godot;
using System;

public class LocalController : Controller
{
    private string uniqueLocalName;
    public LocalController(string localName)
    {
        uniqueLocalName = localName;
    }
    public override void InitializeController(string guid = "")
    {
        base.InitializeController(uniqueLocalName);
    }
}
