using Godot;
using System;

namespace WordleUI
{
    public interface IWordleUI
    {
        public void Init(int length, int height) { }

        public bool IsUsed()
        {
            return false;
        }

        public void SaveGame() { }

        public void LoadGame() { }

        public void DisplayResult(Guess.Result result) { }

        public void DisplayAccuracy(Guess.Accuracy[] accuracy, double duration = 0.0) { }

        public void _OnTextSubmitted() { }

        public void _OnTextChanged() { }

        public void _OnFocusEntered() { }
    }

    public static class Constants
    {
        private static readonly string ScenePath = "res://src/main/cs/wordle-ui/{0}.tscn";
        public static readonly PackedScene GridScene = ResourceLoader.Load<PackedScene>(
            string.Format(ScenePath, "Grid")
        );
        public static readonly PackedScene RowScene = ResourceLoader.Load<PackedScene>(
            string.Format(ScenePath, "Row")
        );
        public static readonly PackedScene CellScene = ResourceLoader.Load<PackedScene>(
            string.Format(ScenePath, "Cell")
        );

        private static readonly string StylePath = "res://src/main/cs/wordle-ui/styles/{0}.tres";
        public static readonly StyleBox BlankCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "BlankCell")
        );
        public static readonly StyleBox FullCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "FullCell")
        );
        public static readonly StyleBox CorrectCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "CorrectCell")
        );
        public static readonly StyleBox SemiCorrectCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "SemiCorrectCell")
        );
        public static readonly StyleBox IncorrectCell = ResourceLoader.Load<StyleBox>(
            string.Format(StylePath, "IncorrectCell")
        );
    }
}
