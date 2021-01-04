using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Event args that contain CosoleKey information
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        public KeyEventArgs(ConsoleKey keyPressed)
        {
            this.keyPressed = keyPressed;
        }

        public ConsoleKey keyPressed { get; }
    }
}
