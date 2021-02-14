using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.UI
{
    /// <summary>
    /// Triggers and event when certain keys are pressed
    /// </summary>
    internal class InputManager
    {
        /// <param name="keysOfInterest">Keys that the input manager should react to</param>
        public InputManager(List<ConsoleKey> keysOfInterest)
        {
            // Remove duplicate keys
            for(int i = 0; i < keysOfInterest.Count; i++)
            {
                for(int j = 0; j < keysOfInterest.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (keysOfInterest[i] == keysOfInterest[j])
                        keysOfInterest.RemoveAt(j);
                }
            }

            this.keysOfInterest = keysOfInterest;
        }

        public readonly List<ConsoleKey> keysOfInterest;

        public delegate void KeyPressedEventHandler(object sender, KeyEventArgs eventArgs);
        /// <summary>
        /// Event that is triggered when a key of interest is pressed
        /// </summary>
        public event KeyPressedEventHandler KeyPressed;

        public void CheckForInput()
        {
            ConsoleKeyInfo keyInfo;

            while (true)
            {
                do
                {
                    keyInfo = Console.ReadKey(true);

                    if (KeyOfInterest(keyInfo.Key))
                        KeyPressed(this, new KeyEventArgs(keyInfo.Key));
                } while (Console.KeyAvailable);
            }
        }

        /// <summary>
        /// Chekcs whether the key is of interest (only some keys are further proccessed)
        /// </summary>
        private bool KeyOfInterest(ConsoleKey key)
        {
            foreach(ConsoleKey consoleKey in keysOfInterest)
            {
                if (consoleKey == key)
                    return true;
            }
            return false;
        }
    }
}
