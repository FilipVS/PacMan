using System;
using System.Collections.Generic;
using System.Threading;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Class that runs each individual game (playthrough of each level is an individual game)
    /// </summary>
    class Game
    {
        private const int UPDATE_FREQUENCY = 100;

        public Game(GameObject[] level)
        {
            // level assignment
            if (level == null)
                throw new ArgumentNullException("level", "Level can not be null!");
            this.level = level;

            // Initializing the inputManager and subscribing individual event handlers
            List<ConsoleKey> keysOfInterest = new List<ConsoleKey>() { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Escape };
            inputManager = new InputManager(keysOfInterest);
            // TODO: subscribe add all the event handlers

            // Initializing the Threads that will run the inputManager and main game method
            inputManagerThread = new Thread(inputManager.CheckForInput);
            gameRunningThread = new Thread(Update);
        }

        #region Fields
        private GameObject[] level;

        private InputManager inputManager;

        private Thread inputManagerThread;
        private Thread gameRunningThread;
        #endregion

        #region Methods
        private void Update()
        {
            // TODO: Finish
        }
        #endregion
    }
}
