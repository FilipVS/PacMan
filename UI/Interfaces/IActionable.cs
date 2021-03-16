using System;

namespace Setnicka.UI
{
    /// <summary>
    /// Classes that implement this interface contain some Action that they want to execute in certain condition
    /// </summary>
    public interface IActionable
    {
        Action Action { get; }
    }
}
