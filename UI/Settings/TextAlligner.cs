using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// This class contains methods to allign text on different parts of the screen (right, left, middle...)
    /// </summary>
    public static class TextAlligner
    {
        /// <summary>
        /// Renders text on specified part of the screen
        /// </summary>
        /// <param name="text">The text to be rendered</param>
        /// <param name="allignment">Where on screen shoudl the text be rendered</param>
        /// <param name="textColor">The color to render the text with</param>
        /// <param name="backgroundColor">The color to render behind the text</param>
        /// <param name="textStartY">The y coordinate on screen, where to render the text</param>
        /// <param name="textStartX">The x coordinate on screen, where to render the text, only necessary with allignment=custom</param>
        public static void RenderText(string text, HorizontalAllignment allignment, ConsoleColor textColor, ConsoleColor backgroundColor, int textStartY, int textStartX = -1)
        {
            if (textStartY < 0)
                throw new ArgumentException("The text y position cannot be negative!");

            switch (allignment)
            {
                case HorizontalAllignment.Left:
                    RenderTextLeft(text, textColor, backgroundColor, textStartY);
                    break;
                case HorizontalAllignment.Right:
                    RenderTextRight(text, textColor, backgroundColor, textStartY);
                    break;
                case HorizontalAllignment.Center:
                    RenderTextCenter(text, textColor, backgroundColor, textStartY);
                    break;
                case HorizontalAllignment.Custom:
                    if (textStartX < 0)
                        throw new ArgumentException("The text x position cannot be negative!");
                    RenderTextCustom(text, textColor, backgroundColor, textStartY, textStartX);
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

        private static void RenderTextCustom(string text, ConsoleColor textColor, ConsoleColor backgroundColor, int textStartY, int textStartX)
        {
            Console.SetCursorPosition(textStartX, textStartY);

            WriteText(text, textColor, backgroundColor);
        }

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
