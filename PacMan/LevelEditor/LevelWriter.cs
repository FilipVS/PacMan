using System;
using System.IO;

namespace Setnicka.PacMan.LevelEditor
{
    /// <summary>
    /// Used for saving levels to files
    /// </summary>
    static class LevelWriter
    {
        // The reader checks, whether this text is present at the beggining of a file
        public const string HEADER_TEXT = "PacManSetnickaLevel";
        // This string is used to separate individual information on a line
        public const string DIFFERENTIATOR = "NEXT";
        // This string announces that the file was written with V1 Writer
        public const string VERSION_1_STRING = "V1";

        public static void SaveLevel(GameObject[,] level, string path)
        {
            if (level == null)
                throw new ArgumentNullException("level", "level parametr cannot be null!");
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path", "path argument cannot be null or empty!");

            SaveLevelV1(level, path);
        }


        #region SaverV1
        private static void SaveLevelV1(GameObject[,] level, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(HEADER_TEXT);

                // Write the version, so the reader knows, how to proceed
                writer.WriteLine(VERSION_1_STRING);

                // Write the dimensions of the level
                writer.WriteLine($"{level.GetLength(0)}{DIFFERENTIATOR}{level.GetLength(1)}");

                // Save all objects
                foreach (GameObject gameObject in level)
                {
                    SaveGameObject(gameObject, writer);
                }
            }

            void SaveGameObject(GameObject gameObject, StreamWriter writer)
            {
                switch (gameObject)
                {
                    case Player _:
                        SavePlayerV1(gameObject as Player, writer);
                        break;
                    case Wall _:
                        SaveWallV1(gameObject as Wall, writer);
                        break;
                    // Don't save completely empty tiles
                    case Empty empty when (empty.ContainsBoost || empty.ContainsCoin):
                        SaveEmtpyV1(gameObject as Empty, writer);
                        break;
                    case Ghost _:
                        SaveGhostV1(gameObject as Ghost, gameObject.GetType(), writer);
                        break;
                }
            }
        }

        private static void SavePlayerV1(Player player, StreamWriter writer)
        {
            writer.WriteLine($"{player.GetType().ToString()}{DIFFERENTIATOR}{player.Position.X}{DIFFERENTIATOR}{player.Position.Y}");
        }

        private static void SaveWallV1(Wall wall, StreamWriter writer)
        {
            writer.WriteLine($"{wall.GetType().ToString()}{DIFFERENTIATOR}{wall.Position.X}{DIFFERENTIATOR}{wall.Position.Y}");
        }

        private static void SaveEmtpyV1(Empty empty, StreamWriter writer)
        {
            writer.WriteLine($"{empty.GetType().ToString()}{DIFFERENTIATOR}{empty.Position.X}{DIFFERENTIATOR}{empty.Position.Y}{DIFFERENTIATOR}{empty.ContainsCoin}{DIFFERENTIATOR}{empty.ContainsBoost}");
        }

        private static void SaveGhostV1(Ghost ghost, Type ghostType, StreamWriter writer)
        {
            writer.WriteLine($"{ghostType.ToString()}{DIFFERENTIATOR}{ghost.Position.X}{DIFFERENTIATOR}{ghost.Position.Y}{DIFFERENTIATOR}{ghost.PlayerPositionThisTurn.X}{DIFFERENTIATOR}{ghost.PlayerPositionThisTurn.Y}");
        }
        #endregion


        /// <summary>
        /// Returns the directory, where the levels are located
        /// </summary>
        public static string GetPathToLevels()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string[] pathParts = baseDirectory.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            string levelsDerectory = String.Empty;

            // If the app is running from debug
            if (pathParts[pathParts.Length - 1] == "Debug" && pathParts[pathParts.Length - 2] == "bin")
                levelsDerectory = (baseDirectory.Remove((baseDirectory.Length - $"bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}".Length))) + $"Levels{Path.DirectorySeparatorChar}";
            // The app is published
            else
                levelsDerectory = baseDirectory + $"Levels{Path.DirectorySeparatorChar}";

            return levelsDerectory;
        }

        /// <summary>
        /// Checks whether a file can be created at a certain path
        /// </summary>
        public static bool FileCreatable(string path)
        {
            bool fileCreatable = false;

            // Try to save find/create the file
            try
            {
                File.Create(path).Dispose();
                File.Delete(path);
                fileCreatable = true;
            }
            catch (UnauthorizedAccessException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (DirectoryNotFoundException) { }
            catch (NotSupportedException) { }
            catch (IOException) { }

            return fileCreatable;
        }
    }
}