using System;
using System.Collections.Generic;
using System.Text;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Type of ghost that tries to ambush Pac-Man (tries to position itself in front of Pac-Man)
    /// </summary>
    internal class Pinky : Ghost
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="playerStartingPosition">The starting position of the player within the level</param>
        public Pinky(GameObject[,] level, Vector2D startingPosition, Vector2D playerStartingPosition) : base(level, startingPosition, playerStartingPosition)
        {
        }


        #region Methods
        protected override void ChooseDesiredTile()
        {
            base.ChooseDesiredTile();

            Vector2D playerHeading = PlayerPositionThisTurn - PlayerPositionLastTurn;

            Vector2D desiredTile = PlayerPositionThisTurn + playerHeading;

            // Tries to aim to position that the player is heading towards
            if(!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), desiredTile))
                if(!(Level[desiredTile.X, desiredTile.Y] is Wall))
                {
                    DesiredTile = desiredTile;
                    base.ChooseDesiredTile();
                    return;
                }

            // If that position is unachievable, he aims directly for the player
            DesiredTile = PlayerPositionThisTurn;
        }

        protected override void Draw()
        {
            Console.ForegroundColor = GameColors.PinkyColor;
            Console.BackgroundColor = GameColors.EmptyColor;

            Console.Write(APPEARANCE);
        }
        #endregion

    }
}
