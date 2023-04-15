using Godot;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WordleUI;

public partial class Grid : GridContainer, IWordleUI
{
    private Vector2I GridDimensions;
    private Row[] GridRows;
    public (string, Guess.Accuracy)[][] GridState;
    public bool Used;

    public void Init(int length, int height)
    {
        this.GridDimensions = new Vector2I(length, height);
        this.GridRows = new Row[GridDimensions.Y];
        this.GridState = new (string, Guess.Accuracy)[GridDimensions.Y][];
        this.Used = false;
        for (int i = 0; i < GridRows.Length; i++)
        {
            GridRows[i] = (Row)Constants.RowScene.Instantiate();
            GridRows[i].Init(GridDimensions.X, 1);
            GridState[i] = GridRows[i].GetRowState();
            this.AddChild(GridRows[i]);
        }
        this.LoadGame();
    }

    public bool IsUsed()
    {
        return Used;
    }

    public void SaveGame((string, Guess.Accuracy)[] rowState)
    {
        System.Text.StringBuilder save = new System.Text.StringBuilder();
        for (int i = 0; i < GridDimensions.Y; i++)
        {
            for (int j = 0; j < GridDimensions.X; j++)
            {
                save.Append(GridState[i][j].Item1);
            }
        }
        File.WriteAllText($"src/data/saves/{GridDimensions.X}.txt", save.ToString());
    }

    public void LoadGame(string text = "")
    {
        string save = File.ReadAllText($"src/data/saves/{GridDimensions.X}.txt");

        string guess = string.Empty;
        for (int i = 0; i < save.Length; i++)
        {
            guess += save[i];
            GridRows[i / GridDimensions.X].LoadGame(guess);
            if (guess.Length == GridDimensions.X)
            {
                _OnTextSubmitted(guess.ToLower());
                guess = string.Empty;
            }
        }
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
