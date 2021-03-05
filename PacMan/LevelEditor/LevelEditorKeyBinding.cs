using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.PacMan.LevelEditor
{
    static class LevelEditorKeyBinding
    {
        // TODO: Finish (make into IEnumerable?)

        public const ConsoleKey LevelPlaneUp = ConsoleKey.W;
        public const ConsoleKey LevelPlaneDown = ConsoleKey.S;
        public const ConsoleKey LevelPlaneLeft = ConsoleKey.A;
        public const ConsoleKey LevelPlaneRight = ConsoleKey.D;

        public const ConsoleKey ObjectOfChoiceUp = ConsoleKey.UpArrow;
        public const ConsoleKey ObjectOfChoiceDown = ConsoleKey.DownArrow;

        public const ConsoleKey PlaceObject = ConsoleKey.Enter;

        public const ConsoleKey DeleteObject = ConsoleKey.Delete;

        public const ConsoleKey GoToMenu = ConsoleKey.Escape;
    }
}
