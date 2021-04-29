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
        public static readonly Vector2D OFFSET = new Vector2D(2, 2);
        // Delay between individual updates of the game thread
        private const int GAME_UPDATE_FREQUENCY = 400;
        // Delay between main thread updates
        private const int MAIN_THREAD_UPDATE_FREQUENCY = 10;
        // How long (milliseconds) is the chasing ghosts mode active
        private const int CHASING_GHOSTS_FOR = 10000;
        // How long before the chasing ghosts period ends do the ghosts blink (expressed as a part of the whole time)
        private const double CHASING_GHOSTS_BLINKING = 0.25;
        // How often the game thread checks for abort each cycle
        private const int CHECK_FOR_ABORT_TIMES = 3;
        // How many numbers does the game countdown contain and what is the time between individual counts
        private const int COUNTDOWN_START = 3;
        private const int COUNTDOWN_FREQUENCY = 750;
        // Scoring
        private const int SCORE_FOR_COIN = 1;

        private const string PLAYER_WON_MESSAGE = "Congratulations, you won!";
        private const string PLAYER_LOST_MESSAGE = "You are dead, try again!";

        // Menu constats

        private const ConsoleColor UNHIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.White;
        private const ConsoleColor UNHIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Black;

        private const ConsoleColor HIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor HIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Red;

        private const ConsoleColor MAIN_LABEL_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor MAIN_LABEL_BACKGROUND_COLOR = ConsoleColor.Yellow;

        private const string EMPTY_LABEL_TEXT = "";

        private readonly string HOW_TO_GET_TO_MENU_LABEL_TEXT = $"Press {GameKeyBinding.GoToMenu} to get to the menu (pause/escape...)";
        private readonly string HOW_TO_CHANGE_HEADING_TEXT = $"Press {GameKeyBinding.MoveUp}/{GameKeyBinding.MoveDown}/{GameKeyBinding.MoveLeft}/{GameKeyBinding.MoveRight}/arrows to change heading...";

        private const string MAIN_LABEL_TEXT = "Game menu";
        private const string CONTINUE_BUTTON_TEXT = "Continue";
        private const string ESCAPE_BUTTON_TEXT = "Escape";
        #endregion


        #region Constructors
        /// <param name="level">Lever that the user wants to play, cannot be null</param>
        public GameManager(GameObject[,] level)
        {
            // level assignment
            if (level == null)
                throw new ArgumentNullException("level", "Level cannot be null!");
            this.Level = level;

            AvailableCoins = 0;

            // player and ghosts assignment, counting the number of coins
            Ghosts = new List<Ghost>();
            foreach (GameObject tile in level)
            {
                if (tile is Player player)
                    this.Player = player;
                else if (tile is Ghost ghost)
                    Ghosts.Add(ghost);
                if (tile is Empty empty && empty.ContainsCoin)
                    AvailableCoins += SCORE_FOR_COIN;
            }
            // Check that player was found
            if (Player == null)
                throw new ArgumentNullException("level", "Player missing");



            // Initializing the InputManager and subscribing individual event handlers
            List<ConsoleKey> keysOfInterest = new List<ConsoleKey>() { GameKeyBinding.MoveUp, GameKeyBinding.MoveUpSecondary, GameKeyBinding.MoveDown, GameKeyBinding.MoveDownSecondary, GameKeyBinding.MoveLeft, GameKeyBinding.MoveLeftSecondary, GameKeyBinding.MoveRight, GameKeyBinding.MoveRightSecondary, GameKeyBinding.GoToMenu, GameKeyBinding.Refresh };
            InputManager = new InputManager(keysOfInterest);
            InputManager.KeyPressed += Player.ChangeHeading;
            InputManager.KeyPressed += GoToMenu;
            InputManager.KeyPressed += Refresh;

            // Initialize Menu and MenuManager
            InitializeMenuAndManager();

            // Initialize message label
            InitializeLabels();
        }
        #endregion

        #region Fields
        private RunningState currentGameState = RunningState.Off;

        private string message = EMPTY_LABEL_TEXT;

        private string healthMessage = EMPTY_LABEL_TEXT;

        private string scoreMessage = EMPTY_LABEL_TEXT;

        private int score = 0;
        #endregion

        #region Properties

        #region Gameplay related
        private GameObject[,] Level { get; set; }
        private Player Player { get; set; }
        private List<Ghost> Ghosts { get; set; }

        private List<Ghost> EatenGhosts { get; set; } = new List<Ghost>();

        // Number of available coins
        private int AvailableCoins { get; }

        // For counting score
        private int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;

                if (score == AvailableCoins)
                    CurrentRunningState = RunningState.Win;
            }
        }

        // Stores the information about time left for chasing ghosts
        private int BoostTimeLeft { get; set; } = -1;
        #endregion

        #region Controlling the state of the game
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
                if (currentGameState != RunningState.Win && RunningStateOverwritable)
                    currentGameState = value;
            }
        }
        // This is used to prevent overwriting of the CurrentRunningState
        private bool RunningStateOverwritable { get; set; } = true;

        // Used for controlling the state of game thread
        private bool GameThreadRunning { get; set; } = false;

        // Used for aborting the game thread from the inside
        private bool AbortGameThread { get; set; } = false;

        // Show countdown? (works only in chasing ghosts)
        private bool ShowCountdown { get; set; } = true;

        // Is the application switching from UpdateChasingGhosts() to Update() ?
        private bool ReturningFromChasingGhosts { get; set; } = false;
        #endregion

        #region UI
        private Menu Menu { get; set; }
        private MenuManager MenuManager { get; set; }

        // For displaying messages to the user
        private Label MessageLabel { get; set; }
        private string Message
        {
            get
            {
                return message;
            }
            set
            {
                string newMessage = value;

                if (value == null)
                    newMessage = "";

                // Delete previous message
                MessageLabel.Delete();

                // Print new one
                MessageLabel.Text = newMessage;
                MessageLabel.Print();

                message = newMessage;
            }
        }

        // For displaying health to the user
        private Label HealthLabel { get; set; }
        private string HealthMessage
        {
            get
            {
                return healthMessage;
            }
            set
            {
                string newHealth = value;

                if (value == null)
                    newHealth = "";

                // Delete previous health
                HealthLabel.Delete();

                // Print new one
                HealthLabel.Text = newHealth;
                HealthLabel.Print();

                healthMessage = newHealth;
            }
        }

        // For displaying score to the user
        private Label ScoreLabel { get; set; }
        private string ScoreMessage
        {
            get
            {
                return scoreMessage;
            }
            set
            {
                string newScore = value;

                if (value == null)
                    newScore = "";

                // Delete previous health
                ScoreLabel.Delete();

                // Print new one
                ScoreLabel.Text = newScore;
                ScoreLabel.Print();

                scoreMessage = newScore;
            }
        }

        // For informing the player how to get to the menu
        Label HowToGetToMenuLabel { get; set; }

        // For informing the player how to change heading
        Label HowToChangeHeadingMenuLabel { get; set; }
        #endregion

        #endregion


        #region Methods for gameplay

        /// <summary>
        /// This method stars the game from the outside and controls running of the input thread and game thread
        /// </summary>
        public void Run()
        {
            CurrentRunningState = RunningState.On;
            RunningState previousGamestate = RunningState.Off;

            Print();

            while (true)
            {
                if (previousGamestate == CurrentRunningState)
                {
                    RunningStateOverwritable = true;
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                }
                else
                {
                    RunningState previousGameStateCopy = previousGamestate;
                    previousGamestate = CurrentRunningState;

                    RunningStateOverwritable = true;

                    switch (CurrentRunningState)
                    {
                        // Normal game
                        case RunningState.On:
                            AbortThreads(false);
                            StartThreads(InputManager.CheckForInput, Update, false);
                            break;
                        // Dealing with collision
                        case RunningState.Collision:
                            AbortThreads(true);

                            if (previousGameStateCopy == RunningState.ChasingGhosts)
                                CollisionChasingGhosts();
                            else
                                CollisionNormal();

                            // If player lost all lives
                            if (CurrentRunningState == RunningState.Finished)
                            {
                                return;
                            }

                            if (previousGameStateCopy == RunningState.ChasingGhosts)
                                CurrentRunningState = RunningState.ChasingGhosts;
                            else
                                CurrentRunningState = RunningState.On;

                            Print();

                            break;
                        // Chasing ghosts mode
                        case RunningState.ChasingGhosts:
                            AbortThreads(false);
                            StartThreads(InputManager.CheckForInput, UpdateChasingGhosts, false);
                            break;
                        // Player winning
                        case RunningState.Win:
                            AbortThreads(true);

                            PlayerWon();

                            return;
                        // Going to menu
                        case RunningState.Menu:
                            AbortThreads(true);

                            MenuManager.Run();

                            // If player chose to exit the game in the menu
                            if (CurrentRunningState == RunningState.Finished)
                            {
                                return;
                            }

                            Print();

                            if (previousGameStateCopy == RunningState.On)
                                CurrentRunningState = RunningState.On;
                            else if (previousGameStateCopy == RunningState.ChasingGhosts)
                                CurrentRunningState = RunningState.ChasingGhosts;
                            break;
                        // Refresh the screen
                        case RunningState.Refresh:
                            AbortThreads(true);

                            Print();

                            if (previousGameStateCopy == RunningState.On)
                                CurrentRunningState = RunningState.On;
                            else if (previousGameStateCopy == RunningState.ChasingGhosts)
                                CurrentRunningState = RunningState.ChasingGhosts;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// This method is running inside the GameRunningThread and is responsible for the gameplay in normal mode
        /// </summary>
        private void Update()
        {
            #region Setup
            GameThreadRunning = true;

            // This prevents countdown after we've returned from chasing ghosts
            if (!ReturningFromChasingGhosts)
            {
                // Countdown, so the player can get ready and check, if the player wanted to go back in the meantime
                Countdown();
                if (AbortGameThread)
                {
                    AbortGameThread = false;
                    GameThreadRunning = false;
                    return;
                }
            }

            ReturningFromChasingGhosts = false;

            // Re-color the ghosts
            foreach (Ghost ghost in Ghosts) 
            {
                // Reset the drawing style
                ghost.Style = Ghost.DrawingStyle.Normal;

                ghost.Print(OFFSET);
            }
                
            #endregion

            #region Update sequence itself
            while (true)
            {
                PlayerMove();

                if (AbortGameThread || CurrentRunningState != RunningState.On)
                {
                    BeforeReturn();
                    return;
                }

                GhostsMove();

                // Try to return eaten ghosts
                ReturnGhosts();

                // Thread sleeps and pereodically checks if it is supposed to abort
                for (int i = 0; i < CHECK_FOR_ABORT_TIMES; i++)
                {
                    Thread.Sleep(GAME_UPDATE_FREQUENCY / CHECK_FOR_ABORT_TIMES);
                    if (AbortGameThread)
                    {
                        BeforeReturn();
                        return;
                    }


                    if (AbortGameThread)
                    {
                        BeforeReturn();
                        return;
                    }
                }
            }
            #endregion


            void PlayerMove()
            {
                MoveResult playerMove = Player.Move();
                if (playerMove == MoveResult.Boost)
                {
                    CurrentRunningState = RunningState.ChasingGhosts;
                    RunningStateOverwritable = false;
                    // Make sure that it waits for the Run method to evaluate
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY * 2);
                }
                else if (playerMove == MoveResult.Collision)
                {
                    CurrentRunningState = RunningState.Collision;
                    RunningStateOverwritable = false;
                    // Make sure that it waits for the Run method to evaluate
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY * 2);
                }
                else if (playerMove == MoveResult.Coin)
                    IncreaseScore(SCORE_FOR_COIN);
            }

            void GhostsMove()
            {
                foreach (Ghost ghost in Ghosts)
                {
                    ghost.Style = Ghost.DrawingStyle.Normal;

                    if (ghost.Move() == MoveResult.Collision)
                    {
                        CurrentRunningState = RunningState.Collision;
                        // Make sure that it waits for the Run method to evaluate
                        Thread.Sleep(GAME_UPDATE_FREQUENCY * 2);
                        break;
                    }
                }
            }

            void BeforeReturn()
            {
                AbortGameThread = false;
                GameThreadRunning = false;
            }
        }

        /// <summary>
        /// This is the Update method when player ate boost and is chasing ghosts
        /// </summary>
        private void UpdateChasingGhosts()
        {
            // Should ghosts go alternate color
            bool ghostsGoAlternate = false;

            #region Setup
            GameThreadRunning = true;

            int timeLeft;

            // In this case, the previous chasing ghosts cycle ended
            if (BoostTimeLeft < 0)
            {
                timeLeft = CHASING_GHOSTS_FOR;

                // Ghosts move is skipped when eating a boost, so they are the first ones to move here
                MoveGhosts();

                // Thread sleep is skipped when eating a boost, this ensures smoother transition between Update and UpdateChasingGhosts
                Thread.Sleep(GAME_UPDATE_FREQUENCY);
            }
            // If the ShowCountdown is set to prevent the countdown
            else if (!ShowCountdown)
            {
                timeLeft = BoostTimeLeft;
                ShowCountdown = true;
            }
            // In this case, the previous chasing ghosts cycle was paused and is supposed to be restarted (and nothing is preventing the countdown)
            else
            {
                timeLeft = BoostTimeLeft;

                // Countdown, so the player can get ready and check, if the player wanted to go back
                Countdown();
                if (AbortGameThread)
                {
                    BeforeReturn();
                    return;
                }
            }

            // Re-color the ghosts
            foreach (Ghost ghost in Ghosts)
            {
                // Reset the drawing style
                ghost.Style = Ghost.DrawingStyle.ChasingGhosts;

                ghost.Print(OFFSET);
            }
            #endregion

            #region Update sequence itself
            do
            {
                MovePlayer();

                if (AbortGameThread || CurrentRunningState != RunningState.ChasingGhosts)
                {
                    BeforeReturn();
                    return;
                }

                MoveGhosts();

                // Try to return eaten ghosts
                ReturnGhosts();

                timeLeft -= GAME_UPDATE_FREQUENCY;

                // Thread sleeps and pereodically checks if it is supposed to abort
                for (int i = 0; i < CHECK_FOR_ABORT_TIMES; i++)
                {
                    Thread.Sleep(GAME_UPDATE_FREQUENCY / CHECK_FOR_ABORT_TIMES);
                    if (AbortGameThread)
                    {
                        BeforeReturn();
                        return;
                    }
                }
            } while (timeLeft >= 0);

            #endregion

            BeforeReturn();



            void MovePlayer()
            {
                MoveResult playerMove = Player.Move();

                if (playerMove == MoveResult.Boost)
                {
                    // Restart the timer
                    timeLeft = CHASING_GHOSTS_FOR;
                }
                else if (playerMove == MoveResult.Collision)
                {
                    // Prevent countdown after restarting the game thread
                    ShowCountdown = false;

                    CurrentRunningState = RunningState.Collision;
                    RunningStateOverwritable = false;
                    // Make sure, that it waits for the Run method to evaluate
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY * 2);
                }
                else if (playerMove == MoveResult.Coin)
                    IncreaseScore(SCORE_FOR_COIN);
            }

            void MoveGhosts()
            {
                // Check if they are supposed to blink
                if (timeLeft < (CHASING_GHOSTS_FOR * CHASING_GHOSTS_BLINKING))
                {
                    // If they are currently in their main version, go alternate, else go main
                    if (ghostsGoAlternate)
                        ghostsGoAlternate = false;
                    else
                        ghostsGoAlternate = true;
                }
                // Else make ghosts use the standard color
                else
                    ghostsGoAlternate = false;

                foreach (Ghost ghost in Ghosts)
                {
                    // Set up the colors
                    if (ghostsGoAlternate)
                        ghost.Style = Ghost.DrawingStyle.ChasingGhostsAlternate;
                    else
                        ghost.Style = Ghost.DrawingStyle.ChasingGhosts;

                    ghost.InvertedMove = true;
                    ghost.Move();
                    ghost.InvertedMove = false;

                    // Reprint the ghost to maintain alternating colors in case he didn't move
                    ghost.Print(OFFSET);
                }
            }

            void BeforeReturn()
            {
                AbortGameThread = false;
                GameThreadRunning = false;
                BoostTimeLeft = timeLeft;
                if(timeLeft < 0)
                    CurrentRunningState = RunningState.On;
                ReturningFromChasingGhosts = true;
            }
        }

        /// <summary>
        /// Is used when ghost collides with player in normal mode
        /// </summary>
        private void CollisionNormal()
        {
            if (Player.Health == 1)
            {
                PLayerLost();
                return;
            }

            // Reset the tiles that player and ghosts are standing on
            Level[Player.Position.X, Player.Position.Y] = new Empty(Level, Player.Position.Copy());
            foreach (Ghost ghost in Ghosts)
            {
                if (ghost.TileStanding != null)
                    Level[ghost.Position.X, ghost.Position.Y] = ghost.TileStanding;
                else
                    Level[ghost.Position.X, ghost.Position.Y] = new Empty(Level, ghost.Position.Copy());
            }

            // Create new player and ghosts with correct stats
            Player = new Player(Level, Player.SpawnPoint, (Player.Health - 1));
            HealthMessage = $"Health: {Player.Health}";
            InputManager.KeyPressed += Player.ChangeHeading;

            for (int i = 0; i < Ghosts.Count; i++)
                Ghosts[i] = RespawnGhost(Ghosts[i]);


            // Put the newely created objects into level
            Level[Player.Position.X, Player.Position.Y] = Player;
            foreach (Ghost ghost in Ghosts)
            {
                Level[ghost.Position.X, ghost.Position.Y] = ghost;
            }
        }

        /// <summary>
        /// Is used when player eats ghost in ghost-chasing mode
        /// </summary>
        private void CollisionChasingGhosts()
        {
            // Check if player really collided with ghost
            if (!(Level[Player.DesiredTile.X, Player.DesiredTile.Y] is Ghost))
            {
                BeforeReturn();
                return;
            }

            // Remove the ghost from the Level and add him to eaten ghosts
            for(int i = 0; i < Ghosts.Count; i++)
            {
                if(Ghosts[i].Position.Equals(Player.DesiredTile))
                {
                    // Try to return the ghosts's TileStanding, or fill the empty position with new tile
                    if (Ghosts[i].TileStanding != null)
                        Level[Ghosts[i].Position.X, Ghosts[i].Position.Y] = Ghosts[i].TileStanding;
                    else
                        Level[Player.DesiredTile.X, Player.DesiredTile.Y] = new Empty(Level, Player.DesiredTile.Copy());

                    // Remove the ghost from the list and add him to eaten ghosts
                    EatenGhosts.Add(Ghosts[i]);
                    Ghosts.RemoveAt(i);

                    // Reprint the tile that the ghost stood on
                    Level[Player.DesiredTile.X, Player.DesiredTile.Y].Print(OFFSET);
                    break;
                }
            }

            BeforeReturn();

            void BeforeReturn()
            {
                // Start running the game again
                CurrentRunningState = RunningState.ChasingGhosts;
            }
        }

        /// <summary>
        /// Used for returning ghosts that the player ate
        /// </summary>
        private void ReturnGhosts()
        {
            // Will be indexes of ghsots that were returned
            List<int> removeAtIndex = new List<int>();

            // No ghosts to return
            if (EatenGhosts.Count < 1)
                return;

            for (int i = 0; i < EatenGhosts.Count; i++)
            {
                // If the ghost's spawn point is not empty, continue
                if (!(Level[EatenGhosts[i].SpawnPoint.X, EatenGhosts[i].SpawnPoint.Y] is Empty))
                    continue;
                // Or if player is standing right next to the tile
                if (PlayerAround(EatenGhosts[i].SpawnPoint))
                    continue;

                // Else create the ghost and set up its information
                Ghosts.Add(RespawnGhost(EatenGhosts[i]));

                // Put the ghost on the level
                Level[EatenGhosts[i].SpawnPoint.X, EatenGhosts[i].SpawnPoint.Y] = Ghosts[Ghosts.Count - 1];

                // Set the style information to match
                Ghosts[Ghosts.Count - 1].Style = EatenGhosts[i].Style;

                // Print the new ghost
                Level[EatenGhosts[i].SpawnPoint.X, EatenGhosts[i].SpawnPoint.Y].Print(OFFSET);

                // Set the ghosts's index to be removed from EatenGhosts
                removeAtIndex.Add(i);
            }

            // Remove the placed ghosts
            foreach (int num in removeAtIndex)
                EatenGhosts.RemoveAt(num);

            // Is player on some tile next to this position?
            bool PlayerAround(Vector2D position)
            {
                if ((position - Player.Position).Magnitude < 2)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Used to re-create a ghost at its spawn point
        /// </summary>
        private Ghost RespawnGhost(Ghost oldGhost)
        {
            Type typeOFGhost = oldGhost.GetType();
            Ghost newGhost;

            if (typeOFGhost.FullName == typeof(Blinky).ToString())
                newGhost = new Blinky(Level, oldGhost.SpawnPoint.Copy(), Player.Position.Copy());
            else if (typeOFGhost.FullName == typeof(Clyde).ToString())
                newGhost = new Clyde(Level, oldGhost.SpawnPoint.Copy(), Player.Position.Copy());
            else if (typeOFGhost.FullName == typeof(Inky).ToString())
                newGhost = new Inky(Level, oldGhost.SpawnPoint.Copy(), Player.Position.Copy());
            else if (typeOFGhost.FullName == typeof(Pinky).ToString())
                newGhost = new Pinky(Level, oldGhost.SpawnPoint.Copy(), Player.Position.Copy());
            else
                throw new ArgumentException("Unknown ghost");

            return newGhost;
        }

        /// <summary>
        /// Prints the level and user interface
        /// </summary>
        private void Print()
        {
            Console.Clear();

            foreach (GameObject gameObject in Level)
                gameObject.Print(OFFSET);

            PrintBorder();

            HealthLabel.Print();
            ScoreLabel.Print();

            HowToGetToMenuLabel.Print();
            HowToChangeHeadingMenuLabel.Print();

            Console.CursorVisible = false;
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

            // Save the original ConsleColor setting, so we can reset it at the end
            ConsoleColor originalForeground = Console.ForegroundColor;
            ConsoleColor originalBackground = Console.BackgroundColor;

            // Set the proper color to draw with
            Console.ForegroundColor = GameColors.LevelBorderColor;
            Console.BackgroundColor = GameColors.LevelBorderColor;

            for (int x = topLeftCorner.X; x <= topRightCorner.X; x++)
                PrintTile(x, topLeftCorner.Y);
            for (int y = topLeftCorner.Y; y <= bottomLeftCorner.Y; y++)
                PrintTile(topLeftCorner.X, y);
            for (int x = bottomLeftCorner.X; x <= bottomRightCorner.X; x++)
                PrintTile(x, bottomRightCorner.Y);
            for (int y = topRightCorner.Y; y <= bottomRightCorner.Y; y++)
                PrintTile(topRightCorner.X, y);

            // Reset the ConsoleColor setting to its original version
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;


            void PrintTile(int x, int y)
            {
                // Validating input
                if (x < 0 || y < 0)
                    return;

                Console.SetCursorPosition(x, y);

                Console.Write(" ");

                Console.CursorVisible = false;
            }
        }

        /// <summary>
        /// Counts down before the game, so the player can get ready
        /// </summary>
        private void Countdown()
        {
            for (int i = COUNTDOWN_START; i > 0; i--)
            {
                // This is always run from game thread, so it checks whether the game thread is supposed to abort (not to delay anything)
                if (AbortGameThread)
                    return;

                // Set the message box to display proper number
                Message = $"{i}";
                Thread.Sleep(COUNTDOWN_FREQUENCY);
            }

            // make the message box not display anything
            Message = EMPTY_LABEL_TEXT;
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
                InputManager.AbortManager = true;

                InputManagerThread.Join();

                InputManagerThread = null;
            }
            if(GameRunningThread != null)
            {
                if (GameThreadRunning)
                    AbortGameThread = true;

                GameRunningThread.Join();

                GameRunningThread = null;
            }
        }

        private void PlayerWon()
        {
            MessageDialog messageDialog = new MessageDialog(PLAYER_WON_MESSAGE);

            messageDialog.Run();
        }

        private void PLayerLost()
        {
            MessageDialog messageDialog = new MessageDialog(PLAYER_LOST_MESSAGE);

            messageDialog.Run();

            CurrentRunningState = RunningState.Finished;
        }

        private void IncreaseScore(int increaseBy)
        {
            Score += increaseBy;

            ScoreMessage = $"Score: {Score} / {AvailableCoins}";
        }

        /// <summary>
        /// This allows the player to refresh the visuals if the game glitches
        /// </summary>
        private void Refresh(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.keyPressed != GameKeyBinding.Refresh)
                return;

            CurrentRunningState = RunningState.Refresh;
        }

        #endregion


        #region Methods for UI
        /// <summary>
        /// Initializes MainMenu and its Manager (assigns handlers to all the buttons etc.)
        /// </summary>
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

        /// <summary>
        /// Initializes Message, Health and Score Label
        /// </summary>
        private void InitializeLabels()
        {
            // Initialize message, health and score labels
            Vector2D messageLabelPosition = (Level[Level.GetLength(0) - 1, 0].Position + OFFSET) + Vector2D.Right * 3;
            Vector2D healthLabelPosition = messageLabelPosition + Vector2D.Down * 2;
            Vector2D scoreLabelPosition = healthLabelPosition + Vector2D.Down * 2;

            MessageLabel = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Custom, messageLabelPosition, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            HealthLabel = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Custom, healthLabelPosition, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            ScoreLabel = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Custom, scoreLabelPosition, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            HealthMessage = $"Health: {Player.Health}";
            ScoreMessage = $"Score: {Score} / {AvailableCoins}";


            // Initialize HowToGetToMenuLabel
            HowToGetToMenuLabel = new Label(HOW_TO_GET_TO_MENU_LABEL_TEXT, HorizontalAlignment.Right, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            // Initialize HowToChangeHadingMenuLabel
            HowToChangeHeadingMenuLabel = new Label(HOW_TO_CHANGE_HEADING_TEXT, HorizontalAlignment.Right, 1, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
        }

        /// <summary>
        /// Event handle that allows user to go to the menu, is subscribed to the InputManager
        /// </summary>
        private void GoToMenu(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.keyPressed != GameKeyBinding.GoToMenu)
                return;

            CurrentRunningState = RunningState.Menu;
        }

        /// <summary>
        /// Event handler that allows player to escape the game, is subscribed to button in the menu
        /// </summary>
        private void EscapeGame(object sender, EventArgs args)
        {
            ConfirmationDialog dialog = new ConfirmationDialog(ConfirmationOptions.YesNo, "Do you really want to end the game?");

            dialog.Run();

            if (dialog.DialogResult == DialogResult.Yes)
                CurrentRunningState = RunningState.Finished;
        }
        #endregion

        private enum RunningState
        {
            Off,
            On,
            ChasingGhosts,
            Collision,
            Menu,
            Win,
            Refresh,
            Finished
        }
    }
}
