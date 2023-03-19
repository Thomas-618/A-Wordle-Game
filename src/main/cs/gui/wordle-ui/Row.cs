using Godot;
using System;
using WordleUI;

public partial class Row : HBoxContainer, BaseWordleUI
{

    private Vector2 RowDimensions;
    private Cell[] RowCells;
    private bool Used;

    public void Init(float length, float height)
    {
        this.RowDimensions = new Vector2(length, height);
        this.RowCells = new Cell[(int)RowDimensions.X];
        this.Used = false;
        for (int i = 0; i < RowCells.Length; i++)
        {
            RowCells[i] = (Cell)Constants.CellScene.Instantiate();
            RowCells[i].Init(1.0f, 1.0f);
            this.AddChild(RowCells[i]);
        }
    }

    public bool IsUsed()
    {
        return Used;
    }

    public void DisplayResult(Guess.Result result)
    {
        async void BounceRow()
        {
            await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
            foreach (Cell cell in RowCells)
            {
                await ToSignal(GetTree().CreateTimer(0.15f), "timeout");
                cell.DisplayResult(result);
            }
        }

        void ShakeRow()
        {
            Tween tween = GetTree().CreateTween();
            for (int i = 0; i < 3; i++)
            {
                tween.TweenProperty(this, "position",
                    new Vector2(8.0f, this.Position.Y), 0.025);
                tween.TweenProperty(this, "position",
                    new Vector2(0.0f, this.Position.Y), 0.025);
                tween.TweenProperty(this, "position",
                    new Vector2(-8.0f, this.Position.Y), 0.025);
                tween.TweenProperty(this, "position",
                    new Vector2(0.0f, this.Position.Y), 0.025);
            }
        }

        switch (result)
        {
            case Guess.Result.Match:
                BounceRow();
                break;
            case Guess.Result.Valid:
                throw new InvalidOperationException(
                    "error: row attempted to display Guess.Result.Valid");
            case Guess.Result.Invalid:
                ShakeRow();
                break;
        }
    }

    public void DisplayAccuracy(Guess.Accuracy[] accuracy, double duration = 0.0)
    {
        async void FlipRow()
        {
            double percentDecay = 1.0;
            for (int i = 0; i < RowCells.Length; i++)
            {
                await ToSignal(GetTree().CreateTimer(duration * percentDecay),
                    "timeout");
                RowCells[i].DisplayAccuracy(new Guess.Accuracy[] { accuracy[i] },
                    0.15 * percentDecay);
                percentDecay -= i * (0.01 - (0.0005 * RowCells.Length));
                GD.Print(percentDecay);
            }
        }

        this.Used = true;
        FlipRow();
    }

    public void _OnTextSubmitted(string text)
    {
        Grid parentGrid = (Grid)GetParent();
        foreach (Cell cell in RowCells)
        {
            text += cell.Text.ToLower();
        }
        parentGrid._OnTextSubmitted(text);
    }

    public void _OnTextChanged(string text)
    {
        this._OnFocusEntered();
    }

    public void _OnFocusEntered()
    {
        foreach (Cell cell in RowCells)
        {
            if (!cell.IsUsed())
            {
                cell._OnFocusEntered();
                return;
            }
        }
        RowCells[RowCells.Length - 1]._OnFocusEntered();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!this.IsUsed())
        {
            if (@event.IsActionReleased("ui_accept"))
            {
                foreach (Cell cell in RowCells)
                {
                    if (!cell.IsUsed() && cell.HasFocus())
                    {
                        cell._OnTextChanged(string.Empty);
                    }
                }
            }
            if (@event.IsActionReleased("ui_text_backspace"))
            {
                for (int i = 0; i < RowCells.Length; i++)
                {
                    if (!RowCells[i].HasFocus()) continue;

                    if (i == 0)
                    {
                        RowCells[i]._OnTextChanged(string.Empty);
                    }
                    else if (!RowCells[i].IsUsed())
                    {
                        RowCells[i - 1]._OnTextChanged(string.Empty);
                    }
                    else if (i == RowCells.Length - 1)
                    {
                        RowCells[RowCells.Length - 1]._OnTextChanged(string.Empty);
                    }
                }
            }
        }
    }
}
