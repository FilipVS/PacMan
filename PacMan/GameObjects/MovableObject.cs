using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Base class for all movable objects in the game (ghosts, player)
    /// </summary>
    abstract class MovableObject : GameObject
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="appearance">Char that is drawn out in the console to represent the GameObject</param>
        public MovableObject(GameObject[] level, Vector2D startingPosition, char appearance) : base(level, startingPosition, appearance)
        {
        }

        #region Fields
        /// <summary>
        /// Determines which way the objects intends to go on the next update
        /// </summary>
        private Direction heading;
        #endregion

        #region Properties
        public Direction Heading
        {
            get
            {
                return heading;
            }
            protected set
            {
                heading = value;
            }
        }
        #endregion

        #region Methods
        // TODO: Decide whether virtual/abstract
        public virtual void Move() { }
        #endregion
    }

    /// <summary>
    /// MovableObjects use this enum to specify which way the intend to move
    /// </summary>
    public enum Direction
    {
        Top,
        Down,
        Left,
        Right
    }
}
