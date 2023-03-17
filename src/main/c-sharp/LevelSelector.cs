using Godot;
using System;

public partial class LevelSelector : VBoxContainer
{
	private readonly PackedScene GameScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/Game.tscn");
	public void _OnButtonPressed(int levelNum)
	{
		Game game = (Game) GameScene.Instantiate();
		game._Init(levelNum);
		GetNode("..").AddChild(game);
		this.QueueFree();
	}
}
