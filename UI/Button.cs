using System;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// User can trigger certain events with this
    /// </summary>
    public class Button : IClickableUIElement, IActionable
    {
        public event EventHandler ElementUnhighlighted;

        #region Constructors
        /// <summary>
        /// Initializes new instance of Button class with information about its line and unhighlighted colors set to current Console colors
        /// </summary>
        /// <param name="text">What is written on the element</param>
        /// <param name="horizontalAlignment">Where will the element be rendered</param>
        /// <param name="lineNumber">On which line should be the element rendered</param>
        /// <param name="highlightedTextColor">The text color, when the element is highlighted</param>
        /// <param name="highlightedBackgroundColor">The background color, when the element is highlighted</param>
        /// <param name="action">An action should be performed under ceratin circumstances</param>
        public Button(string text, HorizontalAlignment horizontalAlignment, int lineNumber, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, Action action = null)
        {
            Text = text;
            HorizontalAlignment = horizontalAlignment;

            RenderPosition = new Vector2D(-1, lineNumber);

            TextColor = Console.ForegroundColor;
            BackgroundColor = Console.BackgroundColor;

            HighlightedTextColor = highlightedTextColor;
            HighlightedBackgroundColor = highlightedBackgroundColor;

            Action = action;
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
        public Button(string text, HorizontalAlignment horizontalAlignment, Vector2D customRenderPosition, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, Action action = null) : this(text, horizontalAlignment, -1, highlightedTextColor, highlightedBackgroundColor, action)
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
        public Button(string text, HorizontalAlignment horizontalAlignment, int lineNumber, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, ConsoleColor textColor, ConsoleColor backgroundColor, Action action = null) : this(text, horizontalAlignment, lineNumber, highlightedTextColor, highlightedBackgroundColor, action)
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
        public Button(string text, HorizontalAlignment horizontalAlignment, Vector2D customRenderPosition, ConsoleColor highlightedTextColor, ConsoleColor highlightedBackgroundColor, ConsoleColor textColor, ConsoleColor backgroundColor, Action action = null) : this(text, horizontalAlignment, -1, highlightedTextColor, highlightedBackgroundColor, action)
        {
            RenderPosition = customRenderPosition;

            TextColor = textColor;
            BackgroundColor = backgroundColor;
        }
        #endregion

        public event EventHandler OnClick;

        #region Properties
        public HorizontalAlignment HorizontalAlignment { get; set; }

        public Vector2D RenderPosition { get; set; }

        public string Text { get; }

        public ConsoleColor TextColor { get; }
        public ConsoleColor BackgroundColor { get; }

        public ConsoleColor HighlightedTextColor { get; }
        public ConsoleColor HighlightedBackgroundColor { get; }

        public Action Action { get; }
        #endregion

        #region Methods
        public void Print()
        {
            TextAligner.RenderText(Text, HorizontalAlignment, TextColor, BackgroundColor, RenderPosition.Y, RenderPosition.X);
        }

        public void PrintHighlighted(bool highlighted)
        {
            if (highlighted)
                TextAligner.RenderText(Text, HorizontalAlignment, HighlightedTextColor, HighlightedBackgroundColor, RenderPosition.Y, RenderPosition.X);
            else
                Print();
        }

        public void Clicked(object sender, EventArgs eventArgs)
        {
            if (OnClick == null)
                return;

            OnClick(this, eventArgs);
        }

        public void OnElementUnhighlighted()
        {
            if(ElementUnhighlighted != null)
                ElementUnhighlighted(this, EventArgs.Empty);
        }
        #endregion
    }
}
