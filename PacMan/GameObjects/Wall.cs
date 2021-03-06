using System;
using System.Collections.Generic;
using System.Text;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    internal class Wall : GameObject
    {
        private const char APPEARANCE = ' ';

        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="position">Positiong of the GameObject in the level</param>
        public Wall(GameObject[,] level, Vector2D position) : base(level, position)
        {

        }

        #region Methods
        protected override void Draw()
        {
            Console.ForegroundColor = GameColors.WallColor;
            Console.BackgroundColor = GameColors.WallColor;

            Console.Write(APPEARANCE);
        }
        #endregion
    }
}
