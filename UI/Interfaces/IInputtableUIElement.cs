using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// Classes implementing this have the abbility to handle KeyEventArgs input
    /// </summary>
    interface IInputtableUIElement : IHighlightableUIElement
    {
        void HandleInput(object sender, KeyEventArgs input, bool reprintHighlighted);
    }
}
