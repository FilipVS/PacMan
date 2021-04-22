using System;
using System.Collections.Generic;
using System.IO;
using Setnicka.PacMan.LevelEditor;
using Setnicka.UI;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// This class brings all the individual elements (GameManager, LevelEditorManager and UI together)
    /// </summary>
    class AppManager
    {
        #region Constants

        #region MenuDesign
        // General constants
        private const ConsoleColor UNHIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.White;
        private const ConsoleColor UNHIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Black;

        private const ConsoleColor HIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor HIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Red;

        private const ConsoleColor MAIN_LABEL_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor MAIN_LABEL_BACKGROUND_COLOR = ConsoleColor.Yellow;

        // UI constants
        private const string EMPTY_LABEL_TEXT = "";
        private const string GO_TO_PREVIOUS_MENU_BUTTON_TEXT = "Go to previous menu";

        // Main menu
        private const string MAIN_LABEL_TEXT_MAIN_MENU = "Pacman main menu";
        private const string PLAY_GAME_BUTTON_TEXT_MAIN_MENU = "Play game";
        private const string OPEN_LEVEL_EDITOR_BUTTON_TEXT_MAIN_MENU = "Open level editor";
        private const string ESCAPE_BUTTON_TEXT_MAIN_MENU = "Escape";

        // Play game submenu
        private const string MAIN_LABEL_TEXT_PLAYGAME_SUBMENU = "Play game submenu";

        // Open level editor submenu
        private const string MAIN_LABEL_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = "Enter the size of the new level / load level to edit";
        private const string LOAD_LEVEL_TO_EDITOR_BUTTON = "Load existing level to editor";
        private const string DELETE_EXISTING_LEVEL_BUTTON = "Delete existing level";
        private readonly string INPUT_LEVEL_WIDTH_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = $"Input level width (min. {LevelEditorManager.MINIMUM_LEVEL_SIZE.X}, max. {LevelEditorManager.MAXIMUM_LEVEL_SIZE.X}): ";
        private readonly string INPUT_LEVEL_HEIGHT_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = $"Input level height (min. {LevelEditorManager.MINIMUM_LEVEL_SIZE.Y}, max. {LevelEditorManager.MAXIMUM_LEVEL_SIZE.Y}): ";
        private const string CREATE_NEW_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU = "Create new level";

        // Load level to editor submenu
        private const string MAIN_LABEL_TEXT_LOAD_LEVEL_TO_EDITOR_SUBMENU = "Load existing level to editor";

        // Delete level submenu
        private const string MAIN_LABEL_TEXT_DELETE_LEVEL_SUBMENU = "Choose level to be deleted (there is no way to revert)";
        #endregion

        #endregion

        #region Constructors
        public AppManager()
        {
            InitializeMenusAndManagers();
        }
        #endregion

        #region Properties
        private Menu MainMenu { get; set; }
        private MenuManager MainMenuManager { get; set; }

        private Menu OpenLevelEditorSubmenu { get; set; }
        private MenuManager OpenLevelEditorSubmenuManager { get; set; }
        NumberField EnterLevelWidthNumberField { get; set; }
        NumberField EnterLevelHeightNumberField { get; set; }
        #endregion

        #region Methods
        public void Run()
        {
            MainMenuManager.Run();
        }

        #region Menus
        private void InitializeMenusAndManagers()
        {
            InitializeOpenLevelEditorSubmenuAndManager();

            InitiliazeMainMenuAndManager();



            void InitiliazeMainMenuAndManager()
            {
                MainMenu = new Menu();

                Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label labelMain = new Label(MAIN_LABEL_TEXT_MAIN_MENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);
                
                Label labelEmpty2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button buttonPlayGame = new Button(PLAY_GAME_BUTTON_TEXT_MAIN_MENU, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, InitializePlayGameSubmenuAndManagerAndRun);
                buttonPlayGame.OnClick += MainMenu.DoPerformAction;
                Button buttonOpenLevelEditor = new Button(OPEN_LEVEL_EDITOR_BUTTON_TEXT_MAIN_MENU, HorizontalAlignment.Center, 4, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, OpenLevelEditorSubmenuManager.Run);
                buttonOpenLevelEditor.OnClick += MainMenu.DoPerformAction;

                Label labelEmpty3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 5, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button buttonEscape = new Button(ESCAPE_BUTTON_TEXT_MAIN_MENU, HorizontalAlignment.Center, 6, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                buttonEscape.OnClick += MainMenu.DoExitMenu;

                MainMenu.AddUIElementRange(new List<IUIElement>() {emptyLabel1, labelMain, labelEmpty2, buttonPlayGame, buttonOpenLevelEditor, labelEmpty3, buttonEscape});
                MainMenuManager = new MenuManager(MainMenu);
            }

            void InitializeOpenLevelEditorSubmenuAndManager()
            {
                OpenLevelEditorSubmenu = new Menu();

                Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label mainLabel = new Label(MAIN_LABEL_TEXT_OPEN_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

                Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button loadExistingLevelButton = new Button(LOAD_LEVEL_TO_EDITOR_BUTTON, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, InitializeOpenLevelToEditorSubmenuAndManagerAndRun);
                loadExistingLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 4, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button deleteExistingLevelButton = new Button(DELETE_EXISTING_LEVEL_BUTTON, HorizontalAlignment.Center, 5, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, InitializeDeleteLevelSubmenuAndManagerAndRun);
                deleteExistingLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel4 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 6, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                EnterLevelWidthNumberField = new NumberField(INPUT_LEVEL_WIDTH_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU, LevelEditorManager.MINIMUM_LEVEL_SIZE.X, 2, HorizontalAlignment.Center, 7, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                EnterLevelHeightNumberField = new NumberField(INPUT_LEVEL_HEIGHT_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU, LevelEditorManager.MINIMUM_LEVEL_SIZE.Y, 2, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label emptyLabel5 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 7, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button createNewLevelButton = new Button(CREATE_NEW_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 9, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, OpenLevelEditor);
                createNewLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel6 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 10, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 11, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                goToPreviousMenuButton.OnClick += OpenLevelEditorSubmenu.DoExitMenu;

                OpenLevelEditorSubmenu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2, loadExistingLevelButton, emptyLabel3, deleteExistingLevelButton, emptyLabel4, EnterLevelWidthNumberField, EnterLevelHeightNumberField, emptyLabel5, createNewLevelButton, emptyLabel6, goToPreviousMenuButton });

                // The MenuManager needs to get a list of keys, that he needs to additionaly listen to
                List<ConsoleKey> numbers = new List<ConsoleKey>() { ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9, ConsoleKey.NumPad0, ConsoleKey.NumPad1, ConsoleKey.NumPad2, ConsoleKey.NumPad3, ConsoleKey.NumPad4, ConsoleKey.NumPad5, ConsoleKey.NumPad6, ConsoleKey.NumPad7, ConsoleKey.NumPad8, ConsoleKey.NumPad9 };

                OpenLevelEditorSubmenuManager = new MenuManager(OpenLevelEditorSubmenu, numbers);
            }
        }

        /// <summary>
        /// Used for opening the PLayGame submenu (it needs to be initialized right before it opens)
        /// </summary>
        private void InitializePlayGameSubmenuAndManagerAndRun()
        {
            Menu playGameSubmenu = new Menu();

            Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Label mainLabel = new Label(MAIN_LABEL_TEXT_PLAYGAME_SUBMENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            playGameSubmenu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2 });

            // Add buttons according to the number of levels
            // Get potential levels
            string[] potentialLevels = Directory.GetFiles(LevelWriter.GetPathToLevels());

            // For all potential levels
            for(int i = 0; i < potentialLevels.Length; i++)
            {
                // Check if the file is openable
                if (!FileOpenable(potentialLevels[i]))
                    continue;

                // Check if the file stores level
                if (LevelReader.ReadLevel(potentialLevels[i]) == null)
                    continue;

                // Else add button, that aims at this file
                string strCopy = String.Copy(potentialLevels[i]);
                Button btn = new Button(Path.GetFileNameWithoutExtension(potentialLevels[i]), HorizontalAlignment.Center, (3 + i), HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, new Action(() => { PlayLevel(strCopy); }));
                btn.OnClick += playGameSubmenu.DoPerformAction;
                playGameSubmenu.AddUIElement(btn);
            }
            //

            Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 7, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            goToPreviousMenuButton.OnClick += playGameSubmenu.DoExitMenu;

            playGameSubmenu.AddUIElementRange(new List<IUIElement>() { emptyLabel3, goToPreviousMenuButton });
            MenuManager playGameSubmenuManager = new MenuManager(playGameSubmenu);
            playGameSubmenuManager = new MenuManager(playGameSubmenu);

            // Run the submenu
            playGameSubmenuManager.Run();
        }

        /// <summary>
        /// Used for opening submenu that allows the user to choose the level he wants to edit
        /// </summary>
        private void InitializeOpenLevelToEditorSubmenuAndManagerAndRun()
        {
            Menu openLevelToEditorMenu = new Menu();

            Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Label mainLabel = new Label(MAIN_LABEL_TEXT_LOAD_LEVEL_TO_EDITOR_SUBMENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            openLevelToEditorMenu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2 });

            // Add buttons according to the number of levels
            // Get potential levels
            string[] potentialLevels = Directory.GetFiles(LevelWriter.GetPathToLevels());

            // For all potential levels
            for (int i = 0; i < potentialLevels.Length; i++)
            {
                // Check if the file is openable
                if (!FileOpenable(potentialLevels[i]))
                    continue;

                // Check if the file stores level
                if (LevelReader.ReadLevel(potentialLevels[i]) == null)
                    continue;

                // Else add button, that aims at this file
                string strCopy = String.Copy(potentialLevels[i]);
                Button btn = new Button(Path.GetFileNameWithoutExtension(potentialLevels[i]), HorizontalAlignment.Center, (3 + i), HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, new Action(() => { LoadLevelToEditor(strCopy); }));
                btn.OnClick += openLevelToEditorMenu.DoPerformAction;
                openLevelToEditorMenu.AddUIElement(btn);
            }

            Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 7, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            goToPreviousMenuButton.OnClick += openLevelToEditorMenu.DoExitMenu;

            openLevelToEditorMenu.AddUIElementRange(new List<IUIElement>() { emptyLabel3, goToPreviousMenuButton });
            MenuManager openLevelToEditorManager = new MenuManager(openLevelToEditorMenu);
            openLevelToEditorManager = new MenuManager(openLevelToEditorMenu);

            // Run the submenu
            openLevelToEditorManager.Run();
        }

        /// <summary>
        /// Used for opening submenu that allows the user to delete a level
        /// </summary>
        private void InitializeDeleteLevelSubmenuAndManagerAndRun()
        {
            Menu deleteLevelMenu = new Menu();

            Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Label mainLabel = new Label(MAIN_LABEL_TEXT_DELETE_LEVEL_SUBMENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            deleteLevelMenu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2 });

            // Add buttons according to the number of levels
            // Get potential levels
            string[] potentialLevels = Directory.GetFiles(LevelWriter.GetPathToLevels());

            // For all potential levels
            for (int i = 0; i < potentialLevels.Length; i++)
            {
                // Check if the file is openable
                if (!FileOpenable(potentialLevels[i]))
                    continue;

                // Check if the file stores level
                if (LevelReader.ReadLevel(potentialLevels[i]) == null)
                    continue;

                // Else add button, that aims at this file
                string strCopy = String.Copy(potentialLevels[i]);
                Button btn = new Button(Path.GetFileNameWithoutExtension(potentialLevels[i]), HorizontalAlignment.Center, (3 + i), HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                btn.OnClick += new EventHandler((object sender, EventArgs args) => { File.Delete(strCopy); });
                btn.OnClick += deleteLevelMenu.DoExitMenu;
                deleteLevelMenu.AddUIElement(btn);
            }

            Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 7, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            goToPreviousMenuButton.OnClick += deleteLevelMenu.DoExitMenu;

            deleteLevelMenu.AddUIElementRange(new List<IUIElement>() { emptyLabel3, goToPreviousMenuButton });
            MenuManager deleteLevelMenuManager = new MenuManager(deleteLevelMenu);
            deleteLevelMenuManager = new MenuManager(deleteLevelMenu);

            // Run the submenu
            deleteLevelMenuManager.Run();
        }
        #endregion

        /// <summary>
        /// Used for running level
        /// </summary>
        /// <param name="pathToLevel"></param>
        private void PlayLevel(string pathToLevel)
        {
            bool fileOpenable = FileOpenable(pathToLevel);

            // Signal the error to the user
            if (!fileOpenable)
            {
                MessageDialog messageDialog = new MessageDialog("The file was not found.");
                messageDialog.Run();
                return;
            }

            GameObject[,] level = LevelReader.ReadLevel(pathToLevel);

            // If the level loading did not go well
            if(level == null)
            {
                MessageDialog messageDialog = new MessageDialog("The level was not loaded succesfully.");
                messageDialog.Run();
                return;
            }

            GameManager manager = new GameManager(level);
            manager.Run();
        }

        /// <summary>
        /// Used for running the instance of level editor
        /// </summary>
        private void OpenLevelEditor()
        {
            int levelWidth = EnterLevelWidthNumberField.NumberInt;
            int levelHeight = EnterLevelHeightNumberField.NumberInt;

            // Confirm that the numbers are within bounds
            if (levelWidth < LevelEditorManager.MINIMUM_LEVEL_SIZE.X)
                levelWidth = LevelEditorManager.MINIMUM_LEVEL_SIZE.X;
            else if (levelWidth > LevelEditorManager.MAXIMUM_LEVEL_SIZE.X)
                levelWidth = LevelEditorManager.MAXIMUM_LEVEL_SIZE.X;

            if (levelHeight < LevelEditorManager.MINIMUM_LEVEL_SIZE.Y)
                levelHeight = LevelEditorManager.MINIMUM_LEVEL_SIZE.Y;
            else if (levelHeight > LevelEditorManager.MAXIMUM_LEVEL_SIZE.Y)
                levelHeight = LevelEditorManager.MAXIMUM_LEVEL_SIZE.Y;

            LevelEditorManager manager = new LevelEditorManager(new Vector2D(levelWidth, levelHeight));
            manager.Run();
        }

        /// <summary>
        /// Used for running the instance of level editor with an existing level
        /// </summary>
        /// <param name="pathToLevel"></param>
        private void LoadLevelToEditor(string pathToLevel)
        {
            bool fileOpenable = FileOpenable(pathToLevel);

            // Signal the error to the user
            if (!fileOpenable)
            {
                MessageDialog messageDialog = new MessageDialog("The file was not found.");
                messageDialog.Run();
                return;
            }

            GameObject[,] level = LevelReader.ReadLevel(pathToLevel);

            // If the level loading did not go well
            if (level == null)
            {
                MessageDialog messageDialog = new MessageDialog("The level was not loaded succesfully.");
                messageDialog.Run();
                return;
            }

            // Else run the manager
            LevelEditorManager manager = new LevelEditorManager(level);
            manager.Run();
        }

        /// <summary>
        /// Checks, whether a given path is openable
        /// </summary>
        private bool FileOpenable(string path)
        {
            bool fileOpenable = false;

            // Test if the file can be opened
            try
            {
                File.Open(path, FileMode.Open).Dispose();

                fileOpenable = true;
            }
            catch (ArgumentOutOfRangeException) { }
            catch (FileNotFoundException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (DirectoryNotFoundException) { }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            catch (NotSupportedException) { }

            return fileOpenable;
        }
        #endregion
    }
}
