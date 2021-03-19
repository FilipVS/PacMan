using System;
using System.Collections.Generic;

namespace Setnicka.AuxiliaryClasses
{
    /// <summary>
    /// Allows to run a method that will be listening for input and will trigger ann event when certain keys are pressed
    /// </summary>
    public class InputManager
    {
        #region Constructors
        /// <summary>
        /// Initializes enw instance of InputManager that will listen for all input
        /// </summary>
        public InputManager()
        {
            ListenForAllInput = true;
        }

        /// <summary>
        /// Initializes enw instance of InputManager that will listen for specified input
        /// </summary>
        /// <param name="keysOfInterest">Keys that the InputManager should react to</param>
        public InputManager(IEnumerable<ConsoleKey> keysOfInterest)
        {
            // Turn the IEnumerable into a list
            List<ConsoleKey> keysOfInterestList = new List<ConsoleKey>();
            foreach (ConsoleKey key in keysOfInterest)
                keysOfInterestList.Add(key);

            // If there are no keys, the InputManager will just listen for all input
            if(keysOfInterestList.Count < 1)
            {
                ListenForAllInput = true;
                return;
            }

            // Remove duplicate keys
            for (int i = 0; i < keysOfInterestList.Count; i++)
            {
                for (int j = 0; j < keysOfInterestList.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (keysOfInterestList[i] == keysOfInterestList[j])
                        keysOfInterestList.RemoveAt(j);
                }
            }

            // Set the properties
            this.KeysOfInterest = keysOfInterestList;
            ListenForAllInput = false;
        }
        #endregion

        /// <summary>
        /// Event that is triggered when a key of interest is pressed
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyPressed;

        #region Properties
        private List<ConsoleKey> KeysOfInterest { get; }

        private bool ListenForAllInput { get; }

        // Used for turning the manager off
        public bool AbortManager { get; set; } = false;
        #endregion

        public void CheckForInput()
        {
            ConsoleKeyInfo keyInfo;

            while (true)
            {
                do
                 {
                    if (Console.KeyAvailable)
                        keyInfo = Console.ReadKey(true);
                    else
                        continue;

                    if (KeyOfInterest(keyInfo.Key) && KeyPressed != null)
                         KeyPressed(this, new KeyEventArgs(keyInfo.Key));
                 } while (Console.KeyAvailable);

                // If the manager should exit
                if (AbortManager)
                {
                    AbortManager = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Chekcs whether the key is of interest (only some keys are further processed)
        /// </summary>
        private bool KeyOfInterest(ConsoleKey key)
        {
            if (ListenForAllInput)
                return true;

            foreach (ConsoleKey consoleKey in KeysOfInterest)
            {
                if (consoleKey == key)
                    return true;
            }
            return false;
        }
    }
}
