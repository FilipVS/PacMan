using System;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Static class containing all game color settings
    /// </summary>
    internal static class GameColors
    {
        public static ConsoleColor EmptyColor { get; set; } = ConsoleColor.Black;
        public static ConsoleColor CoinColor { get; } = ConsoleColor.Green;
        public static ConsoleColor BoostColor { get; } = ConsoleColor.Green;

        public static ConsoleColor WallColor { get; set; } = ConsoleColor.Blue;

        public static ConsoleColor PlayerColor { get; } = ConsoleColor.Yellow;

        public static ConsoleColor BlinkyColor
        {
            get
            {
                return ConsoleColor.Red;
            }
        }
        public static ConsoleColor PinkyColor
        {
            get
            {
                return ConsoleColor.Magenta;
            }
        }
        public static ConsoleColor InkyColor
        {
            get
            {
                return ConsoleColor.Cyan;
            }
        }
        public static ConsoleColor ClydeColor
        {
            get
            {
                return ConsoleColor.White;
            }
        }


        public static ConsoleColor ChasingGhostMainColor { get; } = ConsoleColor.DarkCyan;
        public static ConsoleColor ChasingGhostAlternateColor { get; } = ConsoleColor.White;


        public static ConsoleColor LevelBorderColor { get; } = ConsoleColor.DarkMagenta;
    }
}
