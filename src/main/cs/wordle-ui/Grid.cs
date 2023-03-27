using Godot;
using System;
using WordleUI;

public partial class Grid : GridContainer, IWordleUI
{
    private Vector2 GridDimensions;
    private Row[] GridRows;
    public bool Used;

    public void Init(float length, float height)
    {
        this.GridDimensions = new Vector2(length, height);
        this.GridRows = new Row[(int)GridDimensions.Y];
        this.Used = false;
        for (int i = 0; i < GridRows.Length; i++)
        {
            GridRows[i] = (Row)Constants.RowScene.Instantiate();
            GridRows[i].Init(GridDimensions.X, 1.0f);
            this.AddChild(GridRows[i]);
        }
    }

    public bool IsUsed()
    {
        return Used;
    }

    public void DisplayResult(Guess.Result result)
    {
        for (int i = 0; i < GridRows.Length; i++)
        {
            if (!GridRows[i].IsUsed())
            {
                GridRows[i].DisplayResult(result);
                return;
            }
        }
        this._OnFocusEntered();
    }

    public void DisplayAccuracy(Guess.Accuracy[] accuracy, double duration = 0.0)
    {
        for (int i = 0; i < GridRows.Length; i++)
        {
            if (!GridRows[i].IsUsed())
            {
                GridRows[i].DisplayAccuracy(accuracy, 0.25);
                return;
            }
        }
    }

    public void _OnTextSubmitted(string text)
    {
        GetOwner<Game>().MakeGuess(text);
        this._OnFocusEntered();
    }

    public void _OnTextChanged(string text)
    {
        throw new InvalidOperationException("error: attempted to call text changed on grid");
    }

    public void _OnFocusEntered()
    {
        foreach (Row row in GridRows)
        {
            if (!row.IsUsed())
            {
                row._OnFocusEntered();
                return;
            }
        }
        GridRows[GridRows.Length - 1]._OnFocusEntered();
    }
}
