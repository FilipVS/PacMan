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
        private const int MAIN_THREAD_UPDATE_FREQUENCY = 10;
        // How long (milliseconds) is the chasing ghosts mode active
        private const int CHASING_GHOSTS_FOR = 10000;


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

            // Subscribe Win event handler to Player's GameWon event
            Player.GameWon += PlayerWon;

            // Initializing the inputManager and subscribing individual event handlers
            List<ConsoleKey> keysOfInterest = new List<ConsoleKey>() { GameKeyBinding.MoveUp, GameKeyBinding.MoveUpSecondary, GameKeyBinding.MoveDown, GameKeyBinding.MoveDownSecondary, GameKeyBinding.MoveLeft, GameKeyBinding.MoveLeftSecondary, GameKeyBinding.MoveRight, GameKeyBinding.MoveRightSecondary };
            InputManager = new InputManager(keysOfInterest);
            InputManager.KeyPressed += Player.ChangeHeading;
            // TODO: subscribe add all the event handlers
        }

        #region Fields
        private GameState currentGameState = GameState.Off;
        #endregion

        #region Properties
        private GameObject[,] Level { get; set; }
        private Player Player { get; set; }
        private List<Ghost> Ghosts { get; set; }

        private InputManager InputManager { get; set; }

        private Thread InputManagerThread { get; set; }
        private Thread GameRunningThread { get; set; }

        private GameState CurrentGameState
        {
            get
            {
                return currentGameState;
            }
            set
            {
                // If player already won, the currentGameState can't be overwritten
                if (currentGameState != GameState.Win)
                    currentGameState = value;
            }
        }
        #endregion


        #region Methods
        /// <summary>
        /// This method is running inside the GameRunningThread and is responsible for the gameplay
        /// </summary>
        private void Update()
        {
            while (true)
            {
                PlayerMove();

                GhostsMove();

                Thread.Sleep(GAME_UPDATE_FREQUENCY);
            }

            void PlayerMove()
            {
                MoveResult playerMove = Player.Move();
                if (playerMove == MoveResult.Boost)
                    CurrentGameState = GameState.ChasingGhosts;
                else if (playerMove == MoveResult.Collision)
                {
                    CurrentGameState = GameState.Collision;
                    Thread.Sleep(GAME_UPDATE_FREQUENCY);
                }
            }

            void GhostsMove()
            {
                foreach (Ghost ghost in Ghosts)
                {
                    if (ghost.Move() == MoveResult.Collision)
                    {
                        CurrentGameState = GameState.Collision;
                        Thread.Sleep(GAME_UPDATE_FREQUENCY);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This is the Update method when player ate boost and is chasing ghosts instead
        /// </summary>
        private void UpdateChasingGhosts()
        {
            // TODO: Make ghosts go slower than player
            // TODO: Make ghosts blink for some time before the mode turns off

            // Thread sleep is skipped when eating a boost, this ensures smoother transition between Update and UpdateChasingGhosts
            Thread.Sleep(GAME_UPDATE_FREQUENCY);

            int timeLeft = CHASING_GHOSTS_FOR;

            // Ghosts move is skipped when eating a boost, so they are the first ones to move here
            MoveGhosts();

            while(timeLeft > 0)
            {
                MovePlayer();

                MoveGhosts();

                timeLeft -= GAME_UPDATE_FREQUENCY;
                Thread.Sleep(GAME_UPDATE_FREQUENCY);
            }

            CurrentGameState = GameState.Normal;

            void MovePlayer()
            {
                MoveResult playerMove = Player.Move();
                if (playerMove == MoveResult.Boost)
                    timeLeft = CHASING_GHOSTS_FOR;
                else if (playerMove == MoveResult.Collision)
                {
                    CurrentGameState = GameState.Collision;
                    Thread.Sleep(GAME_UPDATE_FREQUENCY);
                }
            }

            void MoveGhosts()
            {
                foreach (Ghost ghost in Ghosts)
                {
                    ghost.InvertedMove = true;
                    if (ghost.Move() == MoveResult.Collision)
                    {
                        CurrentGameState = GameState.Collision;
                        Thread.Sleep(GAME_UPDATE_FREQUENCY);
                        break;
                    }
                    ghost.InvertedMove = false;
                }
            }
        }

        /// <summary>
        /// This method stars the game from the outside and controls running of the input thread and game thread
        /// </summary>
        public void Run()
        {
            // Print all the elements
            Print();

            CurrentGameState = GameState.Normal;
            GameState previousGamestate = GameState.Off;

            while(true)
            {
                if (previousGamestate == CurrentGameState)
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                else
                {
                    previousGamestate = CurrentGameState;

                    switch (previousGamestate)
                    {
                        case GameState.Normal:
                            AbortThreads(false);
                            StartThreads(InputManager.CheckForInput, Update, false);
                            break;
                        case GameState.Collision:
                            AbortThreads(false);
                            Console.SetCursorPosition(0, 0);
                            Console.Write("Collision!");
                            break;
                        case GameState.ChasingGhosts:
                            AbortThreads(false);
                            StartThreads(InputManager.CheckForInput, UpdateChasingGhosts, false);
                            break;
                        case GameState.Win:
                            AbortThreads(true);
                            Console.SetCursorPosition(0, 0);
                            Console.Write("Congratulations, you win!");
                            break;
                        // TODO: Finish
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Prints the level and user interface
        /// </summary>
        private void Print()
        {
            Console.Clear();

            foreach (GameObject gameObject in Level)
                gameObject.Print(Game.OFFSET);

            // TODO: Finish (user interface elements...)
        }

        /// <summary>
        /// Sets up the threads to run the game and input manager and starts them
        /// </summary>
        private void StartThreads(ThreadStart inputThreadStart, ThreadStart gameThreadStart, bool startNewInputManager = true)
        {
            if(InputManagerThread == null || startNewInputManager)
            {
                InputManagerThread = new Thread(inputThreadStart);
                InputManagerThread.Start();
            }

            GameRunningThread = new Thread(gameThreadStart);
            GameRunningThread.Start();
        }

        /// <summary>
        /// Aborts the threads that run the game and the input manager
        /// </summary>
        private void AbortThreads(bool abortInputManager = true)
        {
            if (abortInputManager && InputManagerThread != null)
            {
                InputManagerThread.Abort();
                InputManagerThread = null;
            }
            if(GameRunningThread != null)
            {
                GameRunningThread.Abort();
                GameRunningThread = null;
            }
        }

        private void PlayerWon(object sender, EventArgs eventArgs)
        {
            CurrentGameState = GameState.Win;
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
        Win
    }
}
