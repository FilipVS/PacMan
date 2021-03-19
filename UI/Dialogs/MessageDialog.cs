using System;
using System.Collections.Generic;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// Class used for messaging something to the user
    /// </summary>
    public class MessageDialog : UserDialog
    {
        private readonly Vector2D LABEL_TITLE_POSITION = new Vector2D(1, 1);
        private readonly Vector2D LABEL_EXIT_INFO_POSITION = new Vector2D(1, 2);

        private readonly ConsoleColor LABEL_TITLE_FOREGROUND_COLOR = ConsoleColor.Cyan;
        private readonly ConsoleColor LABEL_TITLE_BACKGROUND_COLOR = ConsoleColor.Black;

        private readonly ConsoleColor LABEL_EXIT_INFO_FOREGROUND_COLOR = ConsoleColor.White;
        private readonly ConsoleColor LABEL_EXIT_INFO_BACKGROUND_COLOR = ConsoleColor.Black;

        private const string LABEL_EXIT_INFO_TEXT = "Press any key to exit";

        #region Constructors
        public MessageDialog(string message) : base(new List<ConsoleKey>())
        {
            MainLabel = new Label(message, HorizontalAlignment.Custom, LABEL_TITLE_POSITION, LABEL_TITLE_FOREGROUND_COLOR, LABEL_TITLE_BACKGROUND_COLOR);

            ExitInfo = new Label(LABEL_EXIT_INFO_TEXT, HorizontalAlignment.Custom, LABEL_EXIT_INFO_POSITION, LABEL_EXIT_INFO_FOREGROUND_COLOR, LABEL_EXIT_INFO_BACKGROUND_COLOR);
        }
        #endregion

        #region Properties
        Label MainLabel { get; set; }

        Label ExitInfo { get; set; }
        #endregion

        #region Methods
        public override void Run()
        {
            // Remove any leftover key presses
            while (Console.KeyAvailable)
                Console.ReadKey(false);

            Print();

            // Wait for the user to acknowledge the message
            Console.ReadKey(true);
        }

        private void Print()
        {
            Console.Clear();

            MainLabel.Print();
            ExitInfo.Print();

            Console.CursorVisible = false;
        }
        #endregion
    }
}
