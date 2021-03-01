using System;
using System.Collections.Generic;
using System.Text;
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
            BlinkyChoose();

            if (Position.DistanceTo(PlayerPositionThisTurn) < MINIMAL_DISTANCE_FROM_PLAYER)
                InvertedMove = true;
            else
                InvertedMove = false;

            void BlinkyChoose()
            {
                base.ChooseDesiredTile();

                // In order to follow player from behind he aims initially to the tile that player came from and
                // switches directly to the tile with player when close
                if (Position.DistanceTo(PlayerPositionThisTurn) > 1)
                    DesiredTile = PlayerPositionLastTurn;
                else
                    DesiredTile = PlayerPositionThisTurn;
            }
        }

        protected override void Draw()
        {
            Console.ForegroundColor = Colors.ClydeColor;
            Console.BackgroundColor = Colors.EmptyColor;

            Console.Write(APPEARANCE);
        }
        #endregion
    }
}
