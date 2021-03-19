using System;
using System.Collections.Generic;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// Dialog used for getting text input from the user
    /// </summary>
    public class TextInputDialog : UserDialog
    {
        #region Constants
        private readonly Vector2D LABEL_TITLE_POSITION = new Vector2D(1, 1);
        private readonly Vector2D LABEL_PROMPT_POSITION = new Vector2D(1, 2);
        private readonly Vector2D LABEL_EXTRA_INFO_POSITION = new Vector2D(1, 4);

        private readonly ConsoleColor LABEL_TITLE_FOREGROUND_COLOR = ConsoleColor.Cyan;
        private readonly ConsoleColor LABEL_TITLE_BACKGROUND_COLOR = Console.BackgroundColor;

        private readonly ConsoleColor LABEL_PROMPT_FOREGROUND_COLOR = ConsoleColor.Green;
        private readonly ConsoleColor LABEL_PROMPT_BACKGROUND_COLOR = Console.BackgroundColor;

        private readonly ConsoleColor LABEL_EXTRA_INFO_FOREGROUND_COLOR = ConsoleColor.White;
        private readonly ConsoleColor LABEL_EXTRA_INFO_BACKGROUND_COLOR = Console.BackgroundColor;
        #endregion

        #region Constructors
        public TextInputDialog(string prompt, string extraInfo = null) : base(new List<ConsoleKey>())
        {
            // Initialize the labels

            LabelTitle = new Label("Please, enter text (press enter to finish)", HorizontalAlignment.Custom, LABEL_TITLE_POSITION, LABEL_TITLE_FOREGROUND_COLOR, LABEL_TITLE_BACKGROUND_COLOR);
            LabelPrompt = new Label((prompt + ":"), HorizontalAlignment.Custom, LABEL_PROMPT_POSITION, LABEL_PROMPT_FOREGROUND_COLOR, LABEL_PROMPT_BACKGROUND_COLOR);

            if (!String.IsNullOrEmpty(extraInfo))
                LabelExtraInfo = new Label($"({extraInfo})", HorizontalAlignment.Custom, LABEL_EXTRA_INFO_POSITION, LABEL_EXTRA_INFO_FOREGROUND_COLOR, LABEL_EXTRA_INFO_BACKGROUND_COLOR);
        }
        #endregion

        #region Properties
        Label LabelTitle { get; set; }

        Label LabelPrompt { get; set; }

        Label LabelExtraInfo { get; set; }

        public string DialogStringResult { get; private set; }
        #endregion

        #region Methods
        public override void Run()
        {
            Print();

            Console.SetCursorPosition((LABEL_PROMPT_POSITION.X + LabelPrompt.Text.Length + 1), LABEL_PROMPT_POSITION.Y);

            DialogStringResult = Console.ReadLine();
            Console.CursorVisible = false;

            // TODO: If problems occur, it is because of this
            if (String.IsNullOrEmpty(DialogStringResult))
                DialogResult = DialogResult.No;
            else
                DialogResult = DialogResult.Yes;
        }

        private void Print()
        {
            Console.Clear();

            LabelTitle.Print();
            LabelPrompt.Print();

            if (LabelExtraInfo != null)
                LabelExtraInfo.Print();

            Console.CursorVisible = false;
        }
        #endregion
    }
}
