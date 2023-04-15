using Godot;
using System;
using WordleUI;

public partial class Cell : LineEdit, IWordleUI
{
    private Vector2I CellDimensions;
    private (string, Guess.Accuracy) CellState;
    private bool Used;

    public void Init(int length, int height)
    {
        this.CellDimensions = new Vector2I(length, height);
        this.CellState = (string.Empty, Guess.Accuracy.None);
        this.Used = false;
    }

    public bool IsUsed()
    {
        return Used;
    }

    public void SaveGame((string, Guess.Accuracy) cellState)
    {
        Row parentRow = (Row)GetParent();
        parentRow.SaveGame(cellState);
    }

    public void LoadGame(string text)
    {
        this.CellState.Item1 = text;
        this.Text = CellState.Item1;
        this.Used = true;
    }

    public void DisplayResult(Guess.Result result)
    {
        void BounceCell()
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(this, "position", new Vector2(this.Position.X, -48.0f), 0.20);
            tween.TweenProperty(this, "position", new Vector2(this.Position.X, 0.0f), 0.20);
            tween.TweenProperty(this, "position", new Vector2(this.Position.X, -32.0f), 0.20);
            tween.TweenProperty(this, "position", new Vector2(this.Position.X, 0.0f), 0.20);
        }

        switch (result)
        {
            case Guess.Result.Match:
                BounceCell();
                break;
            case Guess.Result.Valid:
                throw new InvalidOperationException(
                    "error: cell attempted to display Guess.Result.Valid"
                );
            case Guess.Result.Invalid:
                throw new InvalidOperationException(
                    "error: cell attempted to display Guess.Result.Invalid"
                );
        }
    }

    public void DisplayAccuracy(Guess.Accuracy[] accuracy, double duration = 0.0)
    {
        void FlipCell(StyleBox style)
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1.0f, 0.0f), duration);
            tween
                .Parallel()
                .TweenProperty(this, "theme_override_styles/read_only", style, duration);
            tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), duration);
        }

        this.Editable = false;
        this.CellState = (CellState.Item1, accuracy[0]);
        switch (accuracy[0])
        {
            case Guess.Accuracy.Correct:
                FlipCell(Constants.CorrectCell);
                break;
            case Guess.Accuracy.SemiCorrect:
                FlipCell(Constants.SemiCorrectCell);
                break;
            case Guess.Accuracy.Incorrect:
                FlipCell(Constants.IncorrectCell);
                break;
        }
        this.SaveGame(CellState);
    }

    public void _OnTextSubmitted(string _text)
    {
        void ShakeCell()
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.075);
            tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.075);
        }

        Row parentRow = (Row)GetParent();
        if (this.IsUsed())
        {
            parentRow._OnTextSubmitted(string.Empty);
        }
        else
        {
            ShakeCell();
        }
    }

    public void _OnTextChanged(string text)
    {
        void ShakeCell()
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.075);
            tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.075);
        }

        this.Text = text.ToUpper();
        this.CellState = (this.Text, Guess.Accuracy.None);
        this.Used = true;
        if (!string.IsNullOrEmpty(this.Text))
        {
            if (!Char.IsLetter(this.Text[0]))
            {
                this.Text = string.Empty;
                this.Used = false;
                ShakeCell();
                return;
            }
            this.AddThemeStyleboxOverride("normal", Constants.FullCell);
            this.AddThemeStyleboxOverride("focus", Constants.FullCell);
        }
        else
        {
            this.AddThemeStyleboxOverride("normal", Constants.BlankCell);
            this.AddThemeStyleboxOverride("focus", Constants.BlankCell);
            this.Used = false;
        }
        Row parentRow = (Row)GetParent();
        parentRow._OnTextChanged(this.Text);
        this.SaveGame(CellState);
        ShakeCell();
    }

    public void _OnFocusEntered()
    {
        this.GrabFocus();
    }
}
