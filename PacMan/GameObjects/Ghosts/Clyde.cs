using System;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Type of ghost that chases Pac-Man like Blinky, but when he gets too close, he turns around
    /// </summary>
    internal class Clyde : Ghost
    {
        private const int MINIMAL_DISTANCE_FROM_PLAYER = 4;

        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="playerStartingPosition">The starting position of the player within the level</param>
        public Clyde(GameObject[,] level, Vector2D startingPosition, Vector2D playerStartingPosition) : base(level, startingPosition, playerStartingPosition)
        {
        }


        #region Methods
        protected override void ChooseDesiredTile()
        {
            base.ChooseDesiredTile();

            // If inverted move, ghosts always aim at player
            if (InvertedMove)
            {
                AimAtPlayer();
                return;
            }

            AimBehindPlayer();

            if (Position.DistanceTo(PlayerPositionThisTurn) < MINIMAL_DISTANCE_FROM_PLAYER)
                InvertedMove = true;
        }

        protected override void Draw()
        {
            if (Style == DrawingStyle.ChasingGhosts)
                Console.ForegroundColor = GameColors.ChasingGhostMainColor;
            else if (Style == DrawingStyle.ChasingGhostsAlternate)
                Console.ForegroundColor = GameColors.ChasingGhostAlternateColor;
            else
                Console.ForegroundColor = GameColors.ClydeColor;
            Console.BackgroundColor = GameColors.EmptyColor;

            Console.Write(APPEARANCE);
        }
        #endregion
    }
}
