using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.PacMan
{
    static class GameKeyBinding
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
    }
}
