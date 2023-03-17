using Godot;
using Godot.Collections;
using System;

public partial class Grid : GridContainer
{

	private Game ParentGame;

	private int Length;
	private int Height;
	public Array<Row> Rows = new Array<Row>();

	private readonly PackedScene RowScene = 
		ResourceLoader.Load<PackedScene>("res://src/main/scenes/gui/Row.tscn");

	public override void _Ready() {}

	public void _Init(Game game, int length, int height)
	{
		this.ParentGame = game;
		this.Length = length;
		this.Height = height;
		Rows.Resize(this.Height);
		for (int i = 0; i < this.Height; i++)
		{
			Rows[i] = (Row) RowScene.Instantiate();
			this.AddChild(Rows[i]);
			Rows[i]._Init(ParentGame, this, length);
		} 
		foreach (Row row in Rows)
		{
			row._Ready();
		}
	}

	public void _OnFocusEntered()
	{
		GetCurrentRow()._OnFocusEntered();
	}

	public void DisplayResult(Guess.Result result)
    {
        GetCurrentRow().DisplayResult(result);
    }

	public void DisplayAccuracy(Array<Guess.Accuracy> accuracy)
	{
		GetCurrentRow().DisplayAccuracy(accuracy);
	}

	public Array<Row> GetRows()
	{
		return Rows; 
	}

	public Row GetCurrentRow()
	{
		foreach (Row row in Rows)
		{
			if (row.Cells[0].Editable == true)
			{
				return row;
			}
		}
		throw new InvalidOperationException("error: cannot get current row");
	}
}
