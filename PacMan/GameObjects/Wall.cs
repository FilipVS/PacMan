using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    class Wall : GameObject
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="position">Positiong of the GameObject in the level</param>
        public Wall(GameObject[] level, Vector2D position) : base(level, position, ' ')
        {

        }

        #region Methods
        public override void Draw()
        {
            // TODO: Fininish

            throw new NotImplementedException();
        }
        #endregion
    }
}
