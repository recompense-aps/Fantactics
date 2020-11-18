using Godot;
using System.Collections.Generic;

public class BasicTreeDisplay : Tree
{
    public override void _Ready()
    {
        AddTreeSection("Base Stats", new List<string>()
        {
            "Attack: 5",
            "Defense: 3",
            "Speed: 4",
            "Range: 1"
        });
    }

    private void AddTreeSection(string title, List<string> data)
    {
        TreeItem section = CreateItem(this);
        section.SetText(0, title);
        data.ForEach(item => {
            TreeItem itemTreeNode = CreateItem(section);
            itemTreeNode.SetText(0, item);
        });
    }
}
