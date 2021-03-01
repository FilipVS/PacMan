using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan.LevelEditor
{
    /// <summary>
    /// Used for loading levels from files
    /// </summary>
    static class LevelReader
    {
        public static GameObject[,] ReadLevel(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path", "path argument cannot be null or empty!");
            if (!File.Exists(path))
                throw new ArgumentException("path", "The path argument doesn't lead to any file!");

            GameObject[,] level;

            using(StreamReader reader = new StreamReader(path))
            {
                // Confirm that the header text is present
                if (reader.ReadLine() != LevelWriter.HEADER_TEXT)
                    throw new ArgumentException("path", "The file doesn't cointain proper level information!");

                // Read the version number and use appropriate reader for it
                string version = reader.ReadLine();
                switch (version)
                {
                    case LevelWriter.VERSION_1_STRING:
                        level = ReadLevelV1(reader);
                        break;
                    default:
                        throw new ArgumentException("path", "Unknown version!");
                }
            }

            if (level != null)
                return level;
            else
                throw new Exception("No level found!");
        }

        #region ReaderV1
        private static GameObject[,] ReadLevelV1(StreamReader reader)
        {
            // Read the dimensions and create the level array
            string[] dimensions = reader.ReadLine().Split(new string[] { LevelWriter.DIFFERENTIATOR }, StringSplitOptions.None);
            GameObject[,] level = new GameObject[int.Parse(dimensions[0]), int.Parse(dimensions[1])];

            // Read all the gameobjects
            while (true)
            {
                string nextLine = reader.ReadLine();
                if (String.IsNullOrEmpty(nextLine))
                    break;

                ReadGameObject(nextLine);
            }

            return level;

            void ReadGameObject(string dataLine)
            {
                string[] informationArray = dataLine.Split(new string[] { LevelWriter.DIFFERENTIATOR }, StringSplitOptions.None);
                Vector2D gameObjectPosition = new Vector2D(int.Parse(informationArray[1]), int.Parse(informationArray[2]));

                if (informationArray[0] == typeof(Player).ToString())
                    level[gameObjectPosition.X, gameObjectPosition.Y] = new Player(level, new Vector2D(gameObjectPosition.X, gameObjectPosition.Y));
                else if (informationArray[0] == typeof(Wall).ToString())
                    level[gameObjectPosition.X, gameObjectPosition.Y] = new Wall(level, new Vector2D(gameObjectPosition.X, gameObjectPosition.Y));
                else if (informationArray[0] == typeof(Empty).ToString())
                    level[gameObjectPosition.X, gameObjectPosition.Y] = ReadEmptyV1(level, gameObjectPosition, informationArray);
                // Ghost types
                else
                {
                    Vector2D playerStartingPosition = new Vector2D(int.Parse(informationArray[3]), int.Parse(informationArray[4]));
                    if (informationArray[0] == typeof(Blinky).ToString())
                        level[gameObjectPosition.X, gameObjectPosition.Y] = new Blinky(level, gameObjectPosition, playerStartingPosition);
                    else if (informationArray[0] == typeof(Pinky).ToString())
                        level[gameObjectPosition.X, gameObjectPosition.Y] = new Pinky(level, gameObjectPosition, playerStartingPosition);
                    else if (informationArray[0] == typeof(Inky).ToString())
                        level[gameObjectPosition.X, gameObjectPosition.Y] = new Inky(level, gameObjectPosition, playerStartingPosition);
                    else if (informationArray[0] == typeof(Clyde).ToString())
                        level[gameObjectPosition.X, gameObjectPosition.Y] = new Clyde(level, gameObjectPosition, playerStartingPosition);
                }
                    

            }
        }

        private static Empty ReadEmptyV1(GameObject[,] level, Vector2D emptyPosition, string[] informationArray)
        {
            bool containsCoin = bool.Parse(informationArray[3]);
            bool containsBoost = bool.Parse(informationArray[4]);

            return new Empty(level, emptyPosition, containsCoin, containsBoost);
        }
        #endregion

    }
}
