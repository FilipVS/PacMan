using System;

namespace Setnicka.UI
{
    /// <summary>
    /// This class contains methods to align text on different parts of the screen (right, left, middle...)
    /// </summary>
    public static class TextAligner
    {
        /// <summary>
        /// Renders text on specified part of the screen
        /// </summary>
        /// <param name="text">The text to be rendered</param>
        /// <param name="alignment">Where on screen shoudl the text be rendered</param>
        /// <param name="textColor">The color to render the text with</param>
        /// <param name="backgroundColor">The color to render behind the text</param>
        /// <param name="textStartY">The y coordinate on screen, where to render the text</param>
        /// <param name="textStartX">The x coordinate on screen, where to render the text, only necessary with allignment='Custom'</param>
        public static void RenderText(string text, HorizontalAlignment alignment, ConsoleColor textColor, ConsoleColor backgroundColor, int textStartY, int textStartX = -1)
        {
            if (textStartY < 0)
                throw new ArgumentException("The text y position cannot be negative!");

            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    RenderTextLeft(text, textColor, backgroundColor, textStartY);
                    break;
                case HorizontalAlignment.Right:
                    RenderTextRight(text, textColor, backgroundColor, textStartY);
                    break;
                case HorizontalAlignment.Center:
                    RenderTextCenter(text, textColor, backgroundColor, textStartY);
                    break;
                case HorizontalAlignment.Custom:
                    if (textStartX < 0)
                        throw new ArgumentException("The text x position cannot be negative!");
                    RenderTextCustom(text, textColor, backgroundColor, textStartY, textStartX);
                    break;
                default:
                    throw new ArgumentException("Unknown allignment setting!");
            }
        }

        /// <summary>
        /// Renders text consisting of unhighlighted and highlighted part on a specific part of the screen
        /// </summary>
        /// <param name="unhighlightedText">The text that should not be highlighted (renders first)</param>
        /// <param name="highlightedText">The text that should be highlighted (renders second)</param>
        /// <param name="alignment">Where on screen shoudl the text be rendered</param>
        /// <param name="unhighlightedTextColor">Unhighlighted text's color</param>
        /// <param name="unhighlightedBackgroundColor">Unhighlighted background color</param>
        /// <param name="highlightedTextColor">Highlighted text's color</param>
        /// <param name="highlightedBackgroundColor">Highlighted background color</param>
        /// <param name="textStartY">The y coordinate on screen, where to render the text</param>
        /// <param name="textStartX">The x coordinate on screen, where to render the text, only necessary with allignment=custom</param>
        public static void RenderText(string unhighlightedText, string highlightedText, HorizontalAlignment alignment, ConsoleColor unhighlightedTextColor, ConsoleColor unhighlightedBackgroundColor, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, int textStartY, int textStartX = -1)
        {
            if (textStartY < 0)
                throw new ArgumentException("The text y position cannot be negative!");


            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    RenderText(unhighlightedText, HorizontalAlignment.Left, unhighlightedTextColor, unhighlightedBackgroundColor, textStartY);
                    RenderText(highlightedText, HorizontalAlignment.Custom, highlightedTextColor, highlightedBackgroundColor, textStartY, (0 + unhighlightedText.Length));
                    break;
                case HorizontalAlignment.Right:
                    RenderText((unhighlightedText + highlightedText), HorizontalAlignment.Right, unhighlightedTextColor, unhighlightedBackgroundColor, textStartY);
                    RenderText(highlightedText, HorizontalAlignment.Right, highlightedTextColor, highlightedBackgroundColor, textStartY);
                    break;
                case HorizontalAlignment.Center:
                    RenderTextCenter(unhighlightedText, highlightedText, unhighlightedTextColor, unhighlightedBackgroundColor, highlightedTextColor, highlightedBackgroundColor, textStartY);
                    break;
                case HorizontalAlignment.Custom:
                    if (textStartX < 0)
                        throw new ArgumentException("The text x position cannot be negative!");

                    RenderText(unhighlightedText, HorizontalAlignment.Custom, unhighlightedTextColor, unhighlightedBackgroundColor, textStartY, textStartX);
                    RenderText(highlightedText, HorizontalAlignment.Custom, highlightedTextColor, highlightedBackgroundColor, textStartY, (textStartX + unhighlightedText.Length));
                    break;
                default:
                    throw new ArgumentException("Unknown allignment setting!");
            }
        }


        private static void RenderTextLeft(string text, ConsoleColor textColor, ConsoleColor backgroundColor, int textStartY)
        {
            Console.SetCursorPosition(0, textStartY);
            WriteText(text, textColor, backgroundColor);
        }

        private static void RenderTextRight(string text, ConsoleColor textColor, ConsoleColor backgroundColor, int textStartY)
        {
            int textLength = text.Length;

            if (textLength == 0)
                return;

            // Set the cursor, so the text just fits into the windown from the right
            Console.SetCursorPosition(Console.BufferWidth - textLength, textStartY);

            WriteText(text, textColor, backgroundColor);
        }

        private static void RenderTextCenter(string text, ConsoleColor textColor, ConsoleColor backgroundColor, int textStartY)
        {
            int textLength = text.Length;

            if (textLength == 0)
                return;

            int halfTextLength = textLength / 2;
            int halfScreenWidth = Console.BufferWidth / 2;

            Console.SetCursorPosition(halfScreenWidth - halfTextLength, textStartY);

            WriteText(text, textColor, backgroundColor);
        }
        private static void RenderTextCenter(string unhighlightedText, string highlightedText, ConsoleColor unhighlightedTextColor, ConsoleColor unhighlightedBackgroundColor, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, int textStartY)
        {
            int textLength = (unhighlightedText + highlightedText).Length;

            if (textLength == 0)
                return;

            int halfTextLength = textLength / 2;
            int halfScreenWidth = Console.BufferWidth / 2;

            Console.SetCursorPosition(halfScreenWidth - halfTextLength, textStartY);
            WriteText(unhighlightedText, unhighlightedTextColor, unhighlightedBackgroundColor);

            Console.SetCursorPosition(halfScreenWidth - halfTextLength + unhighlightedText.Length, textStartY);
            WriteText(highlightedText, highlightedTextColor, highlightedBackgroundColor);
        }

        private static void RenderTextCustom(string text, ConsoleColor textColor, ConsoleColor backgroundColor, int textStartY, int textStartX)
        {
            Console.SetCursorPosition(textStartX, textStartY);

            WriteText(text, textColor, backgroundColor);
        }

        /// <summary>
        /// Method that performs the writing intself
        /// </summary>
        private static void WriteText(string text, ConsoleColor textColor, ConsoleColor backgroundColor)
        {
            ConsoleColor previousForegroundColor = Console.ForegroundColor;
            ConsoleColor previousBackgroundColor = Console.BackgroundColor;

            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;

            Console.Write(text);

            Console.ForegroundColor = previousForegroundColor;
            Console.BackgroundColor = previousBackgroundColor;

            Console.CursorVisible = false;
        }
    }
}
