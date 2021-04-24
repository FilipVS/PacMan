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

        private const ConsoleColor SECONDARY_LABEL_FOREGROUND_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor SECONDARY_LABEL_BACKGROUND_COLOR = ConsoleColor.Black;

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
        private const string LEVELS_LABEL_TEXT_PLAYGAME_SUBMENU = "Levels";

        // Open level editor submenu
        private const string MAIN_LABEL_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = "Enter the size of the new level / load level to edit";
        private const string LOAD_LEVEL_TO_EDITOR_BUTTON_LEVEL_EDITOR_SUBMENU = "Load existing level to editor";
        private const string DELETE_EXISTING_LEVEL_BUTTON_LEVEL_EDITOR_SUBMENU = "Delete existing level";
        private const string CREATE_NEW_LEVEL_LABEL_TEXT_LEVEL_EDITOR_SUBMENU = "Create new level";
        private readonly string INPUT_LEVEL_WIDTH_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = $"Input level width (min. {LevelEditorManager.MINIMUM_LEVEL_SIZE.X}, max. {LevelEditorManager.MAXIMUM_LEVEL_SIZE.X}): ";
        private readonly string INPUT_LEVEL_HEIGHT_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = $"Input level height (min. {LevelEditorManager.MINIMUM_LEVEL_SIZE.Y}, max. {LevelEditorManager.MAXIMUM_LEVEL_SIZE.Y}): ";
        private const string CREATE_NEW_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU = "Create new level";
        private const string IMPORT_EXPORT_LABEL_TEXT_LEVEL_EDITOR_SUBMENU = "Import/Export level";
        private const string IMPORT_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU = "Import existing level";
        private const string EXPORT_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU = "Export existing level";

        // Load level to editor submenu
        private const string MAIN_LABEL_TEXT_LOAD_LEVEL_TO_EDITOR_SUBMENU = "Load existing level to editor";
        private const string LEVELS_LABEL_TEXT_LOAD_LEVEL_TO_EDITOR_SUBMENU = "Levels";

        // Delete level submenu
        private const string MAIN_LABEL_TEXT_DELETE_LEVEL_SUBMENU = "Choose level to be deleted (there is no way to revert)";
        private const string LEVELS_LABEL_TEXT_DELETE_LEVEL_SUBMENU = "Levels";

        // Export level submenu
        private const string MAIN_LABEL_TEXT_EXPORT_LEVEL_SUBMENU = "Choose level to be exported";
        private const string LEVELS_LABEL_TEXT_EXPORT_LEVEL_SUBMENU = "Levels";
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

                Button loadExistingLevelButton = new Button(LOAD_LEVEL_TO_EDITOR_BUTTON_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, InitializeOpenLevelToEditorSubmenuAndManagerAndRun);
                loadExistingLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 4, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button deleteExistingLevelButton = new Button(DELETE_EXISTING_LEVEL_BUTTON_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 5, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, InitializeDeleteLevelSubmenuAndManagerAndRun);
                deleteExistingLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel4 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 6, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label createNewLevelLabel = new Label(CREATE_NEW_LEVEL_LABEL_TEXT_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 7, SECONDARY_LABEL_FOREGROUND_COLOR, SECONDARY_LABEL_BACKGROUND_COLOR);

                EnterLevelWidthNumberField = new NumberField(INPUT_LEVEL_WIDTH_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU, LevelEditorManager.MINIMUM_LEVEL_SIZE.X, 2, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                EnterLevelHeightNumberField = new NumberField(INPUT_LEVEL_HEIGHT_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU, LevelEditorManager.MINIMUM_LEVEL_SIZE.Y, 2, HorizontalAlignment.Center, 9, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button createNewLevelButton = new Button(CREATE_NEW_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 10, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, OpenLevelEditor);
                createNewLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel5 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 11, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label importExportLabel = new Label(IMPORT_EXPORT_LABEL_TEXT_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 12, SECONDARY_LABEL_FOREGROUND_COLOR, SECONDARY_LABEL_BACKGROUND_COLOR);

                Button importLevelButton = new Button(IMPORT_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 13, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, ImportLevel);
                importLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;
                Button exportLevelButton = new Button(EXPORT_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 14, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, InitializeExportLevelSubmenuAndManagerAndRun);
                exportLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel6 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 15, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 16, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                goToPreviousMenuButton.OnClick += OpenLevelEditorSubmenu.DoExitMenu;

                OpenLevelEditorSubmenu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2, loadExistingLevelButton, emptyLabel3, deleteExistingLevelButton, emptyLabel4, createNewLevelLabel, EnterLevelWidthNumberField, EnterLevelHeightNumberField, createNewLevelButton, emptyLabel5, importExportLabel, importLevelButton, exportLevelButton, emptyLabel6, goToPreviousMenuButton });

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
            Menu playGameSubmenu = InitializeLevelLoadingMenu(MAIN_LABEL_TEXT_PLAYGAME_SUBMENU, PlayLevel);

            MenuManager playGameSubmenuManager = new MenuManager(playGameSubmenu);

            // Run the submenu
            playGameSubmenuManager.Run();
        }

        /// <summary>
        /// Used for opening submenu that allows the user to choose the level he wants to edit
        /// </summary>
        private void InitializeOpenLevelToEditorSubmenuAndManagerAndRun()
        {
            Menu openLevelToEditorSubmenu = InitializeLevelLoadingMenu(MAIN_LABEL_TEXT_LOAD_LEVEL_TO_EDITOR_SUBMENU, LoadLevelToEditor);

            MenuManager openLevelToEditorSubmenuManager = new MenuManager(openLevelToEditorSubmenu);

            // Run the submenu
            openLevelToEditorSubmenuManager.Run();
        }

        /// <summary>
        /// Used for opening submenu that allows the user to delete a level
        /// </summary>
        private void InitializeDeleteLevelSubmenuAndManagerAndRun()
        {
            Menu deleteLevelSubmenu = InitializeLevelLoadingMenu(MAIN_LABEL_TEXT_DELETE_LEVEL_SUBMENU, DeleteLevel, true);

            MenuManager deleteLevelSubmenuManager = new MenuManager(deleteLevelSubmenu);

            // Run the submenu
            deleteLevelSubmenuManager.Run();
        }

        /// <summary>
        /// Usef or opening submenu, that allows the user to export a level
        /// </summary>
        private void InitializeExportLevelSubmenuAndManagerAndRun()
        {
            Menu exportLevelSubmenu = InitializeLevelLoadingMenu(MAIN_LABEL_TEXT_EXPORT_LEVEL_SUBMENU, ExportLevel);

            MenuManager exportLevelSubmenuManager = new MenuManager(exportLevelSubmenu);

            // Run the submenu
            exportLevelSubmenuManager.Run();
        }

        /// <summary>
        /// Used to generaly initialize a submenu, that loads all the available levels for interaction (DeleteLevelSubmenu/LoadLevelToLevelEditoSubmenu...)
        /// </summary>
        private Menu InitializeLevelLoadingMenu(string mainLabelText, Action<string> actionOnButtons, bool endSubmenuUponClick = false)
        {
            Menu menu = new Menu();

            Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Label mainLabel = new Label(mainLabelText, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

            Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Label levelsLabel = new Label(LEVELS_LABEL_TEXT_PLAYGAME_SUBMENU, HorizontalAlignment.Center, 3, SECONDARY_LABEL_FOREGROUND_COLOR, SECONDARY_LABEL_BACKGROUND_COLOR);

            menu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2, levelsLabel });

            // Add buttons according to the number of levels
            // Get potential levels
            string[] potentialLevels = Directory.GetFiles(LevelWriter.GetPathToLevels());

            // For all potential levels
            for (int i = 0; i < potentialLevels.Length; i++)
            {
                // Check if the file is openable
                if (!LevelReader.FileOpenable(potentialLevels[i]))
                    continue;

                // Check if the file stores level
                if (LevelReader.ReadLevel(potentialLevels[i]) == null)
                    continue;

                // Else add button, that aims at this file
                string strCopy = String.Copy(potentialLevels[i]);
                Button btn = new Button(Path.GetFileNameWithoutExtension(potentialLevels[i]), HorizontalAlignment.Center, (4 + i), HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, new Action(() => { actionOnButtons(strCopy); }));
                if(!endSubmenuUponClick)
                    btn.OnClick += menu.DoPerformAction;
                else
                {
                    btn.OnClick += new EventHandler((object sender, EventArgs args) => { actionOnButtons(strCopy); });
                    btn.OnClick += menu.DoExitMenu;
                }
                menu.AddUIElement(btn);
            }
            //

            Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 7, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

            Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
            goToPreviousMenuButton.OnClick += menu.DoExitMenu;

            menu.AddUIElementRange(new List<IUIElement>() { emptyLabel3, goToPreviousMenuButton });

            return menu;
        }
        #endregion

        /// <summary>
        /// Used for running level
        /// </summary>
        private void PlayLevel(string pathToLevel)
        {
            bool fileOpenable = LevelReader.FileOpenable(pathToLevel);

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
            bool fileOpenable = LevelReader.FileOpenable(pathToLevel);

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
        /// Used for deleting an existing level
        /// </summary>
        /// <param name="pathToLevel"></param>
        private void DeleteLevel(string pathToLevel)
        {
            File.Delete(pathToLevel);

            // Singnal the success
            MessageDialog messageDialog = new MessageDialog("Level deleted successfully.");
            messageDialog.Run();
        }

        /// <summary>
        /// Allows user to import new levels from other places of the computer
        /// </summary>
        private void ImportLevel()
        {
            TextInputDialog dialog = new TextInputDialog("Enter path to the level", "Please enter full path, for example: 'C:\\Levels\\LevelToImport.txt'");
            dialog.Run();

            string path = dialog.DialogStringResult;

            // If level cannot be loaded from this file, signal the error and retrn
            if(!LevelReader.FileOpenable(path) || LevelReader.ReadLevel(path) == null)
            {
                MessageDialog messageDialog = new MessageDialog("The program was unable to load the level.");
                messageDialog.Run();
                return;
            }

            // Path of the imported
            string newPath = LevelWriter.GetPathToLevels() + Path.GetFileNameWithoutExtension(path) + ".txt";

            // If there is already a level at that path - ask the user
            if (File.Exists(newPath))
            {
                ConfirmationDialog confirmationDialog = new ConfirmationDialog(ConfirmationOptions.YesNo, "Level with that name is already there, do you want to overwrite it?");
                confirmationDialog.Run();
                if (confirmationDialog.DialogResult != DialogResult.Yes)
                    return;
            }

            // Import the level
            LevelWriter.SaveLevel(LevelReader.ReadLevel(path), newPath);

            // Signal success to the user
            // Infrom the user about the successful save
            MessageDialog successfulImport = new MessageDialog("The level was imported successfully.");
            successfulImport.Run();
        }

        /// <summary>
        /// Used for exporting chosen level
        /// </summary>
        private void ExportLevel(string levelPath)
        {
            TextInputDialog dialog = new TextInputDialog("Enter path that you want to export the level to", "Please enter full path, for example: 'C:\\Levels\\ExportedLevel.txt'");
            dialog.Run();

            string path = dialog.DialogStringResult;

            // If file exist, disallow the user to continue
            if (File.Exists(path))
            {
                MessageDialog messageDialogFileExists = new MessageDialog("That file already exists, delete it first.");
                messageDialogFileExists.Run();
                return;
            }

            // Check if the program can create such file (otherwise signal error to the user and return)
            if (!LevelWriter.FileCreatable(path))
            {
                MessageDialog messageDialogFileCannotBeCreated = new MessageDialog("Program was unable to create such file.");
                messageDialogFileCannotBeCreated.Run();
                return;
            }

            // Otherwise export level
            LevelWriter.SaveLevel(LevelReader.ReadLevel(levelPath), path);

            // Signal success to the user
            // Infrom the user about the successful save
            MessageDialog successfulExport = new MessageDialog("The level was exported successfully.");
            successfulExport.Run();
        }
        #endregion
    }
}
