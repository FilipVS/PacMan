using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// Dialog used for getting text input from the user
    /// </summary>
    public class TextInputDialog : UserDialog
    {
        private readonly Vector2D LABEL_TITLE_POSITION = new Vector2D(1, 1);
        private readonly Vector2D LABEL_PROMPT_POSITION = new Vector2D(1, 2);

        private readonly ConsoleColor LABEL_TITLE_FOREGROUND_COLOR = ConsoleColor.Cyan;
        private readonly ConsoleColor LABEL_TITLE_BACKGROUND_COLOR = Console.BackgroundColor;

        private readonly ConsoleColor LABEL_PROMPT_FOREGROUND_COLOR = ConsoleColor.Green;
        private readonly ConsoleColor LABEL_PROMPT_BACKGROUND_COLOR = Console.BackgroundColor;

        #region Constructors
        public TextInputDialog(string prompt) : base(new List<ConsoleKey>())
        {
            LabelTitle = new Label("Please, enter text (press enter to finish): ", HorizontalAlignment.Custom, LABEL_TITLE_POSITION, LABEL_TITLE_FOREGROUND_COLOR, LABEL_TITLE_BACKGROUND_COLOR);
            LabelPrompt = new Label((prompt + ":"), HorizontalAlignment.Custom, LABEL_PROMPT_POSITION, LABEL_PROMPT_FOREGROUND_COLOR, LABEL_PROMPT_BACKGROUND_COLOR);
        }
        #endregion

        #region Properties
        Label LabelTitle { get; set; }

        Label LabelPrompt { get; set; }

        public string DialogStringResult { get; private set; }
        #endregion

        #region Methods
        public override void Run()
        {
            Print();

            Console.SetCursorPosition((LABEL_PROMPT_POSITION.X + LabelPrompt.Text.Length + 1), LABEL_PROMPT_POSITION.Y);

            DialogStringResult = Console.ReadLine();
            Console.CursorVisible = false;

            DialogResult = DialogResult.Yes;
        }

        private void Print()
        {
            Console.Clear();

            LabelTitle.Print();
            LabelPrompt.Print();

            Console.CursorVisible = false;
        }
        #endregion
    }
}
