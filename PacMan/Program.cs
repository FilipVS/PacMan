using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Setnicka.PacMan.LevelEditor;
using Setnicka.UI;
using Setnicka.AuxiliaryClasses;


namespace Setnicka.PacMan
{
    class Program
    {
        static void Main(string[] args)
        {
            AppManager appManager = new AppManager();
            appManager.Run();

            Console.Clear();
            Console.WriteLine("I'm back at program!");
            Console.ReadKey(true);


            Console.WriteLine("Do you want to play game (press 'g'), test level editor (press 'e') or test custom level ('press c'): ");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            #region CustomLevelTest
            if(keyInfo.Key == ConsoleKey.C)
            {
                string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName.ToString();
                GameObject[,] level1 = LevelReader.ReadLevel(projectDirectory + @"\\Levels\\CustomLevel.txt");
                GameManager game1 = new GameManager(level1);


                Menu menu = new Menu();
                Button btn = new Button("Press to run the game", HorizontalAlignment.Center, 0, ConsoleColor.Red, ConsoleColor.White, game1.Run);
                btn.OnClick += menu.DoPerformAction;
                menu.AddUIElement(btn);
                MenuManager manager = new MenuManager(menu);
                manager.Run();
            }
            #endregion

            #region Test Level Editor
            if (keyInfo.Key == ConsoleKey.E)
            {
                LevelEditorManager levelCreator = new LevelEditorManager(new Vector2D(20, 10));
                levelCreator.Run();
                Console.ReadKey();
            }
            #endregion

            #region Test Level
            if(keyInfo.Key == ConsoleKey.G)
            {
                GameObject[,] level = new GameObject[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    level[i, 0] = new Wall(level, new Vector2D(i, 0));
                    level[i, 8] = new Wall(level, new Vector2D(i, 8));
                    level[0, i] = new Wall(level, new Vector2D(0, i));
                    level[8, i] = new Wall(level, new Vector2D(8, i));
                }
                level[1, 3] = new Player(level, new Vector2D(1, 3));
                level[7, 7] = new Blinky(level, new Vector2D(7, 7), new Vector2D(1, 3));
                level[6, 7] = new Clyde(level, new Vector2D(6, 7), new Vector2D(1, 3));

                level[6, 3] = new Empty(level, new Vector2D(6, 3), false, true);

                //level[1, 2] = new Wall(level, new Vector2D(1, 2));
                level[2, 2] = new Wall(level, new Vector2D(2, 2));
                level[3, 2] = new Wall(level, new Vector2D(3, 2));
                level[4, 2] = new Wall(level, new Vector2D(4, 2));
                level[5, 2] = new Wall(level, new Vector2D(5, 2));
                level[6, 2] = new Wall(level, new Vector2D(6, 2));
                //level[7, 5] = new Wall(level, new Vector2D(7, 5));

                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        if (level[x, y] == null)
                            level[x, y] = new Empty(level, new Vector2D(x, y), true, false);
                    }
                }

                GameManager game = new GameManager(level);
                game.Run();
            }
            #endregion

            Console.ReadKey();
        }
    }
}
