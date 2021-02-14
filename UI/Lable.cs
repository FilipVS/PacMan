using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// Used for displaying text to the user 
    /// </summary>
    class Lable : IUIElement
    {
        #region Constructors
        public Lable(string text, HorizontalAllignment horizontalAllignment, int lineNumber)
        {
            Text = text;
            HorizontalAllignment = horizontalAllignment;

            RenderPosition = new Vector2D(-1, lineNumber);

            TextColor = Console.ForegroundColor;
            BackgroundColor = Console.BackgroundColor;
        }
        public Lable(string text, HorizontalAllignment horizontalAllignment, Vector2D customRenderPosition) : this(text, horizontalAllignment, -1)
        {
            RenderPosition = customRenderPosition;
        }
        public Lable(string text, HorizontalAllignment horizontalAllignment, int lineNumber, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, horizontalAllignment, lineNumber)
        {
            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        public Lable(string text, HorizontalAllignment horizontalAllignment, Vector2D customRenderPosition, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, horizontalAllignment, -1)
        {
            RenderPosition = customRenderPosition;

            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        #endregion

        #region Properties
        public HorizontalAllignment HorizontalAllignment { get; set; }

        public Vector2D RenderPosition { get; set; }

        public string Text { get; }

        public ConsoleColor TextColor { get; }
        public ConsoleColor BackgroundColor { get; }
        #endregion

        #region Methods
        public void Print()
        {
            TextAlligner.RenderText(Text, HorizontalAllignment, TextColor, BackgroundColor, RenderPosition.Y, RenderPosition.X);
        }
        #endregion
    }
}
