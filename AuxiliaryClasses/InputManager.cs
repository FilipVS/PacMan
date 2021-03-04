using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.AuxiliaryClasses
{
    /// <summary>
    /// Triggers and event when certain keys are pressed
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
        /// <param name="keysOfInterest">Keys that the input manager should react to</param>
        public InputManager(IEnumerable<ConsoleKey> keysOfInterest)
        {
            List<ConsoleKey> keysOfInterestList = new List<ConsoleKey>();
            foreach (ConsoleKey key in keysOfInterest)
                keysOfInterestList.Add(key);

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

            this.KeysOfInterest = keysOfInterestList;
            ListenForAllInput = false;
        }
        #endregion

        public delegate void KeyPressedEventHandler(object sender, KeyEventArgs eventArgs);
        /// <summary>
        /// Event that is triggered when a key of interest is pressed
        /// </summary>
        public event KeyPressedEventHandler KeyPressed;

        #region Properties
        private List<ConsoleKey> KeysOfInterest { get; }

        private bool ListenForAllInput { get; }
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
            }
        }

        /// <summary>
        /// Chekcs whether the key is of interest (only some keys are further proccessed)
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
