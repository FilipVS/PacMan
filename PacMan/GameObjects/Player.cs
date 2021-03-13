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

            DesiredTile = startingPosition.Copy();
        }

        public Player(GameObject[,] level, Vector2D startingPosition, int health) : this(level, startingPosition)
        {
            Health = health;
        }

        #region Fields
        private int health;
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

        // Where did the player want to go previously
        public Vector2D DesiredTile { get; private set; }
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
            // If the destination tile is outside of the level
            if (Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), (Position + new Vector2D(Heading))))
                return MoveResult.None;

            DesiredTile = Position + (new Vector2D(Heading));

            if (Level[DesiredTile.X, DesiredTile.Y] is Empty empty)
            {
                bool containsBoost = empty.ContainsBoost;
                bool containsCoin = empty.ContainsCoin;

                // Move the player and redraw the tiles
                Level[Position.X, Position.Y] = new Empty(Level, Position);
                Level[Position.X, Position.Y].Print(GameManager.OFFSET);
                Position = DesiredTile;
                Level[Position.X, Position.Y] = this;
                Level[Position.X, Position.Y].Print(GameManager.OFFSET);

                if (containsBoost)
                    return MoveResult.Boost;
                else if (containsCoin)
                    return MoveResult.Coin;
            }
            else if(Level[DesiredTile.X, DesiredTile.Y] is Ghost ghost)
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
        #endregion
    }
}
