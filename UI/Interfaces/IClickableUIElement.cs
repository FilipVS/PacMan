using System;

namespace Setnicka.UI
{
    /// <summary>
    /// Classes implementing this have the functionality to fire an event when an action happens (they are clicked...)
    /// </summary>
    public interface IClickableUIElement : IHighlightableUIElement
    {
        event EventHandler OnClick;

        void Clicked(object sender, EventArgs eventArgs);
    }
}
