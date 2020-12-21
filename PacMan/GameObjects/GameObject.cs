using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Base class for all uncollectable objects in the game (walls, ghosts, player...)
    /// </summary>
    abstract class GameObject
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="appearance">Char that is drawn out in the console to represent the GameObject</param>
        public GameObject(GameObject[] level, Vector2D startingPosition, char appearance)
        {
            Appearance = appearance;
            Level = level;
            Position = startingPosition;
        }

        #region Fields
        /// <summary>
        /// Represents the GameObjetc's position in the level
        /// </summary>
        private Vector2D position;

        /// <summary>
        /// Char that is drawn out in the console to represent the GameObject
        /// </summary>
        private char appearance;


        /// <summary>
        /// The level that the GameObject is associated with
        /// </summary>
        private GameObject[] level;
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

        protected char Appearance
        {
            get
            {
                return appearance;
            }
            private set
            {
                appearance = value;
            }
        }

        protected GameObject[] Level
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
        /// Each GameObject will draw itself
        /// </summary>
        public abstract void Draw();
        #endregion
    }
}
