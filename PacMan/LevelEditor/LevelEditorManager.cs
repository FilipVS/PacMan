﻿using System;
using System.Collections.Generic;
using System.Threading;
using Setnicka.AuxiliaryClasses;
using Setnicka.UI;
using System.IO;

namespace Setnicka.PacMan.LevelEditor
{
    /// <summary>
    /// This class is used for creation of levels (everything connected to it - checking for input through InputManager, validating input)
    /// </summary>
    internal class LevelEditorManager
    {
        #region Constants
        // General constants

        // Minimal size of a level
        public static readonly Vector2D MINIMUM_LEVEL_SIZE = new Vector2D(2, 2);
        // Maximal size of a level
        public static readonly Vector2D MAXIMUM_LEVEL_SIZE = new Vector2D(100, 25);
        // Delay between main thread updates
        private const int MAIN_THREAD_UPDATE_FREQUENCY = 20;
        // The offset of drawn level objects in relation to CursorPosition(0, 0)
        private static readonly Vector2D OFFSET = new Vector2D(2, 2);
        // The offset of objects for choice
        private readonly Vector2D OFFSET_OBJECTS_FOR_CHOICE;


        // Menu constants

        private const ConsoleColor UNHIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.White;
        private const ConsoleColor UNHIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Black;

        private const ConsoleColor HIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor HIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Red;

        private const ConsoleColor MAIN_LABEL_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor MAIN_LABEL_BACKGROUND_COLOR = ConsoleColor.Yellow;

        private const string EMPTY_LABEL_TEXT = "";

        private const string MAIN_LABEL_TEXT = "Level Editor Menu";
        private const string SAVE_BUTTON_TEXT = "Save level";
        private const string ESCAPE_BUTTON_TEXT = "Escape";
        #endregion


        public LevelEditorManager(Vector2D levelSize)
        {
            // Check for valid levelSize and initialize LevelArray
            if (levelSize.X < 1 || levelSize.Y < 1 || levelSize.X > MAXIMUM_LEVEL_SIZE.X || levelSize.Y > MAXIMUM_LEVEL_SIZE.Y)
                throw new ArgumentException("levelSize is not proper number!");
            LevelArray = new GameObject[levelSize.X, levelSize.Y];
            // Create new Empty GameObject on each tile of the LevelArray
            for(int x = 0; x < LevelArray.GetLength(0); x++)
            {
                for(int y = 0; y < LevelArray.GetLength(1); y++)
                {
                    LevelArray[x, y] = new Empty(LevelArray, new Vector2D(x, y));
                }
            }

            // Initialize InputManager
            List<ConsoleKey> keysOfInteresst = new List<ConsoleKey>() { LevelEditorKeyBinding.LevelPlaneDown, LevelEditorKeyBinding.LevelPlaneUp, LevelEditorKeyBinding.LevelPlaneRight, LevelEditorKeyBinding.LevelPlaneLeft, LevelEditorKeyBinding.ObjectOfChoiceUp, LevelEditorKeyBinding.ObjectOfChoiceDown, LevelEditorKeyBinding.PlaceObject, LevelEditorKeyBinding.DeleteObject, LevelEditorKeyBinding.GoToMenu };
            InputManager = new InputManager(keysOfInteresst);
            InputManager.KeyPressed += ChangeHighlighted;
            InputManager.KeyPressed += ChangeHighlightedObjectOfChoice;
            InputManager.KeyPressed += PlaceGameObject;
            InputManager.KeyPressed += DeleteGameObject;
            InputManager.KeyPressed += GoToMenu;

            InitializeObjectsForChoice();

            // Initiallizen offset of objects for choice - it needs to be next to the edited level
            OFFSET_OBJECTS_FOR_CHOICE = Vector2D.Right * levelSize.X + OFFSET + 2*Vector2D.Right;

            // Player always has to be present on the level
            LevelArray[0, 0] = new Player(LevelArray, new Vector2D(0, 0));

            void InitializeObjectsForChoice()
            {
                ObjectsOfChoice = new GameObject[1, 8];
                ObjectsOfChoice[0, 0] = new Empty(ObjectsOfChoice, new Vector2D(0, 0), true);
                ObjectsOfChoice[0, 1] = new Empty(ObjectsOfChoice, new Vector2D(0, 1), false, true);
                ObjectsOfChoice[0, 2] = new Wall(ObjectsOfChoice, new Vector2D(0, 2));
                ObjectsOfChoice[0, 3] = new Player(ObjectsOfChoice, new Vector2D(0, 3));
                ObjectsOfChoice[0, 4] = new Blinky(ObjectsOfChoice, new Vector2D(0, 4), new Vector2D(0, 3));
                ObjectsOfChoice[0, 5] = new Pinky(ObjectsOfChoice, new Vector2D(0, 5), new Vector2D(0, 3));
                ObjectsOfChoice[0, 6] = new Inky(ObjectsOfChoice, new Vector2D(0, 6), new Vector2D(0, 3));
                ObjectsOfChoice[0, 7] = new Clyde(ObjectsOfChoice, new Vector2D(0, 7), new Vector2D(0, 3));
            }

            InitializeMenuAndManager();
        }


        #region Properties
        InputManager InputManager { get; set; }

        Thread InputManagerThread { get; set; }

        GameObject[,] LevelArray { get; set; }

        Vector2D HighlightedPosition = new Vector2D(0, 1);

        Vector2D HighlightedObjectOfChoice = new Vector2D(0, 1);

        RunningState CurrentRunningState { get; set; }

        // Player chooses which GameObject he wants to place with these
        GameObject[,] ObjectsOfChoice;

        Menu Menu;
        MenuManager MenuManager;
        #endregion

        #region Methods
        /// <summary>
        /// Main method that controls the creation of a level
        /// </summary>
        public void Run()
        {
            // Print all the elements
            Print();

            // Highlight first position
            ChangeHighlighted(this, new KeyEventArgs(LevelEditorKeyBinding.LevelPlaneUp));

            // Highlight firs key of interest
            ChangeHighlightedObjectOfChoice(this, new KeyEventArgs(LevelEditorKeyBinding.ObjectOfChoiceUp));

            CurrentRunningState = RunningState.On;
            RunningState previousGamestate = RunningState.Off;

            do
            {
                if (previousGamestate == CurrentRunningState)
                    Thread.Sleep(MAIN_THREAD_UPDATE_FREQUENCY);
                else
                {
                    previousGamestate = CurrentRunningState;

                    switch (CurrentRunningState)
                    {
                        case RunningState.On:
                            Print();
                            AbortThread();
                            StartThread(InputManager.CheckForInput);
                            break;
                        case RunningState.Finished:
                            AbortThread();
                            break;
                        case RunningState.Menu:
                            AbortThread();

                            MenuManager.Run();

                            if (CurrentRunningState == RunningState.Finished)
                                return;

                            CurrentRunningState = RunningState.On;
                            break;
                        default:
                            break;
                    }
                }
            } while (CurrentRunningState != RunningState.Finished);
        }

        private void AbortThread()
        {
            if (InputManagerThread == null)
                return;

            InputManagerThread.Abort();
            InputManagerThread = null;
        }

        private void StartThread(ThreadStart inputThreadStart)
        {
            InputManagerThread = new Thread(inputThreadStart);
            InputManagerThread.Start();
        }

        /// <summary>
        /// Prints the level and user interface
        /// </summary>
        private void Print()
        {
            Console.Clear();

            foreach (GameObject gameObject in LevelArray)
                gameObject.Print(OFFSET);

            foreach (GameObject gameObject1 in ObjectsOfChoice)
                gameObject1.Print(OFFSET_OBJECTS_FOR_CHOICE);

            PrintBorder();

            InitialHighlight();
        }

        /// <summary>
        /// Draws border around the level, so the user knows, where he can create
        /// </summary>
        private void PrintBorder()
        {
            Vector2D topLeftCorner = (LevelArray[0, 0].Position + OFFSET) + Vector2D.Left + Vector2D.Up;
            Vector2D topRightCorner = (LevelArray[LevelArray.GetLength(0) - 1, 0].Position + OFFSET) + Vector2D.Right + Vector2D.Up;
            Vector2D bottomLeftCorner = (LevelArray[0, LevelArray.GetLength(1) - 1].Position + OFFSET) + Vector2D.Left + Vector2D.Down;
            Vector2D bottomRightCorner = (LevelArray[LevelArray.GetLength(0) - 1, LevelArray.GetLength(1) - 1].Position + OFFSET) + Vector2D.Right + Vector2D.Down;

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

                Console.ForegroundColor = LevelEditorColors.LevelBorderColor;
                Console.BackgroundColor = LevelEditorColors.LevelBorderColor;

                Console.Write(" ");

                Console.ForegroundColor = originalForeground;
                Console.BackgroundColor = originalBackground;

                Console.CursorVisible = false;
            }
        }

        /// <summary>
        /// Hihglights the initial objects at the beginning
        /// </summary>
        private void InitialHighlight()
        {
            ConsoleColor[] unhighlightedColors = new ConsoleColor[] { Colors.EmptyColor, Colors.WallColor }; // The original Pacman.Colors setting

            SetColorsHighlight(true, unhighlightedColors);
            LevelArray[HighlightedPosition.X, HighlightedPosition.Y].Print(OFFSET);
            ObjectsOfChoice[HighlightedObjectOfChoice.X, HighlightedObjectOfChoice.Y].Print(OFFSET_OBJECTS_FOR_CHOICE);
            SetColorsHighlight(false, unhighlightedColors);
        }

        /// <summary>
        /// Moves cursor according to player's input
        /// </summary>
        private void ChangeHighlighted(object sender, KeyEventArgs keyArgs)
        {
            Vector2D highlightChange = null;

            switch (keyArgs.keyPressed)
            {
                case LevelEditorKeyBinding.LevelPlaneUp:
                    if (HighlightedPosition.Y > 0)
                        highlightChange = Vector2D.Up;
                    break;
                case LevelEditorKeyBinding.LevelPlaneDown:
                    if (HighlightedPosition.Y < (LevelArray.GetLength(1) - 1))
                        highlightChange = Vector2D.Down;
                    break;
                case LevelEditorKeyBinding.LevelPlaneLeft:
                    if (HighlightedPosition.X > 0)
                        highlightChange = Vector2D.Left;
                    break;
                case LevelEditorKeyBinding.LevelPlaneRight:
                    if (HighlightedPosition.X < (LevelArray.GetLength(0) - 1))
                        highlightChange = Vector2D.Right;
                    break;
                default:
                    break;
            }

            if (highlightChange == null)
                return;

            // Reset previously highlighted position
            LevelArray[HighlightedPosition.X, HighlightedPosition.Y].Print(OFFSET);
            // Highlight new position
            ConsoleColor[] unhighlightedColors = new ConsoleColor[] { Colors.EmptyColor, Colors.WallColor }; // The original Pacman.Colors setting
            HighlightedPosition += highlightChange;
            SetColorsHighlight(true, unhighlightedColors);
            LevelArray[HighlightedPosition.X, HighlightedPosition.Y].Print(OFFSET);
            // Reset color setting
            SetColorsHighlight(false, unhighlightedColors);
        }

        private void ChangeHighlightedObjectOfChoice(object sender, KeyEventArgs keyArgs)
        {
            if (keyArgs.keyPressed != ConsoleKey.UpArrow && keyArgs.keyPressed != ConsoleKey.DownArrow)
                return;

            Vector2D previouslyHighlighted = HighlightedObjectOfChoice.Copy();

            if (keyArgs.keyPressed == LevelEditorKeyBinding.ObjectOfChoiceUp)
            {
                if (HighlightedObjectOfChoice.Y == 0)
                    HighlightedObjectOfChoice = new Vector2D(0, ObjectsOfChoice.GetLength(1) - 1);
                else
                    HighlightedObjectOfChoice += Vector2D.Up;
            }
            else if (keyArgs.keyPressed == LevelEditorKeyBinding.ObjectOfChoiceDown)
            {
                if (HighlightedObjectOfChoice.Y == (ObjectsOfChoice.GetLength(1) - 1))
                    HighlightedObjectOfChoice = new Vector2D(0, 0);
                else
                    HighlightedObjectOfChoice += Vector2D.Down;
            }

            ConsoleColor[] unhighlightedColors = new ConsoleColor[] { Colors.EmptyColor, Colors.WallColor };
            SetColorsHighlight(true, unhighlightedColors);
            ObjectsOfChoice[HighlightedObjectOfChoice.X, HighlightedObjectOfChoice.Y].Print(OFFSET_OBJECTS_FOR_CHOICE);
            SetColorsHighlight(false, unhighlightedColors);
            ObjectsOfChoice[previouslyHighlighted.X, previouslyHighlighted.Y].Print(OFFSET_OBJECTS_FOR_CHOICE);


            
        }

        private void PlaceGameObject(object sender, KeyEventArgs keyArgs)
        {
            if (keyArgs.keyPressed != LevelEditorKeyBinding.PlaceObject)
                return;

            // You can't overwrite a player (player has to be present all the time)
            if (LevelArray[HighlightedPosition.X, HighlightedPosition.Y] is Player)
                return; // TODO: Add message - can't overwrite a player, playe him elsewhere

            Vector2D startingPosition = HighlightedPosition.Copy();

            switch(ObjectsOfChoice[HighlightedObjectOfChoice.X, HighlightedObjectOfChoice.Y].GetType())
            {
                case Type playerType when playerType == typeof(Player):
                    RemoveOtherPlayer();
                    LevelArray[HighlightedPosition.X, HighlightedPosition.Y] = new Player(LevelArray, startingPosition);

                    // Ghosts need to know about the new Player staring position
                    UpdateGhostInformation();

                    break;
                case Type wallType when wallType == typeof(Wall):
                    LevelArray[HighlightedPosition.X, HighlightedPosition.Y] = new Wall(LevelArray, startingPosition);
                    break;
                case Type emptyType when emptyType == typeof(Empty):
                    if (((Empty)ObjectsOfChoice[HighlightedObjectOfChoice.X, HighlightedObjectOfChoice.Y]).ContainsCoin)
                        LevelArray[HighlightedPosition.X, HighlightedPosition.Y] = new Empty(LevelArray, startingPosition, true);
                    else if (((Empty)ObjectsOfChoice[HighlightedObjectOfChoice.X, HighlightedObjectOfChoice.Y]).ContainsBoost)
                        LevelArray[HighlightedPosition.X, HighlightedPosition.Y] = new Empty(LevelArray, startingPosition, false, true);
                    break;
                case Type blinkyType when blinkyType == typeof(Blinky):
                case Type pinkyType when pinkyType == typeof(Pinky):
                case Type inkyType when inkyType == typeof(Inky):
                case Type clydeType when clydeType == typeof(Clyde):
                    LevelArray[HighlightedPosition.X, HighlightedPosition.Y] = CreateNewGhost(ObjectsOfChoice[HighlightedObjectOfChoice.X, HighlightedObjectOfChoice.Y].GetType(), startingPosition, GetPlayerPosition());
                    break;
                default:
                    break;
            }

            // Highlight the newly placed tile
            ConsoleColor[] unhighlightedColors = new ConsoleColor[] { Colors.EmptyColor, Colors.WallColor };
            SetColorsHighlight(true, unhighlightedColors);
            LevelArray[HighlightedPosition.X, HighlightedPosition.Y].Print(OFFSET);
            SetColorsHighlight(false, unhighlightedColors);

            // There can be only one player in the level
            void RemoveOtherPlayer()
            {
                for(int x = 0; x < LevelArray.GetLength(0); x++)
                {
                    for(int y = 0; y < LevelArray.GetLength(1); y++)
                    {
                        if (LevelArray[x, y] is Player)
                        {
                            LevelArray[x, y] = new Empty(LevelArray, new Vector2D(x, y));
                            LevelArray[x, y].Print(OFFSET);
                            return;
                        }
                    }
                }
            }

            Ghost CreateNewGhost(Type ghost, Vector2D ghostStartingPosition, Vector2D playerPosition)
            {
                switch (ghost)
                {
                    case Type blinkyType when blinkyType == typeof(Blinky):
                        return new Blinky(LevelArray, ghostStartingPosition, playerPosition);
                    case Type pinkyType when pinkyType == typeof(Pinky):
                        return new Pinky(LevelArray, ghostStartingPosition, playerPosition);
                    case Type inkyType when inkyType == typeof(Inky):
                        return new Inky(LevelArray, ghostStartingPosition, playerPosition);
                    case Type clydeType when clydeType == typeof(Clyde):
                        return new Clyde(LevelArray, ghostStartingPosition, playerPosition);
                    default:
                        throw new ArgumentException("The type isn't a valid ghost!");
                }
            }

            // Updates the ghost's setting of PlayerStartingPosition
            void UpdateGhostInformation()
            {
                foreach (GameObject gameObject in LevelArray)
                {
                    switch (gameObject.GetType())
                    {
                        case Type blinkyType when blinkyType == typeof(Blinky):
                        case Type pinkyType when pinkyType == typeof(Pinky):
                        case Type inkyType when inkyType == typeof(Inky):
                        case Type clydeType when clydeType == typeof(Clyde):
                            LevelArray[gameObject.Position.X, gameObject.Position.Y] = CreateNewGhost(gameObject.GetType(), gameObject.Position, GetPlayerPosition());
                            break;
                        default:
                            break;
                    }
                }
            }

            // Returns the position of player inside the level
            Vector2D GetPlayerPosition()
            {
                foreach (GameObject tile in LevelArray)
                {
                    if (tile is Player)
                    {
                        return tile.Position;
                    }
                }

                throw new ArgumentException("There is no player in the level!");
            }
        }

        private void DeleteGameObject(object sender, KeyEventArgs keyArgs)
        {
            if (keyArgs.keyPressed != LevelEditorKeyBinding.DeleteObject)
                return;

            // Player cannot be deleted from the level
            if (LevelArray[HighlightedPosition.X, HighlightedPosition.Y] is Player)
            {
                // TODO: Add message - player cannot be deleted, move him away
                return;
            }

            // Delete the highlighted object
            LevelArray[HighlightedPosition.X, HighlightedPosition.Y] = new Empty(LevelArray, new Vector2D(HighlightedPosition.X, HighlightedPosition.Y));

            // Print the changes
            ConsoleColor[] unhighlightedColors = new ConsoleColor[] { Colors.EmptyColor, Colors.WallColor };
            SetColorsHighlight(true, unhighlightedColors);
            LevelArray[HighlightedPosition.X, HighlightedPosition.Y].Print(OFFSET);
            SetColorsHighlight(false, unhighlightedColors);
        }

        /// <summary>
        /// Sets the printing colors to their highlighted/unhighlighted versions
        /// </summary>
        /// <param name="setHighlighted">Set to highlighted if true, unhighlighted if false</param>
        /// <param name="unhighlightedColors">ConsoleColor[] {ConsoleColor.Empty, ConsoleColor.WallColor}</param>
        private void SetColorsHighlight(bool setHighlighted, ConsoleColor[] unhighlightedColors)
        {
            if (unhighlightedColors.Length != 2)
                throw new ArgumentException("unhighlightedColors has to contain exactly two ConsoleColors!");

            if (setHighlighted)
            {
                Colors.EmptyColor = LevelEditorColors.HighlightedEmptyColor;
                Colors.WallColor = LevelEditorColors.HighlightedWallColor;
            }
            else
            {
                Colors.EmptyColor = unhighlightedColors[0];
                Colors.WallColor = unhighlightedColors[1];
            }
        }
        #endregion

        #region UI Methods
        private void InitializeMenuAndManager()
        {
            Menu = new Menu();

            Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Label mainLabel = new Label(MAIN_LABEL_TEXT, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button saveButton = new Button(SAVE_BUTTON_TEXT, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            saveButton.OnClick += SaveLevel;
            saveButton.OnClick += Menu.DoExitMenu;

            Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 4, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button escapeButton = new Button(ESCAPE_BUTTON_TEXT, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            escapeButton.OnClick += EscapeMenu;
            escapeButton.OnClick += Menu.DoExitMenu;

            Menu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2, saveButton, emptyLabel3, escapeButton });
            MenuManager = new MenuManager(Menu);
        }

        private void GoToMenu(object sender, KeyEventArgs args)
        {
            if (args.keyPressed != LevelEditorKeyBinding.GoToMenu)
                return;

            CurrentRunningState = RunningState.Menu;
        }

        private void SaveLevel(object sender, EventArgs args)
        {
            TextInputDialog dialog = new TextInputDialog("Please enter path to save your level", "Tha path should contain the name of the file as well, example: 'C:\\level.txt'");

            dialog.Run();

            string path = dialog.DialogStringResult;

            // Try to save find/create the file
            try
            {
                if (!File.Exists(path))
                    File.Create(path).Dispose();
            }
            catch (UnauthorizedAccessException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (DirectoryNotFoundException) { }
            catch (NotSupportedException) { }
            catch (IOException) { }

            // If the file was not created, signal error to the user
            if (!File.Exists(path))
            {
                MessageDialog messageDialog = new MessageDialog("The program was not capable of finding/creating that file");
                messageDialog.Run();
                return;
            }

            // If the error did not occur, save the level
            LevelWriter.SaveLevel(LevelArray, path);
        }

        private void EscapeMenu(object sender, EventArgs args)
        {
            ConfirmationDialog dialog = new ConfirmationDialog(ConfirmationOptions.YesNo, "Do you really want to escape (did you save the level/want it to be discarded)?");

            dialog.Run();

            if(dialog.DialogResult == DialogResult.Yes)
                CurrentRunningState = RunningState.Finished;
        }
        #endregion

        private enum RunningState
        {
            Off,
            On,
            Menu,
            Finished
        }
    }
}