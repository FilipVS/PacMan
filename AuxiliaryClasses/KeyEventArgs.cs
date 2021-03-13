using System;

namespace Setnicka.AuxiliaryClasses
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
