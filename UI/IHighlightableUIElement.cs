using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// Menu item that can be highlighted
    /// </summary>
    interface IHighlightableUIElement : IUIElement
    {
        // Colors when highlighted
        ConsoleColor HighlightedTextColor { get; }
        ConsoleColor HighlightedBackgroundColor { get; }

        // Highlights/Unhighlights the object
        void PrintHighlighted(bool highlighted);
    }
}
