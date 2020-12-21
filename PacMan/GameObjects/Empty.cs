using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Type of GameObject that player and ghosts can move across (it can as well store collectible objetcs - coins and boosts)
    /// Each Empty GameObject can store only one collectible object (a coin or a boost)
    /// </summary>
    class Empty : GameObject
    {
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="position">Positiong of the GameObject in the level</param>
        public Empty(GameObject[] level, Vector2D position) : base(level, position, ' ')
        {
        }

        #region Fields
        private bool containsCoin;
        private bool containsBoost;


        #endregion

        #region Properties
        public bool ContainsCoin
        {
            get
            {
                return containsCoin;
            }
            set
            {
                // TODO: If value changed, add/remove a coin

                if (value && ContainsBoost)
                    ContainsBoost = false;

                containsCoin = value;
            }
        }

        public bool ContainsBoost
        {
            get
            {
                return containsBoost;
            }
            set
            {
                // TODO: If value changed, add/remove a boost

                if (value && ContainsCoin)
                    ContainsCoin = false;

                containsBoost = value;
            }
        }
        #endregion

        #region Methods
        public override void Draw()
        {
            // TODO: Finish

            throw new NotImplementedException();
        }
        #endregion
    }
}
