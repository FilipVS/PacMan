using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Base class for all movable objects in the game (ghosts, player)
    /// </summary>
    internal abstract class MovableObject : GameObject
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        public MovableObject(GameObject[,] level, Vector2D startingPosition) : base(level, startingPosition)
        {
            spawnPoint = startingPosition.Copy();
        }


        #region Fields
        /// <summary>
        /// Determines which way the objects intends to go on the next update
        /// </summary>
        private Direction heading;

        private Vector2D spawnPoint;
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

        protected Vector2D SpawnPoint
        {
            get
            {
                return spawnPoint;
            }
        }
        #endregion

        #region Methods
        public abstract MoveResult Move();
        #endregion
    }

    /// <summary>
    /// MovableObjects use this enum to specify which way the intend to move
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// The enum that is returned by the Move method, signalize the result of the move
    /// </summary>
    public enum MoveResult
    {
        None,
        Boost,
        Collision
    }
}
