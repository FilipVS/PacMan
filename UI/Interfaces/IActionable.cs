using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.UI
{
    /// <summary>
    /// Classes that implement this interface contain some Action that they want to execute in certain condition
    /// </summary>
    interface IActionable
    {
        Action Action { get; }
    }
}
