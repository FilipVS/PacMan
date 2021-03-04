using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// This class allows the user to input numbers
    /// </summary>
    public class NumberField : IInputtableUIElement
    {
        public const int MAXIMUM_DIGITS = 5;

        public event EventHandler ElementUnhighlighted;

        #region Constructors
        /// <summary>
        /// Initializes new instance of NumberField class with information about its line and colors set to current Console colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="lineNumber">On which line should be the element rendered</param>
        /// <param name="numberOfDigitsToEnter">How many digits will the user be able to enter (has to be between 0 and MAXIMUM_DIGITS)</param>
        public NumberField(string text, int startingDigit, int numberOfDigitsToEnter, HorizontalAlignment horizontalAlignment, int lineNumber, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor)
        {
            Text = text;
            HorizontalAlignment = horizontalAlignment;

            RenderPosition = new Vector2D(-1, lineNumber);

            TextColor = Console.ForegroundColor;
            BackgroundColor = Console.BackgroundColor;

            HighlightedTextColor = highlightedTextColor;
            HighlightedBackgroundColor = highlightedBackgroundColor;

            if (numberOfDigitsToEnter < 0 || numberOfDigitsToEnter > MAXIMUM_DIGITS)
                throw new ArgumentException("numberOfDigits has to be between 0 and MAXIMUM_DIGITS!");
            NumberOfDigits = numberOfDigitsToEnter;
            NumberCharArray = new char[NumberOfDigits];
            CursorAt = 1;
            NumberCharArray[0] = startingDigit.ToString().ToCharArray()[0];
        }
        /// <summary>
        /// Initializes new instance of Button class with full information about its position and unhighlighted colors set to current Console colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="customRenderPosition">Specific position on the screen, where the element should be rendered (only works with alignment="custom")</param>
        /// <param name="highlightedTextColor">The text color, when the element is highlighted</param>
        /// <param name="highlightedBackgroundColor">The background color, when the element is highlighted</param>
        /// <param name="action">An action should be performed under ceratin circumstances</param>
        public NumberField(string text, int startingDigit, int numberOfDigitsToEnter, HorizontalAlignment horizontalAlignment, Vector2D customRenderPosition, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor) : this(text, startingDigit, numberOfDigitsToEnter, horizontalAlignment, -1, highlightedTextColor, highlightedBackgroundColor)
        {
            RenderPosition = customRenderPosition;
        }
        /// <summary>
        /// Initializes new instance of Button class with information about its line and full information about the unhighlighted colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="lineNumber">On which line should be the element rendered</param>
        /// <param name="highlightedTextColor">The text color, when the element is highlighted</param>
        /// <param name="highlightedBackgroundColor">The background color, when the element is highlighted</param>
        /// <param name="textColor">The text color, when the element is not highlighted</param>
        /// <param name="backgroundColor">The background color, when the element is not highlighted</param>
        /// <param name="action">An action should be performed under ceratin circumstances</param>
        public NumberField(string text, int startingDigit, int numberOfDigitsToEnter, HorizontalAlignment horizontalAlignment, int lineNumber, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, startingDigit, numberOfDigitsToEnter, horizontalAlignment, lineNumber, highlightedTextColor, highlightedBackgroundColor)
        {
            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        /// <summary>
        /// Initializes new instance of Button class with full information about its position and unhighlighted colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="customRenderPosition">Specific position on the screen, where the element should be rendered (only works with alignment="custom")</param>
        /// <param name="highlightedTextColor">The text color, when the element is highlighted</param>
        /// <param name="highlightedBackgroundColor">The background color, when the element is highlighted</param>
        /// <param name="textColor">The text color, when the element is not highlighted</param>
        /// <param name="backgroundColor">The background color, when the element is not highlighted</param>
        /// <param name="action">An action should be performed under ceratin circumstances</param>
        public NumberField(string text, int startingDigit, int numberOfDigitsToEnter, HorizontalAlignment horizontalAlignment, Vector2D customRenderPosition, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, startingDigit, numberOfDigitsToEnter, horizontalAlignment, -1, highlightedTextColor, highlightedBackgroundColor)
        {
            RenderPosition = customRenderPosition;

            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        #endregion

        #region Properties
        public HorizontalAlignment HorizontalAlignment { get; set; }

        public Vector2D RenderPosition { get; set; }

        public string Text { get; }

        public ConsoleColor TextColor { get; }
        public ConsoleColor BackgroundColor { get; }

        public ConsoleColor HighlightedTextColor { get; }
        public ConsoleColor HighlightedBackgroundColor { get; }

        // Properties that are used for handling the user input
        private int NumberOfDigits { get; }
        private int CursorAt { get; set; }
        private char[] NumberCharArray { get; set; }

        // Returns the number in format for printing
        private string NumberString
        {
            get
            {
                if (CursorAt == 0)
                    return new string(NumberCharArray);
                else 
                {
                    string result = String.Empty;

                    for (int i = 0; i < CursorAt; i++)
                        result += NumberCharArray[i].ToString();

                    return result;
                }
            }
        }

        // This property returns the number stored in the char array as int (if it fails, it returns -1)
        public int Number
        {
            get
            {
                int number;
                if (int.TryParse(NumberString, out number))
                    return number;
                else
                    return -1;
            }
        }        
        #endregion

        #region Methods
        public void HandleInput(object sender, KeyEventArgs input, bool reprintHighlighted)
        {
            ConsoleKey keyInput = input.keyPressed;
            char number;

            // Checks, whether the input is a number
            switch (keyInput)
            {
                case ConsoleKey.NumPad0:
                case ConsoleKey.D0:
                    number = '0';
                    break;
                case ConsoleKey.NumPad1:
                case ConsoleKey.D1:
                    number = '1';
                    break;
                case ConsoleKey.NumPad2:
                case ConsoleKey.D2:
                    number = '2';
                    break;
                case ConsoleKey.NumPad3:
                case ConsoleKey.D3:
                    number = '3';
                    break;
                case ConsoleKey.NumPad4:
                case ConsoleKey.D4:
                    number = '4';
                    break;
                case ConsoleKey.NumPad5:
                case ConsoleKey.D5:
                    number = '5';
                    break;
                case ConsoleKey.NumPad6:
                case ConsoleKey.D6:
                    number = '6';
                    break;
                case ConsoleKey.NumPad7:
                case ConsoleKey.D7:
                    number = '7';
                    break;
                case ConsoleKey.NumPad8:
                case ConsoleKey.D8:
                    number = '8';
                    break;
                case ConsoleKey.NumPad9:
                case ConsoleKey.D9:
                    number = '9';
                    break;
                default:
                    return;
            }

            // Delete the previous text
            DeletePrint();

            NumberCharArray[CursorAt] = number;
            if (CursorAt >= (NumberCharArray.Length - 1))
                CursorAt = 0;
            else
                CursorAt++;

            if (reprintHighlighted)
                PrintHighlighted(true);
            else
                Print();
        }

        public void Print()
        {
            TextAligner.RenderText(Text, NumberString, HorizontalAlignment.Center, TextColor, BackgroundColor, TextColor, BackgroundColor, RenderPosition.Y, RenderPosition.X);
        }

        private void DeletePrint()
        {
            TextAligner.RenderText(Text, NumberString, HorizontalAlignment.Center, Console.BackgroundColor, Console.BackgroundColor, Console.BackgroundColor, Console.BackgroundColor, RenderPosition.Y, RenderPosition.X);
        }

        public void PrintHighlighted(bool highlighted)
        {
            if (highlighted)
                TextAligner.RenderText(Text, NumberString, HorizontalAlignment.Center, TextColor, BackgroundColor, HighlightedTextColor, HighlightedBackgroundColor, RenderPosition.Y, RenderPosition.X);
            else
                Print();
        }

        public void OnElementUnhighlighted()
        {
            if(ElementUnhighlighted != null)
                ElementUnhighlighted(this, EventArgs.Empty);
        }
        #endregion
    }
}
