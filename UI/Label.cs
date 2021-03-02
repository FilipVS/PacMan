using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// Used for displaying text to the user 
    /// </summary>
    public class Label : IUIElement
    {
        #region Constructors
        /// <summary>
        /// Initializes new instance of Label class with information about its line and colors set to current Console colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="lineNumber">On which line should be the element rendered</param>
        public Label(string text, HorizontalAlignment horizontalAlignment, int lineNumber)
        {
            Text = text;
            HorizontalAlignment = horizontalAlignment;

            RenderPosition = new Vector2D(-1, lineNumber);

            TextColor = Console.ForegroundColor;
            BackgroundColor = Console.BackgroundColor;
        }
        /// <summary>
        /// Initializes new instance of Label class with full information about its position and colors set to current Console colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="customRenderPosition">Specific position on the screen, where the element should be rendered (only works with alignment="custom")</param>
        public Label(string text, HorizontalAlignment horizontalAlignment, Vector2D customRenderPosition) : this(text, horizontalAlignment, -1)
        {
            RenderPosition = customRenderPosition;
        }
        /// <summary>
        /// Initializes new instance of Label class with information about its line and colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="lineNumber">On which line should be the element rendered</param>
        /// <param name="textColor">Color of the text</param>
        /// <param name="backgroundColor">Color of the background</param>
        public Label(string text, HorizontalAlignment horizontalAlignment, int lineNumber, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, horizontalAlignment, lineNumber)
        {
            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        /// <summary>
        /// Initializes new instance of Label class with full information about its position and colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="customRenderPosition">Specific position on the screen, where the element should be rendered (only works with alignment="custom")</param>
        /// <param name="textColor">Color of the text</param>
        /// <param name="backgroundColor">Color of the background</param>
        public Label(string text, HorizontalAlignment horizontalAlignment, Vector2D customRenderPosition, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, horizontalAlignment, -1)
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
        #endregion

        #region Methods
        public void Print()
        {
            TextAligner.RenderText(Text, HorizontalAlignment, TextColor, BackgroundColor, RenderPosition.Y, RenderPosition.X);
        }
        #endregion
    }
}
