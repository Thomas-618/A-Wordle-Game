using Godot;
using Godot.Collections;
using System;

public partial class Game : Node
{
    string Answer;
    int WordLength;
    int GuessCount;

    Grid GameGrid;
    Reel PopupReel;

    public void _Init(int wordLength)
    {
        this.WordLength = wordLength;
    }

    public override void _Ready()
    {
        this.Answer = SelectWord();
        this.GameGrid = (Grid)GetNode("Content/Grid");
        this.PopupReel = (Reel)GetNode("Margin/Reel");
        GameGrid.Init(this.WordLength, 6.0f);
		GameGrid.GrabFocus();
    }

    public string SelectWord()
    {
        string selectedWord;
        Random randomize = new Random();
        int bytesPerWord = WordLength + 2;
        string filePath = $"res://src/main/resources/words/selectable/{WordLength}.txt";
        using (FileAccess selectableWords = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
        {
            int totalWords = ((int)selectableWords.GetLength()) / bytesPerWord;
            int randomWord = randomize.Next(0, totalWords) * bytesPerWord;
            selectableWords.Seek((ulong)randomWord);
            selectedWord = selectableWords.GetLine();
        }
        return selectedWord;
    }

    public void MakeGuess(string word)
    {
        Guess guess = new Guess(this, word);
        switch (guess.GetGuessResult())
        {
            case Guess.Result.Match:
                GameGrid.DisplayAccuracy(guess.GetGuessAccuracy());
                GameGrid.DisplayResult(guess.GetGuessResult());
                PopupReel.createPopup("Genius");
                GuessCount++;
                break;
            case Guess.Result.Valid:
				GameGrid.DisplayAccuracy(guess.GetGuessAccuracy());
				GameGrid.DisplayResult(guess.GetGuessResult());
                GuessCount++;
				if (GuessCount >= 6)
				{
					PopupReel.createPopup(this.Answer, duration: 3.0f);
				}
                break;
            case Guess.Result.Invalid:
                GameGrid.DisplayResult(guess.GetGuessResult());
                PopupReel.createPopup("Not in word list");
                break;
        }
    }

    public string GetAnswer()
    {
        return Answer;
    }
}
