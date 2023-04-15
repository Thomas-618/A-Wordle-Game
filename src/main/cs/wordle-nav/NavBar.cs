using Godot;
using System;
using WordleNav;

public partial class NavBar : VBoxContainer, IWordleNav
{
    public void _OnMenuPressed()
    {
        GD.Print("Menu");
        Node menuDialog = Constants.LevelSelectorScene.Instantiate();
        GetTree().Root.GetNode("App").AddChild(menuDialog);
        GetTree().Root.GetNode("App/WordleGame").QueueFree();
    }

    public void _OnHelpPressed()
    {
        GD.Print("Help");
        Node helpDialog = Constants.HelpDialogScene.Instantiate();
        GetOwner<Node>().AddChild(helpDialog);
    }

    public void _OnStatsPressed()
    {
        GD.Print("Stats");
        Node statsDialog = Constants.StatsDialogScene.Instantiate();
        GetOwner<Node>().AddChild(statsDialog);
    }

    public void _OnConfigPressed()
    {
        GD.Print("Config");
        Node configDialog = Constants.ConfigDialogScene.Instantiate();
        GetOwner<Node>().AddChild(configDialog);
    }
}
