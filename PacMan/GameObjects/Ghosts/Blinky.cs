using System;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Type of ghost that tries to directly follow Pac-Man from behind
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

            // If inverted move, ghosts allways aim at player
            if (InvertedMove)
            {
                AimAtPlayer();
                return;
            }

            AimBehindPlayer();
        }

        protected override void Draw()
        {
            if (Style == DrawingStyle.ChasingGhosts)
                Console.ForegroundColor = GameColors.ChasingGhostMainColor;
            else if (Style == DrawingStyle.ChasingGhostsAlternate)
                Console.ForegroundColor = GameColors.ChasingGhostAlternateColor;
            else
                Console.ForegroundColor = GameColors.BlinkyColor;
            Console.BackgroundColor = GameColors.EmptyColor;

            Console.Write(APPEARANCE);
        }
        #endregion
    }
}
