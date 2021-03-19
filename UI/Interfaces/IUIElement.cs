using System;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.UI
{
    /// <summary>
    /// A class has to implement this interface in order to be usable as a part of the UI
    /// </summary>
    public interface IUIElement
    {
        /// Where will be the UIElement rendered on the screen
        HorizontalAlignment HorizontalAlignment { get; set; }

        // Is used when HorizontalAlignment is set to Custom
        Vector2D RenderPosition { get; set; }

        // What is written on the element
        string Text { get; }

        // Colors that are used when rendering the element
        ConsoleColor TextColor { get; }
        ConsoleColor BackgroundColor { get; }

        void Print();
    }

    /// <summary>
    /// Defines, where will be the UIElement rendered
    /// </summary>
    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center,
        Custom
    }
}