using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// UI elements that can be highlighted
    /// </summary>
    public interface IHighlightableUIElement : IUIElement
    {
        // This element can be used for validating once the user unhighlights the element...
        event EventHandler ElementUnhighlighted;
        // Method that fires the event
        void OnElementUnhighlighted();

        // Colors when highlighted
        ConsoleColor HighlightedTextColor { get; }
        ConsoleColor HighlightedBackgroundColor { get; }

        // Highlights/Unhighlights the object
        void PrintHighlighted(bool highlighted);
    }
}
