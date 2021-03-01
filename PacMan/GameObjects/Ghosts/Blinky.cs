using System;
using System.Collections.Generic;
using System.Text;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Type of ghost that tries to directly follow Pac-Man
    /// </summary>
    internal class Blinky : Ghost
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="playerStartingPosition">The starting position of the player within the level</param>
        public Blinky(GameObject[,] level, Vector2D startingPosition, Vector2D playerStartingPosition) : base(level, startingPosition, playerStartingPosition)
        {
        }


        #region Methods
        protected override void ChooseDesiredTile()
        {
            base.ChooseDesiredTile();

            // In order to follow player from behind he aims initially to the tile that player came from and
            // switches directly to the tile with player when close
            if (Position.DistanceTo(PlayerPositionThisTurn) > 1)
                DesiredTile = PlayerPositionLastTurn;
            else
                DesiredTile = PlayerPositionThisTurn;
        }

        protected override void Draw()
        {
            Console.ForegroundColor = Colors.BlinkyColor;
            Console.BackgroundColor = Colors.EmptyColor;

            Console.Write(APPEARANCE);
        }
        #endregion
    }
}
