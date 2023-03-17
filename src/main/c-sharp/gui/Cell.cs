using Godot;
using Godot.Collections;
using System;

public partial class Cell : LineEdit
{

	private Game ParentGame;
	private Grid ParentGrid;
	private Row ParentRow;

	private AnimationPlayer Animation;
	private float AnimationSpeedFactor; 
	
	private static readonly StyleBox BlankCell = 
		(StyleBox) GD.Load("res://src/main/resources/styles/BlankCell.tres");
	private static readonly StyleBox FullCell = 
		(StyleBox) GD.Load("res://src/main/resources/styles/FullCell.tres");
	private static readonly StyleBox CorrectCell = 
		(StyleBox) GD.Load("res://src/main/resources/styles/CorrectCell.tres");
	private static readonly StyleBox SemiCorrectCell = 
		(StyleBox) GD.Load("res://src/main/resources/styles/SemiCorrectCell.tres");
	private static readonly StyleBox IncorrectCell = 
		(StyleBox) GD.Load("res://src/main/resources/styles/IncorrectCell.tres");

	public void _Init(Game game, Grid grid, Row row)
	{
		this.ParentGame = game;
		this.ParentGrid = grid;
		this.ParentRow = row;

		this.Animation = (AnimationPlayer) this.GetNode("Animation");
		this.AnimationSpeedFactor = 
			(1.5f - 1.0f) / (ParentRow.GetCells().Count * 4.0f);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_text_backspace")) {
			if (HasFocus() && string.IsNullOrEmpty(Text))
			{
				this._OnTextChanged(string.Empty, true);
			} 
		}
		else if (@event.IsActionPressed("ui_accept"))
		{
			if (HasFocus() && string.IsNullOrEmpty(Text))
			{
				this._OnTextChangeRejected(string.Empty);
			}
		}
	}

	private void _OnTextSubmitted(string submittedString) 
	{
		if (this != ParentRow.GetCells()[ParentRow.GetCells().Count - 1] || 
			string.IsNullOrEmpty(Text))
		{
			return;
		}
		string submission = string.Empty;
		foreach (Cell cell in ParentRow.GetCells())
		{
			submission += cell.Text.ToLower();
		}
		if (ParentGame.MakeGuess(submission) != Guess.Result.Invalid)
		{
			this.ReleaseFocus();
		}
	}

	private void _OnTextChanged(string changedText, bool goToPrev)
	{
		Text = changedText.ToUpper();
		CaretColumn = 1;
		AnimationPlayer Animation = (AnimationPlayer) this.GetNode("Animation");
		if (!goToPrev) Animation.Play("ShakeCell");
		if (!string.IsNullOrEmpty(changedText) && 
			!Char.IsLetter(changedText[0])) 
		{
			Text = string.Empty;
			return;
		}
		int i = -1; 
		do i++; while (i < ParentRow.GetCells().Count && this != ParentRow.GetCells()[i]);
		if (!string.IsNullOrEmpty(changedText))
		{
			this.AddThemeStyleboxOverride("normal", FullCell);
			this.AddThemeStyleboxOverride("focus", FullCell);
			if ((i + 1) < ParentRow.GetCells().Count)
			{
				Cell nextCell = (Cell) ParentRow.GetCells()[i + 1];
				nextCell.GrabFocus();
			}
		}
		else
		{
			this.AddThemeStyleboxOverride("normal", BlankCell);
			this.AddThemeStyleboxOverride("focus", BlankCell);
			if ((i - 1) >= 0 && goToPrev)
			{
				Cell prevCell = (Cell) ParentRow.GetCells()[i - 1];
				prevCell.GrabFocus();
			}
			else
			{
				Animation.Play("ShakeCell");
			}
		}
	}

	private void _OnTextChangeRejected(string rejectedText)
	{
		AnimationPlayer Animation = (AnimationPlayer) this.GetNode("Animation");
		Animation.Play("ShakeCell");
	}

	public void DisplayResult(Guess.Result result)
	{
		Tween tween = GetTree().CreateTween();
		switch (result)
		{
			case Guess.Result.Match:
				tween.TweenProperty(this, "position",
						new Vector2(this.Position.X, -48), 0.20f);
				tween.TweenProperty(this, "position",
					new Vector2(this.Position.X, 0), 0.20f);
				tween.TweenProperty(this, "position",
					new Vector2(this.Position.X, -32), 0.20f);
				tween.TweenProperty(this, "position",
					new Vector2(this.Position.X, 0), 0.20f);
				break;
			case Guess.Result.Valid:
				throw new InvalidOperationException(
				"error: attempted to display Guess.Result.Valid");
			case Guess.Result.Invalid:
				throw new InvalidOperationException(
				"error: attempted to display Guess.Result.Invalid");
		}
	}
	public void DisplayAccuracy(int cellNum, Guess.Accuracy accuracy)
	{
		this.ReleaseFocus();
		this.Editable = false;
		Animation.SpeedScale += 
			(float) Math.Min(1.5f, (Math.Pow(cellNum,2) * AnimationSpeedFactor));
		switch (accuracy)
		{
			case Guess.Accuracy.Correct:
				Animation.Play("FlipToCorrectCell");
				break;
			case Guess.Accuracy.SemiCorrect:
				Animation.Play("FlipToSemiCorrectCell");
				break;
			case Guess.Accuracy.Incorrect:
				Animation.Play("FlipToIncorrectCell");
				break;
		}
	}
}
