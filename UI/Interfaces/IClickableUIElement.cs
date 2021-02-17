using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// Classes implementing this have the functionality to fire an event when an action happens (they are clicked...)
    /// </summary>
    interface IClickableUIElement : IHighlightableUIElement
    {
        event EventHandler OnClick;

        void Clicked(object sender, EventArgs eventArgs);
    }
}
