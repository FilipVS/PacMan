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
        // The offset of drawn objects in relation to CursorPosition(0, 0)
        public static readonly Vector2D OFFSET = new Vector2D(2, 1);
        // Delay between individual updates of the game thread
        private const int GAME_UPDATE_FREQUENCY = 400;
        // Delay between main thread updates
        private const int MAIN_THREAD_UPDATE_FREQUENCY = 20;


        /// <param name="level">Lever that is the player wants to play, cannot be null</param>
        public Game(GameObject[,] level)
        {
            // level assignment
            if (level == null)
                throw new ArgumentNullException("level", "Level cannot be null!");
            this.Level = level;


            // player and ghosts assignment
            Ghosts = new List<Ghost>();
            foreach(GameObject tile in level)
            {
                if (tile is Player player)
                    this.Player = player;
                else if (tile is Ghost ghost)
                    Ghosts.Add(ghost);
            }
            // Check that player was found
            if (Player == null)
                throw new ArgumentNullException("level", "Player missing");


            // Initializing the inputManager and subscribing individual event handlers
            List<ConsoleKey> keysOfInterest = new List<ConsoleKey>() { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D };
            InputManager = new InputManager(keysOfInterest);
            InputManager.KeyPressed += Player.ChangeHeading;
            // TODO: subscribe add all the event handlers
        }


        #region Automatic properties
        private GameObject[,] Level { get; set; }
        private Player Player { get; set; }
        private List<Ghost> Ghosts { get; set; }

        private InputManager InputManager { get; set; }

        private Thread InputManagerThread { get; set; }
        private Thread GameRunningThread { get; set; }

        private GameState CurrentGameState { get; set; } = GameState.Off;
        #endregion


        #region Methods
        /// <summary>
        /// This method is running inside the GameRunningThread and is responsible for the gameplay
        /// </summary>
        private void Update()
        {
            while (true)
            {
                MoveResult playerMove = Player.Move();
                if (playerMove == MoveResult.Boost)
                    CurrentGameState = GameState.ChasingGhosts; // TODO: When eating another boost, the timer should reset
                else if (playerMove == MoveResult.Collision)
                    CurrentGameState = GameState.Collision;

                foreach (Ghost ghost in Ghosts)
                {
                    if(ghost.Move() == MoveResult.Collision)
                    {
                        CurrentGameState = GameState.Collision;
                        break;
                    }
                }

                Thread.Sleep(GAME_UPDATE_FREQUENCY);
            }
        }

        /// <summary>
        /// This method stars the game from the outside and controls running of the input thread and game thread
        /// </summary>
        public void Start()
        {
            // Print all the elements
            Print();

            // Start the threads
            StartThreads(InputManager.CheckForInput, Update);

            CurrentGameState = GameState.Normal;
            GameState previousGamestate = CurrentGameState;

            InputManagerThread.Start();
            GameRunningThread.Start();

            do
            {
                if (previousGamestate == CurrentGameState)
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                else
                {
                    previousGamestate = CurrentGameState;

                    switch (previousGamestate)
                    {
                        case GameState.Collision:
                            AbortThreads();
                            Console.SetCursorPosition(0, 0);
                            Console.Write("Collision!");
                            break;
                        // TODO: Finish
                        default:
                            break;
                    }
                }
            } while (previousGamestate != GameState.Finished);
        }

        /// <summary>
        /// Prints the level and user interface
        /// </summary>
        private void Print()
        {
            foreach (GameObject gameObject in Level)
                gameObject.Print();

            // TODO: Finish (user interface elements...)
        }

        /// <summary>
        /// Sets up the threads to run the game and input manager and starts them
        /// </summary>
        private void StartThreads(ThreadStart inputThreadStart, ThreadStart gameThreadStart)
        {
            InputManagerThread = new Thread(inputThreadStart);
            GameRunningThread = new Thread(gameThreadStart);
        }

        /// <summary>
        /// Aborts the threads that run the game and the input manager
        /// </summary>
        private void AbortThreads()
        {
            // TODO: Figure out how to make it work
            
            InputManagerThread.Abort();
            GameRunningThread.Abort();
        }
        #endregion
    }

    
    public enum GameState
    {
        Off,
        Normal,
        ChasingGhosts,
        Collision,
        Pause,
        Refresh,
        Finished
    }
}
