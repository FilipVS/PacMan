using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Setnicka.PacMan.LevelEditor;
using Setnicka.UI;

namespace Setnicka.PacMan
{
    class Program
    {
        static void Main(string[] args)
        {
            TestRun.Run();

            Console.WriteLine("Do you want to test level editor (press 'E') or play level (press any other key): ");
            bool testLevelEditor = (Console.ReadKey(true).Key == ConsoleKey.E);

            #region Test Level Editor
            if (testLevelEditor)
            {
                LevelCreator levelCreator = new LevelCreator(new Vector2D(10, 10));
                levelCreator.Run();
                Console.ReadKey();
            }
            #endregion

            #region Test Level
            GameObject[,] level = new GameObject[9, 9];
            for(int i = 0; i < 9; i++)
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

            Game game = new Game(level);
            game.Run();
            #endregion

            Console.ReadKey();
        }
    }
}
