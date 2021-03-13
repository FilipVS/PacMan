using System;

namespace Setnicka.PacMan
{
    internal static class GameKeyBinding
    {
        public const ConsoleKey MoveUp = ConsoleKey.W;
        public const ConsoleKey MoveUpSecondary = ConsoleKey.UpArrow;

        public const ConsoleKey MoveDown = ConsoleKey.S;
        public const ConsoleKey MoveDownSecondary = ConsoleKey.DownArrow;

        public const ConsoleKey MoveLeft = ConsoleKey.A;
        public const ConsoleKey MoveLeftSecondary = ConsoleKey.LeftArrow;

        public const ConsoleKey MoveRight = ConsoleKey.D;
        public const ConsoleKey MoveRightSecondary = ConsoleKey.RightArrow;

        public const ConsoleKey GoToMenu = ConsoleKey.Escape;

        public const ConsoleKey Refresh = ConsoleKey.R;
    }
}
