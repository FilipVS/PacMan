using System;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Type of GameObject that player and ghosts can move across (it can as well store collectable objetcs - coins and boosts)
    /// Each Empty GameObject can store only one collectable object (a coin or a boost)
    /// </summary>
    internal class Empty : GameObject
    {
        private const char APPEARANCE_EMPTY = ' ';
        private const char APPEARANCE_WITH_COIN = '-';
        private const char APPEARANCE_WITH_BOOST = 'O';

        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="position">Positiong of the GameObject in the level</param>
        /// <param name="containsCoin">Determines if the Empty GameObject contains coin (can only hold coin or boost)</param>
        /// <param name="containsBoost">Determines if the Empty GameObject contains boost (can only hold coin or boost)</param>
        public Empty(GameObject[,] level, Vector2D position, bool containsCoin = false, bool containsBoost = false) : base(level, position)
        {
            if (containsCoin && containsBoost)
                throw new ArgumentException("Cannot hold both coin and boost!");

            ContainsCoin = containsCoin;
            ContainsBoost = containsBoost;
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
                if (value && ContainsCoin)
                    ContainsCoin = false;

                containsBoost = value;
            }
        }
        #endregion

        #region Methods
        protected override void Draw()
        {
            Console.BackgroundColor = GameColors.EmptyColor;

            if (ContainsBoost)
            {
                Console.ForegroundColor = GameColors.BoostColor;
                Console.Write(APPEARANCE_WITH_BOOST);
            }
            else if (ContainsCoin)
            {
                Console.ForegroundColor = GameColors.CoinColor;
                Console.Write(APPEARANCE_WITH_COIN);
            }
            else
            {
                Console.ForegroundColor = GameColors.EmptyColor;
                Console.Write(APPEARANCE_EMPTY);
            }
        }
        #endregion
    }
}
