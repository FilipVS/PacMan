using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// User can trigger certain events with this
    /// </summary>
    class Button : IClickableUIElement
    {
        #region Constructors
        public Button(string text, HorizontalAllignment horizontalAllignment, int lineNumber, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor)
        {
            Text = text;
            HorizontalAllignment = horizontalAllignment;

            RenderPosition = new Vector2D(-1, lineNumber);

            TextColor = Console.ForegroundColor;
            BackgroundColor = Console.BackgroundColor;

            HighlightedTextColor = highlightedTextColor;
            HighlightedBackgroundColor = highlightedBackgroundColor;
        }
        public Button(string text, HorizontalAllignment horizontalAllignment, Vector2D customRenderPosition, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor) : this(text, horizontalAllignment, -1, highlightedTextColor, highlightedBackgroundColor)
        {
            RenderPosition = customRenderPosition;
        }
        public Button(string text, HorizontalAllignment horizontalAllignment, int lineNumber, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, horizontalAllignment, lineNumber, highlightedTextColor, highlightedBackgroundColor)
        {
            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        public Button(string text, HorizontalAllignment horizontalAllignment, Vector2D customRenderPosition, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, ConsoleColor textColor, ConsoleColor backgroundColor) : this(text, horizontalAllignment, -1, highlightedTextColor, highlightedBackgroundColor)
        {
            RenderPosition = customRenderPosition;

            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        #endregion

        public event EventHandler OnClick;

        #region Properties
        public HorizontalAllignment HorizontalAllignment { get; set; }

        public Vector2D RenderPosition { get; set; }

        public string Text { get; }

        public ConsoleColor TextColor { get; }
        public ConsoleColor BackgroundColor { get; }

        public ConsoleColor HighlightedTextColor { get; }
        public ConsoleColor HighlightedBackgroundColor { get; }
        #endregion

        #region Methods
        public void Print()
        {
            TextAlligner.RenderText(Text, HorizontalAllignment, TextColor, BackgroundColor, RenderPosition.Y, RenderPosition.X);
        }

        public void PrintHighlighted(bool highlighted)
        {
            if (highlighted)
                TextAlligner.RenderText(Text, HorizontalAllignment, HighlightedTextColor, HighlightedBackgroundColor, RenderPosition.Y, RenderPosition.X);
            else
                Print();
        }

        public void Clicked(object sender, EventArgs eventArgs)
        {
            if (OnClick == null)
                return;

            OnClick(this, eventArgs);
        }
        #endregion
    }
}
