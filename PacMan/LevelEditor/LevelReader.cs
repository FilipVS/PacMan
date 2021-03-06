﻿using System;
using System.IO;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan.LevelEditor
{
    /// <summary>
    /// Used for loading levels from files
    /// </summary>
    static class LevelReader
    {
        /// <summary>
        /// Reads the level and returns it in an GameObject array
        /// </summary>
        /// <param name="path">Path to the level</param>
        /// <returns>The level as GameObject array, in case of problem it returns null</returns>
        public static GameObject[,] ReadLevel(string path)
        {
            if (String.IsNullOrEmpty(path))
                return null;
            if (!File.Exists(path))
                return null;

            GameObject[,] level;

            using(StreamReader reader = new StreamReader(path))
            {
                // Confirm that the header text is present
                if (reader.ReadLine() != LevelWriter.HEADER_TEXT)
                    return null;

                // Read the version number and use appropriate reader for it
                string version = reader.ReadLine();
                switch (version)
                {
                    case LevelWriter.VERSION_1_STRING:
                        level = ReadLevelV1(reader);
                        break;
                    default:
                        return null;
                }
            }

            return level;
        }

        #region ReaderV1
        private static GameObject[,] ReadLevelV1(StreamReader reader)
        {
            // Read the dimensions and create the level array
            string[] dimensions = reader.ReadLine().Split(new string[] { LevelWriter.DIFFERENTIATOR }, StringSplitOptions.None);
            GameObject[,] level = new GameObject[int.Parse(dimensions[0]), int.Parse(dimensions[1])];

            // Initialize all tiles to be empty
            for(int x = 0; x < level.GetLength(0); x++)
            {
                for(int y = 0; y < level.GetLength(1); y++)
                {
                    level[x, y] = new Empty(level, new Vector2D(x, y), false, false);
                }
            }

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
                // The data line contains all the information needed to create new GameObject, so we split it from a line of text into chunks of valid informations
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


        /// <summary>
        /// Checks, whether a given path is openable
        /// </summary>
        public static bool FileOpenable(string path)
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

    }
}
