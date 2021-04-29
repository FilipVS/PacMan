using Setnicka.AuxiliaryClasses;

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
            SpawnPoint = startingPosition.Copy();
        }

        #region Properties
        public Direction Heading { get; protected set; }

        public Vector2D SpawnPoint { get; }
        #endregion

        #region Methods
        public abstract MoveResult Move();
        #endregion
    }

    /// <summary>
    /// The enum that is returned by the Move method, signalizes the result of the movement
    /// </summary>
    public enum MoveResult
    {
        None,
        Boost,
        Coin,
        Collision
    }
}
