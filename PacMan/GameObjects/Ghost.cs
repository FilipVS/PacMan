using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Base class for all ghosts
    /// Contains maze-solving logic (it is the same for all ghosts)
    /// </summary>
    abstract class Ghost : MovableObject
    {
        /// <summary>
        /// Creates a new instance of Ghost with the appearance set to &
        /// </summary>
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        public Ghost(GameObject[] level, Vector2D startingPosition) : base(level, startingPosition, '&')
        {
        }

        #region Methods
        /// <summary>
        /// Determine which way the Ghost wants to go on the next update
        /// </summary>
        public abstract void DetermineHeading();
        #endregion
    }
}