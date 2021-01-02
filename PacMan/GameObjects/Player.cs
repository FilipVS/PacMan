using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    class Player : MovableObject
    {
        private const char APPEARANCE = '@';
        private const int STARTING_HEALTH = 3;

        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        public Player(GameObject[,] level, Vector2D startingPosition) : base(level, startingPosition)
        {
            Health = STARTING_HEALTH;
        }


        #region Fields
        private int health;

        private int score;
        #endregion

        #region Properties
        public int Health
        {
            get
            {
                return health;
            }
            private set
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
        #endregion

        #region Methods
        protected override void Draw()
        {
            Console.ForegroundColor = Colors.PlayerColor;
            Console.BackgroundColor = Colors.EmptyColor;

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
                    Score++;

                bool containsBoost = empty.ContainsBoost;

                // Move the player and redraw the tiles
                Level[Position.X, Position.Y] = new Empty(Level, Position);
                Level[Position.X, Position.Y].Print();
                Position = moveToTile;
                Level[Position.X, Position.Y] = this;
                Level[Position.X, Position.Y].Print();

                if (containsBoost)
                    return MoveResult.Boost;
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
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    Heading = Direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    Heading = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    Heading = Direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
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
