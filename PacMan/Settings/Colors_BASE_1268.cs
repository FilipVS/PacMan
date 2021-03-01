using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Static class containing all color settings
    /// </summary>
    internal static class Colors
    {
        public static ConsoleColor EmptyColor { get; } = ConsoleColor.Black;
        public static ConsoleColor CoinColor { get; } = ConsoleColor.Green;
        public static ConsoleColor BoostColor { get; } = ConsoleColor.Green;

        public static ConsoleColor WallColor { get; } = ConsoleColor.DarkBlue;

        public static ConsoleColor PlayerColor { get; } = ConsoleColor.Yellow;

        public static ConsoleColor BlinkyColor { get; } = ConsoleColor.Red;
        public static ConsoleColor PinkyColor { get; } = ConsoleColor.Magenta;
        public static ConsoleColor InkyColor { get; } = ConsoleColor.Cyan;
        public static ConsoleColor ClydeColor { get; } = ConsoleColor.White;
    }
}
