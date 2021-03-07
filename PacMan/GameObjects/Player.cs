using System;
using System.Collections.Generic;
using System.Text;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    internal class Player : MovableObject
    {
        private const char APPEARANCE = '@';
        private const int STARTING_HEALTH = 3;

        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        public Player(GameObject[,] level, Vector2D startingPosition) : base(level, startingPosition)
        {
            Health = STARTING_HEALTH;
        }

        public Player(GameObject[,] level, Vector2D startingPosition, int health, int score) : this(level, startingPosition)
        {
            Health = health;
            Score = score;
        }

        #region Event
        public event EventHandler GameWon;
        #endregion

        #region Fields
        private int health;

        private int score;

        // -1 means that coins weren't counted yet
        private int availableCoins = -1;
        #endregion

        #region Properties
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
            }
        }

        public int Score
        {
            get
            {
                return score;
            }
            private set
            {
                score = value;
            }
        }

        private int AvailableCoins
        {
            get
            {
                // If coins weren't counted yet, count them
                if (availableCoins == -1)
                    CountAvailableCoins();

                return availableCoins;
            }
            set
            {
                availableCoins = value;
            }
        }
        #endregion

        #region Methods
        protected override void Draw()
        {
            Console.ForegroundColor = GameColors.PlayerColor;
            Console.BackgroundColor = GameColors.EmptyColor;

            Console.Write(APPEARANCE);
        }

        public override MoveResult Move()
        {
            Vector2D moveToTile = Position + (new Vector2D(Heading));

            // If the destination tile is outside of the level
            if (Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), moveToTile))
                return MoveResult.None;

            if(Level[moveToTile.X, moveToTile.Y] is Empty empty)
            {
                if (empty.ContainsCoin)
                {
                    Score++;
                    if (Score == AvailableCoins)
                        GameWon(this, EventArgs.Empty);
                }

                bool containsBoost = empty.ContainsBoost;
                bool containsCoin = empty.ContainsCoin;

                // Move the player and redraw the tiles
                Level[Position.X, Position.Y] = new Empty(Level, Position);
                Level[Position.X, Position.Y].Print(GameManager.OFFSET);
                Position = moveToTile;
                Level[Position.X, Position.Y] = this;
                Level[Position.X, Position.Y].Print(GameManager.OFFSET);

                if (containsBoost)
                    return MoveResult.Boost;
                else if (containsCoin)
                    return MoveResult.Coin;
            }
            else if(Level[moveToTile.X, moveToTile.Y] is Ghost ghost)
            {
                return MoveResult.Collision;
            }

            return MoveResult.None;
        }

        public void ChangeHeading(object sender, KeyEventArgs keyEventArgs)
        {
            Direction originalHeading = Heading;

            switch (keyEventArgs.keyPressed)
            {
                case GameKeyBinding.MoveUp:
                case GameKeyBinding.MoveUpSecondary:
                    Heading = Direction.Up;
                    break;
                case GameKeyBinding.MoveDown:
                case GameKeyBinding.MoveDownSecondary:
                    Heading = Direction.Down;
                    break;
                case GameKeyBinding.MoveLeft:
                case GameKeyBinding.MoveLeftSecondary:
                    Heading = Direction.Left;
                    break;
                case GameKeyBinding.MoveRight:
                case GameKeyBinding.MoveRightSecondary:
                    Heading = Direction.Right;
                    break;
                default:
                    break;
            }

            // Player can't stop by turning into a wall
            try
            {
                Vector2D headingPosition = Position + (new Vector2D(Heading));
                if (Level[headingPosition.X, headingPosition.Y] is Wall)
                    Heading = originalHeading;
            }
            catch (IndexOutOfRangeException)
            {
                Heading = originalHeading;
            }
        }

        private void CountAvailableCoins()
        {
            AvailableCoins = (Score - 1);

            foreach (GameObject gameObject in Level)
            {
                if (gameObject is Empty empty && empty.ContainsCoin)
                    AvailableCoins++;
            }
        }
        #endregion
    }
}
