using Godot;
using Godot.Collections;
using System;

public partial class Guess : Node
{

	private readonly Game GameGame;
	private readonly string GuessedWord;
	private readonly Result GuessResult;
	private readonly Array<Accuracy> GuessAccuracy;

	public enum Result {Match, Valid, Invalid}
	public enum Accuracy {Correct, SemiCorrect, Incorrect}

	public Guess(Game game, string guess)
	{	this.GameGame = game;
		this.GuessedWord = guess;
		this.GuessResult = CheckGuessResult();
		this.GuessAccuracy = CheckGuessAccuracy();
	}

	private Result CheckGuessResult() 
	{
		if (GuessedWord == GameGame.GetAnswer()) return Result.Match;
		string filePath = 
			$"res://src/main/resources/words/all/{GameGame.GetAnswer().Length}/{GuessedWord[0]}.txt";
		using (FileAccess words = 
				FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
		{
			while (words.GetPosition() < words.GetLength())
			{
				if (GuessedWord == words.GetLine())
				{
					return Result.Valid;
				}
			}
		}
		return Result.Invalid;
	}

	private Array<Accuracy> CheckGuessAccuracy() 
	{
		Array<Accuracy> answerAccuracy = new Array<Accuracy>();
		Array<Accuracy> guessAccuracy = new Array<Accuracy>();
		answerAccuracy.Resize(GameGame.GetAnswer().Length);
		guessAccuracy.Resize(GameGame.GetAnswer().Length);

		for (int i = 0; i < GameGame.GetAnswer().Length; i++)
		{
			if (GuessedWord[i] == GameGame.GetAnswer()[i])
			{
				answerAccuracy[i] = Accuracy.Correct;
				guessAccuracy[i] = Accuracy.Correct;
			} 
			else 
			{
				answerAccuracy[i] = Accuracy.Incorrect;
				guessAccuracy[i] = Accuracy.Incorrect;
			}
		}
		for (int i = 0; i < GameGame.GetAnswer().Length; i++)
		{
			if (guessAccuracy[i] == Accuracy.Incorrect) 
			{
				for (int j = GameGame.GetAnswer().IndexOf(GuessedWord[i]); 
					j > -1; j = GameGame.GetAnswer().IndexOf(GuessedWord[i], j + 1))
				{
					if (answerAccuracy[j] == Accuracy.Incorrect)
					{
						answerAccuracy[j] = Accuracy.SemiCorrect;
						guessAccuracy[i] = Accuracy.SemiCorrect;
						break;
					}
				}
			}
		}
		return guessAccuracy;
	}

	public Result GetGuessResult()
	{
		return this.GuessResult;
	}

	public Array<Accuracy> GetGuessAccuracy()
	{
		if (this.GuessResult == Result.Invalid) {
			throw new InvalidOperationException(
				"error: attempted to get accuracy of invalid guess");
		}
		return this.GuessAccuracy;
	}
}
