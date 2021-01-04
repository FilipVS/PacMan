using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Base class for all uncollectable objects in the game (walls, ghosts, player...)
    /// </summary>
    internal abstract class GameObject
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        public GameObject(GameObject[,] level, Vector2D startingPosition)
        {
            Level = level;
            Position = startingPosition;
        }

        #region Fields
        /// <summary>
        /// Represents the GameObjetc's position in the level
        /// </summary>
        private Vector2D position;


        /// <summary>
        /// The level that the GameObject is associated with
        /// </summary>
        private GameObject[,] level;
        #endregion

        #region Properties
        public Vector2D Position
        {
            get
            {
                return position;
            }
            protected set
            {
                position = value;
            }
        }

        protected GameObject[,] Level
        {
            get
            {
                return level;
            }
            private set
            {
                level = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Preparing the cursor for drawing of the object (setting the cursor position)
        /// The process of drawing itself will be different for each child (via the Draw method)
        /// </summary>
        public void Print(Vector2D printOffset)
        {
            Vector2D printingPosition = Position + printOffset;
            Console.SetCursorPosition(printingPosition.X, printingPosition.Y);

            ConsoleColor originalForegroundColor = Console.ForegroundColor;
            ConsoleColor originalBackgroundColor = Console.BackgroundColor;

            Draw();

            Console.ForegroundColor = originalForegroundColor;
            Console.BackgroundColor = originalBackgroundColor;

            Console.CursorVisible = false;
        }

        /// <summary>
        /// The actual process of drawing something on the screen
        /// </summary>
        protected abstract void Draw();
        #endregion
    }
}
