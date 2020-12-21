using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    class Player : MovableObject
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        public Player(GameObject[] level, Vector2D startingPosition) : base(level, startingPosition, '@')
        {

        }



        #region Methods
        public override void Draw()
        {
            // TODO: Finish

            throw new NotImplementedException();
        }
        #endregion
    }
}
