using System;
using System.Collections.Generic;
using System.IO;
using Setnicka.PacMan.LevelEditor;
using Setnicka.UI;
using Setnicka.AuxiliaryClasses;
using System.Threading;

namespace Setnicka.PacMan
{
    /// <summary>
    /// This class brings all the individual elements (GameManager, LevelEditorManager and UI together)
    /// </summary>
    class AppManager
    {
        #region Constants

        #region MenuDesign
        // General
        private const ConsoleColor UNHIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.White;
        private const ConsoleColor UNHIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Black;

        private const ConsoleColor HIGHLIGHTED_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor HIGHLIGHTED_BACKGROUND_COLOR = ConsoleColor.Red;

        private const ConsoleColor MAIN_LABEL_FOREGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor MAIN_LABEL_BACKGROUND_COLOR = ConsoleColor.Yellow;

        private const string EMPTY_LABEL_TEXT = "";
        private const string GO_TO_PREVIOUS_MENU_BUTTON_TEXT = "Go to previous menu";

        // Main menu
        private const string MAIN_LABEL_TEXT_MAIN_MENU = "Pacman main menu";
        private const string PLAY_GAME_BUTTON_TEXT_MAIN_MENU = "Play game";
        private const string OPEN_LEVEL_EDITOR_BUTTON_TEXT_MAIN_MENU = "Open level editor";
        private const string ESCAPE_BUTTON_TEXT_MAIN_MENU = "Escape";

        // Play game submenu
        private const string MAIN_LABEL_TEXT_PLAYGAME_SUBMENU = "Play game submenu";
        private const string PLAY_LEVEL1_BUTTON_TEX_PLAY_GAME_SUBMENU = "Level 1";
        private const string PLAY_LEVEL2_BUTTON_TEX_PLAY_GAME_SUBMENU = "Level 2";
        private const string PLAY_LEVEL3_BUTTON_TEX_PLAY_GAME_SUBMENU = "Level 3";
        private const string PLAY_CUSTOM_LEVEL_BUTTON_TEX_PLAY_GAME_SUBMENU = "Custom level";

        // Open level editor submenu
        private const string MAIN_LABEL_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = "Enter the size of the new level";
        private readonly string INPUT_LEVEL_WIDTH_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = $"Input level width (min. {LevelEditorManager.MINIMUM_LEVEL_SIZE.X}, max. {LevelEditorManager.MAXIMUM_LEVEL_SIZE.X}): ";
        private readonly string INPUT_LEVEL_HEIGHT_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU = $"Input level height (min. {LevelEditorManager.MINIMUM_LEVEL_SIZE.Y}, max. {LevelEditorManager.MAXIMUM_LEVEL_SIZE.Y}): ";
        private const string CREATE_NEW_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU = "Create new level";
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

        private Menu PlayGameSubmenu { get; set; }
        private MenuManager PlayGameSubmenuManager { get; set; }

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

        private void InitializeMenusAndManagers()
        {
            InitializePlayGameSubmenuAndManager();

            InitializeOpenLevelEditorSubmenuAndManager();

            InitiliazeMainMenuAndManager();



            void InitiliazeMainMenuAndManager()
            {
                MainMenu = new Menu();

                Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label labelMain = new Label(MAIN_LABEL_TEXT_MAIN_MENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);
                
                Label labelEmpty2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button buttonPlayGame = new Button(PLAY_GAME_BUTTON_TEXT_MAIN_MENU, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, PlayGameSubmenuManager.Run);
                buttonPlayGame.OnClick += MainMenu.DoPerformAction;
                Button buttonOpenLevelEditor = new Button(OPEN_LEVEL_EDITOR_BUTTON_TEXT_MAIN_MENU, HorizontalAlignment.Center, 4, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, OpenLevelEditorSubmenuManager.Run);
                buttonOpenLevelEditor.OnClick += MainMenu.DoPerformAction;

                Label labelEmpty3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 5, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button buttonEscape = new Button(ESCAPE_BUTTON_TEXT_MAIN_MENU, HorizontalAlignment.Center, 6, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                buttonEscape.OnClick += MainMenu.DoExitMenu;

                MainMenu.AddUIElementRange(new List<IUIElement>() {emptyLabel1, labelMain, labelEmpty2, buttonPlayGame, buttonOpenLevelEditor, labelEmpty3, buttonEscape});
                MainMenuManager = new MenuManager(MainMenu);
            }

            void InitializePlayGameSubmenuAndManager()
            {
                PlayGameSubmenu = new Menu();

                Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label mainLabel = new Label(MAIN_LABEL_TEXT_PLAYGAME_SUBMENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

                Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button playLevel1Button = new Button(PLAY_LEVEL1_BUTTON_TEX_PLAY_GAME_SUBMENU, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, PlayLevel1);
                playLevel1Button.OnClick += PlayGameSubmenu.DoPerformAction;
                Button playLevel2Button = new Button(PLAY_LEVEL2_BUTTON_TEX_PLAY_GAME_SUBMENU, HorizontalAlignment.Center, 4, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, PlayLevel2);
                playLevel2Button.OnClick += PlayGameSubmenu.DoPerformAction;
                Button playLevel3Button = new Button(PLAY_LEVEL3_BUTTON_TEX_PLAY_GAME_SUBMENU, HorizontalAlignment.Center, 5, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, PlayLevel3);
                playLevel3Button.OnClick += PlayGameSubmenu.DoPerformAction;
                Button playCustomLevelButton = new Button(PLAY_CUSTOM_LEVEL_BUTTON_TEX_PLAY_GAME_SUBMENU, HorizontalAlignment.Center, 6, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, PlayCustomLevel);
                playCustomLevelButton.OnClick += PlayGameSubmenu.DoPerformAction;

                Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 7, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                goToPreviousMenuButton.OnClick += PlayGameSubmenu.DoExitMenu;

                PlayGameSubmenu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2, playLevel1Button, playLevel2Button, playLevel3Button, playCustomLevelButton, emptyLabel3, goToPreviousMenuButton });
                PlayGameSubmenuManager = new MenuManager(PlayGameSubmenu);

                // TODO: Add events to buttons
            }

            void InitializeOpenLevelEditorSubmenuAndManager()
            {
                OpenLevelEditorSubmenu = new Menu();

                Label emptyLabel1 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 0, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label mainLabel = new Label(MAIN_LABEL_TEXT_PLAYGAME_SUBMENU, HorizontalAlignment.Center, 1, MAIN_LABEL_FOREGROUND_COLOR, MAIN_LABEL_BACKGROUND_COLOR);

                Label emptyLabel2 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 2, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                EnterLevelWidthNumberField = new NumberField(INPUT_LEVEL_WIDTH_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU, LevelEditorManager.MINIMUM_LEVEL_SIZE.X, 3, HorizontalAlignment.Center, 3, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                EnterLevelHeightNumberField = new NumberField(INPUT_LEVEL_HEIGHT_NUMBERFIELD_TEXT_OPEN_LEVEL_EDITOR_SUBMENU, LevelEditorManager.MINIMUM_LEVEL_SIZE.Y, 2, HorizontalAlignment.Center, 4, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Label emptyLabel3 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 5, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button createNewLevelButton = new Button(CREATE_NEW_LEVEL_BUTTON_TEXT_LEVEL_EDITOR_SUBMENU, HorizontalAlignment.Center, 6, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR, OpenLevelEditor);
                createNewLevelButton.OnClick += OpenLevelEditorSubmenu.DoPerformAction;

                Label emptyLabel4 = new Label(EMPTY_LABEL_TEXT, HorizontalAlignment.Center, 7, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);

                Button goToPreviousMenuButton = new Button(GO_TO_PREVIOUS_MENU_BUTTON_TEXT, HorizontalAlignment.Center, 8, HIGHLIGHTED_FOREGROUND_COLOR, HIGHLIGHTED_BACKGROUND_COLOR, UNHIGHLIGHTED_FOREGROUND_COLOR, UNHIGHLIGHTED_BACKGROUND_COLOR);
                goToPreviousMenuButton.OnClick += OpenLevelEditorSubmenu.DoExitMenu;

                OpenLevelEditorSubmenu.AddUIElementRange(new List<IUIElement>() { emptyLabel1, mainLabel, emptyLabel2, EnterLevelWidthNumberField, EnterLevelHeightNumberField, emptyLabel3, createNewLevelButton, emptyLabel4, goToPreviousMenuButton });

                // The MenuManager needs to get a list of keys, that he needs to additionaly listen to
                List<ConsoleKey> numbers = new List<ConsoleKey>() { ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9, ConsoleKey.NumPad0, ConsoleKey.NumPad1, ConsoleKey.NumPad2, ConsoleKey.NumPad3, ConsoleKey.NumPad4, ConsoleKey.NumPad5, ConsoleKey.NumPad6, ConsoleKey.NumPad7, ConsoleKey.NumPad8, ConsoleKey.NumPad9 };

                OpenLevelEditorSubmenuManager = new MenuManager(OpenLevelEditorSubmenu, numbers);

                // TODO: Subscribe button events
            }
        }

        #region Play game methods

        private void PlayLevel1()
        {
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName.ToString();
            string pathToLevel = (projectDirectory + @"\\Levels\\CustomLevel.txt");

            PlayLevel(pathToLevel);

            // TODO: Set proper path
        }
        private void PlayLevel2()
        {
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName.ToString();
            string pathToLevel = (projectDirectory + @"\\Levels\\CustomLevel.txt");

            PlayLevel(pathToLevel);

            // TODO: Set proper path
        }
        private void PlayLevel3()
        {
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName.ToString();
            string pathToLevel = (projectDirectory + @"\\Levels\\CustomLevel.txt");

            PlayLevel(pathToLevel);

            // TODO: Set proper path
        }
        private void PlayCustomLevel()
        {
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName.ToString();
            string pathToLevel = (projectDirectory + @"\\Levels\\CustomLevel.txt");

            PlayLevel(pathToLevel);

            // TODO: Set proper path
        }
        private void PlayLevel(string pathToLevel)
        {
            GameManager manager = new GameManager(LevelReader.ReadLevel(pathToLevel));
            manager.Run();
        }
        #endregion

        private void OpenLevelEditor()
        {
            int levelWidth = EnterLevelWidthNumberField.Number;
            int levelHeight = EnterLevelHeightNumberField.Number;

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

        #endregion
    }
}
