using System;
using System.Collections.Generic;
using System.Threading;
using Setnicka.AuxiliaryClasses;
using Setnicka.UI;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Class that runs each individual game (playthrough of each level is an individual game)
    /// </summary>
    class GameManager
    {
        #region Constants
        // General Constants

        // The offset of drawn objects in relation to CursorPosition(0, 0)
        public static readonly Vector2D OFFSET = new Vector2D(2, 1);
        // Delay between individual updates of the game thread
        private const int GAME_UPDATE_FREQUENCY = 400;
        // Delay between main thread updates
        private const int MAIN_THREAD_UPDATE_FREQUENCY = 10;
        // How long (milliseconds) is the chasing ghosts mode active
        private const int CHASING_GHOSTS_FOR = 10000;
        // How long before the chasing ghosts period ends do the ghosts blink (expressed as a part of the whole time)
        private const double CHASING_GHOSTS_BLINKING = 1 / 4;

        // Menu constats

        private const ConsoleColor UNHIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.White;
        private const ConsoleColor UNHIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Black;

        private const ConsoleColor HIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor HIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Red;

        private const ConsoleColor MAIN_LABEL_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor MAIN_LABEL_BACKGROUND_COLOR = ConsoleColor.Yellow;

        private const string EMPTY_LABEL_TEXT = "";

        private const string MAIN_LABEL_TEXT = "Game menu";
        private const string CONTINUE_BUTTON_TEXT = "Continue";
        private const string ESCAPE_BUTTON_TEXT = "Escape";
        #endregion


        #region Constructors
        /// <param name="level">Lever that is the player wants to play, cannot be null</param>
        public GameManager(GameObject[,] level)
        {
            // level assignment
            if (level == null)
                throw new ArgumentNullException("level", "Level cannot be null!");
            this.Level = level;


            // player and ghosts assignment
            Ghosts = new List<Ghost>();
            foreach (GameObject tile in level)
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
            List<ConsoleKey> keysOfInterest = new List<ConsoleKey>() { GameKeyBinding.MoveUp, GameKeyBinding.MoveUpSecondary, GameKeyBinding.MoveDown, GameKeyBinding.MoveDownSecondary, GameKeyBinding.MoveLeft, GameKeyBinding.MoveLeftSecondary, GameKeyBinding.MoveRight, GameKeyBinding.MoveRightSecondary, GameKeyBinding.GoToMenu };
            InputManager = new InputManager(keysOfInterest);
            InputManager.KeyPressed += Player.ChangeHeading;
            InputManager.KeyPressed += GoToMenu;

            // Initialize Menu and MenuManager
            InitializeMenuAndManager();
        }
        #endregion

        #region Fields
        private RunningState currentGameState = RunningState.Off;
        #endregion

        #region Properties
        private GameObject[,] Level { get; set; }
        private Player Player { get; set; }
        private List<Ghost> Ghosts { get; set; }

        private InputManager InputManager { get; set; }

        private Thread InputManagerThread { get; set; }
        private Thread GameRunningThread { get; set; }

        private RunningState CurrentRunningState
        {
            get
            {
                return currentGameState;
            }
            set
            {
                // If player already won, the currentGameState can't be overwritten
                if (currentGameState != RunningState.Win)
                    currentGameState = value;
            }
        }

        private Menu Menu { get; set; }
        private MenuManager MenuManager { get; set; }
        #endregion


        #region Methods for gameplay
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
                    CurrentRunningState = RunningState.ChasingGhosts;
                else if (playerMove == MoveResult.Collision)
                {
                    CurrentRunningState = RunningState.Collision;
                    Thread.Sleep(GAME_UPDATE_FREQUENCY);
                }
            }

            void GhostsMove()
            {
                foreach (Ghost ghost in Ghosts)
                {
                    if (ghost.Move() == MoveResult.Collision)
                    {
                        CurrentRunningState = RunningState.Collision;
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

            CurrentRunningState = RunningState.On;

            void MovePlayer()
            {
                MoveResult playerMove = Player.Move();
                if (playerMove == MoveResult.Boost)
                    timeLeft = CHASING_GHOSTS_FOR;
                else if (playerMove == MoveResult.Collision)
                {
                    CurrentRunningState = RunningState.Collision;
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
                        CurrentRunningState = RunningState.Collision;
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
            CurrentRunningState = RunningState.On;
            RunningState previousGamestate = RunningState.Off;

            while(true)
            {
                if (previousGamestate == CurrentRunningState)
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                else
                {
                    previousGamestate = CurrentRunningState;

                    switch (previousGamestate)
                    {
                        case RunningState.On:
                            Print();

                            AbortThreads(false);
                            StartThreads(InputManager.CheckForInput, Update, false);
                            break;
                        case RunningState.Collision:
                            AbortThreads(true); // TODO: Change to false
                            Console.SetCursorPosition(0, 0);
                            Console.Write("Collision!");
                            Console.ReadKey(true);
                            // TODO: Finish
                            return;
                        case RunningState.ChasingGhosts:
                            AbortThreads(false);
                            StartThreads(InputManager.CheckForInput, UpdateChasingGhosts, false);
                            break;
                        case RunningState.Win:
                            AbortThreads(true);
                            Console.SetCursorPosition(0, 0);
                            Console.Write("Congratulations, you win!");
                            break;
                        case RunningState.Menu:
                            AbortThreads(true);

                            MenuManager.Run();

                            if (CurrentRunningState == RunningState.Finished)
                                return;

                            CurrentRunningState = RunningState.On;
                            break;
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
                gameObject.Print(GameManager.OFFSET);

            PrintBorder();
        }

        /// <summary>
        /// Draws border around the level, so the user knows, where he can walk
        /// </summary>
        private void PrintBorder()
        {
            Vector2D topLeftCorner = (Level[0, 0].Position + OFFSET) + Vector2D.Left + Vector2D.Up;
            Vector2D topRightCorner = (Level[Level.GetLength(0) - 1, 0].Position + OFFSET) + Vector2D.Right + Vector2D.Up;
            Vector2D bottomLeftCorner = (Level[0, Level.GetLength(1) - 1].Position + OFFSET) + Vector2D.Left + Vector2D.Down;
            Vector2D bottomRightCorner = (Level[Level.GetLength(0) - 1, Level.GetLength(1) - 1].Position + OFFSET) + Vector2D.Right + Vector2D.Down;

            for (int x = topLeftCorner.X; x <= topRightCorner.X; x++)
                PrintTile(x, topLeftCorner.Y);
            for (int y = topLeftCorner.Y; y <= bottomLeftCorner.Y; y++)
                PrintTile(topLeftCorner.X, y);
            for (int x = bottomLeftCorner.X; x <= bottomRightCorner.X; x++)
                PrintTile(x, bottomRightCorner.Y);
            for (int y = topRightCorner.Y; y <= bottomRightCorner.Y; y++)
                PrintTile(topRightCorner.X, y);

            // Prints one border tile
            void PrintTile(int x, int y)
            {
                // Validating input
                if (x < 0 || y < 0)
                    return;

                ConsoleColor originalForeground = Console.ForegroundColor;
                ConsoleColor originalBackground = Console.BackgroundColor;

                Console.SetCursorPosition(x, y);

                Console.ForegroundColor = Colors.LevelBorderColor;
                Console.BackgroundColor = Colors.LevelBorderColor;

                Console.Write(" ");

                Console.ForegroundColor = originalForeground;
                Console.BackgroundColor = originalBackground;

                Console.CursorVisible = false;
            }
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
            CurrentRunningState = RunningState.Win;
        }
        #endregion


        #region Methods for UI
        private void InitializeMenuAndManager()
        {
            Menu = new Menu();

            Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Label mainLabel = new Label(MAIN_LABEL_TEXT, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button buttonContinue = new Button(CONTINUE_BUTTON_TEXT, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            buttonContinue.OnClick += Menu.DoExitMenu;

            Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 4, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button buttonEscape = new Button(ESCAPE_BUTTON_TEXT, HorizontalAlignment.Center, 5, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            buttonEscape.OnClick += EscapeGame;
            buttonEscape.OnClick += Menu.DoExitMenu;

            Menu.AddUIElementRange(new List<IUIElement> { emptyLabel1, mainLabel, emptyLabel2, buttonContinue, emptyLabel3, buttonEscape });

            MenuManager = new MenuManager(Menu);
        }

        private void GoToMenu(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.keyPressed != GameKeyBinding.GoToMenu)
                return;

            CurrentRunningState = RunningState.Menu;
        }

        private void EscapeGame(object sender, EventArgs args)
        {
            ConfirmationDialog dialog = new ConfirmationDialog(ConfirmationOptions.YesNo, "Do you really want to end the game?");

            dialog.Run();

            if (dialog.DialogResult == DialogResult.Yes)
                CurrentRunningState = RunningState.Finished;
        }
        #endregion
    }


    public enum RunningState
    {
        Off,
        On,
        ChasingGhosts,
        Collision,
        Menu,
        Win,
        Finished
    }
}
