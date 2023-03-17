using Godot;
using Godot.Collections;
using System;
public partial class Row : HBoxContainer
{
    private Game ParentGame;
	private Grid ParentGrid;

    private int Length; 
	public Array<Cell> Cells = new Array<Cell>();

	private AnimationPlayer Animation;
	private float AnimationDelayFactor;

	private readonly PackedScene CellScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/gui/Cell.tscn");

	public void _Init(Game game, Grid grid, int length)
	{
		this.ParentGame = game;
		this.ParentGrid = grid;
		this.Animation = (AnimationPlayer) this.GetNode("Animation");
		this.Length = length;
		Cells.Resize(this.Length);
		for (int i = 0; i < this.Length; i++)
		{
			Cells[i] = (Cell) CellScene.Instantiate();
			this.GetNode("Cells").AddChild(Cells[i]);
			Cells[i]._Init(ParentGame, ParentGrid, this);
		} 
		foreach (Cell cell in Cells)
		{
			cell._Ready();
		}
		this.AnimationDelayFactor = 
			(0.25f - 0.10f) / (this.GetCells().Count * 0.80f);
	}

	public void _OnFocusEntered()
	{
		GetCurrentCell().GrabFocus();
	}

	public async void DisplayResult(Guess.Result result)
	{
		Tween tween = GetTree().CreateTween();
		switch (result)
		{
			case Guess.Result.Match:
				await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
				foreach (Cell cell in Cells)
				{
					await ToSignal(GetTree().CreateTimer(0.15f), "timeout");
					cell.DisplayResult(result);
				}
				break;
			case Guess.Result.Valid:
				throw new InvalidOperationException(
					"error: attempted to display Guess.Result.Valid"
				);
			case Guess.Result.Invalid:
				for (int i = 0; i < 3; i++)
				{
					tween.TweenProperty(this, "position", 
						new Vector2(8, this.Position.Y), 0.025f);
					tween.TweenProperty(this, "position", 
						new Vector2(0, this.Position.Y), 0.025f);
					tween.TweenProperty(this, "position", 
						new Vector2(-8, this.Position.Y), 0.025f);
					tween.TweenProperty(this, "position", 
						new Vector2(0, this.Position.Y), 0.025f);
				}
				break;
		}
	}

	public async void DisplayAccuracy(Array<Guess.Accuracy> accuracy)
	{
		for (int i = 0; i < Cells.Count; i++)
		{
			await ToSignal(GetTree().CreateTimer(
				Math.Max(0.10f, 0.25f - (i * AnimationDelayFactor))),
				"timeout");
			Cells[i].DisplayAccuracy(i, accuracy[i]);
		}
		ParentGrid._OnFocusEntered();
	}
	public Array<Cell> GetCells()
	{
		return this.Cells; 
	}

	public Cell GetCurrentCell()
	{
		foreach (Cell cell in Cells)
		{
			if (string.IsNullOrEmpty(cell.Text))
			{
				return cell;
			}
		}
		return Cells[Cells.Count - 1];
	}
}
