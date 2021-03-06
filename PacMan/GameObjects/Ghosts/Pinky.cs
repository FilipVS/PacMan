﻿using System;
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

            // If inverted move, ghosts always aim at player
            if (InvertedMove)
            {
                AimAtPlayer();
                return;
            }

            AimInFrontOfPlayer();
        }

        protected override void Draw()
        {
            if (Style == DrawingStyle.ChasingGhosts)
                Console.ForegroundColor = GameColors.ChasingGhostMainColor;
            else if (Style == DrawingStyle.ChasingGhostsAlternate)
                Console.ForegroundColor = GameColors.ChasingGhostAlternateColor;
            else
                Console.ForegroundColor = GameColors.PinkyColor;
            Console.BackgroundColor = GameColors.EmptyColor;

            Console.Write(APPEARANCE);
        }
        #endregion

    }
}
