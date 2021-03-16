using System;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Static class containing all game color settings
    /// </summary>
    internal static class GameColors
    {
        // ChasingGhosts and ChasingGhostsMainVersion are used to alter the colors, that are returned for individual ghosts
        // Return normal ghost colors or their chasing ghosts version
        public static bool ChasingGhosts { get; set; } = false;
        // Return chasing ghost main version color or alternate?
        public static bool ChasingGhostsMainVersion { get; set; } = true;


        public static ConsoleColor EmptyColor { get; set; } = ConsoleColor.Black;
        public static ConsoleColor CoinColor { get; } = ConsoleColor.Green;
        public static ConsoleColor BoostColor { get; } = ConsoleColor.Green;

        public static ConsoleColor WallColor { get; set; } = ConsoleColor.Blue;

        public static ConsoleColor PlayerColor { get; } = ConsoleColor.Yellow;

        public static ConsoleColor BlinkyColor
        {
            get
            {
                if(ChasingGhosts)
                {
                    if (ChasingGhostsMainVersion)
                        return ChasingGhostMainColor;
                    return ChasingGhostAlternateColor;
                }
                return ConsoleColor.Red;
            }
        }
        public static ConsoleColor PinkyColor
        {
            get
            {
                if (ChasingGhosts)
                {
                    if (ChasingGhostsMainVersion)
                        return ChasingGhostMainColor;
                    return ChasingGhostAlternateColor;
                }
                return ConsoleColor.Magenta;
            }
        }
        public static ConsoleColor InkyColor
        {
            get
            {
                if (ChasingGhosts)
                {
                    if (ChasingGhostsMainVersion)
                        return ChasingGhostMainColor;
                    return ChasingGhostAlternateColor;
                }
                return ConsoleColor.Cyan;
            }
        }
        public static ConsoleColor ClydeColor
        {
            get
            {
                if (ChasingGhosts)
                {
                    if (ChasingGhostsMainVersion)
                        return ChasingGhostMainColor;
                    return ChasingGhostAlternateColor;
                }
                return ConsoleColor.White;
            }
        }


        public static ConsoleColor ChasingGhostMainColor { get; } = ConsoleColor.DarkCyan;
        public static ConsoleColor ChasingGhostAlternateColor { get; } = ConsoleColor.White;


        public static ConsoleColor LevelBorderColor { get; } = ConsoleColor.DarkMagenta;
    }
}
