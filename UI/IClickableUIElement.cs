using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    interface IClickableUIElement : IHighlightableUIElement
    {
        event EventHandler OnClick;

        void Clicked(object sender, EventArgs eventArgs);
    }
}
