using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.UI
{
    /// <summary>
    /// Event args that contain CosoleKey information
    /// </summary>
    internal class KeyEventArgs : EventArgs
    {
        public KeyEventArgs(ConsoleKey keyPressed)
        {
            this.keyPressed = keyPressed;
        }

        public ConsoleKey keyPressed { get; }
    }
}
