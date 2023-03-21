using Godot;
using System;
using WordleNav;

public partial class NavBar : VBoxContainer, IWordleNav
{
    public void _OnMenuPressed()
    {
        GD.Print("Menu");
    }

    public void _OnHelpPressed()
    {
        GD.Print("Help");
    }

    public void _OnStatsPressed()
    {
        GD.Print("Stats");
    }

    public void _OnSettingsPressed()
    {
        GD.Print("Settings");
    }
}
