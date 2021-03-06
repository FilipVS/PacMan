using System;
using System.Collections.Generic;
using System.Text;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Base class for all ghosts
    /// Contains maze-solving logic (it is the same for all ghosts)
    /// </summary>
    internal abstract class Ghost : MovableObject
    { 
        protected const char APPEARANCE = '&';

        /// <summary>
        /// Creates a new instance of Ghost with the appearance set to &
        /// </summary>
        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="playerStartingPosition">The starting position of the player within the level</param>
        public Ghost(GameObject[,] level, Vector2D startingPosition, Vector2D playerStartingPosition) : base(level, startingPosition)
        {
            PlayerPositionLastTurn = playerStartingPosition;
            PlayerPositionThisTurn = playerStartingPosition;
        }


        #region Fields
        private int[,] mazeMap;

        private Vector2D playerPositionLastTurn;
        private Vector2D playerPositionThisTurn;

        // The Empty GameObject that the Ghost is currently standing on
        private Empty tileStanding;

        // Tile that the ghost wants to eventually reach
        private Vector2D desiredTile;

        private bool invertedMove = false;
        #endregion


        #region Properties
        public int[,] MazeMap
        {
            get
            {
                return mazeMap;
            }
            private set
            {
                mazeMap = value;
            }
        }

        private Empty TileStanding
        {
            get
            {
                return tileStanding;
            }
            set
            {
                tileStanding = value;
            }
        }

        protected Vector2D PlayerPositionLastTurn
        {
            get
            {
                return playerPositionLastTurn;
            }
            set
            {
                playerPositionLastTurn = value;
            }
        }

        public Vector2D PlayerPositionThisTurn
        {
            get
            {
                return playerPositionThisTurn;
            }
            protected set
            {
                playerPositionThisTurn = value;
            }
        }

        protected Vector2D DesiredTile
        {
            get
            {
                return desiredTile;
            }
            set
            {
                desiredTile = value;
            }
        }

        public bool InvertedMove
        {
            get
            {
                return invertedMove;
            }
            set
            {
                invertedMove = value;
            }
        }
        #endregion


        #region Methods
        /// <summary>
        /// Determine which way the Ghost wants to go on the next update
        /// </summary>
        private void DetermineHeading()
        {
            ChooseDesiredTile();

            SolveMaze();

            // Ghost doesn't want to go anywhere, choosing heading is arbitrary
            if (DesiredTile.Equals(Position))
            {
                Heading = Direction.Down;
                return;
            }

            // Backtrack MazeMap to determine on which tile the ghost wants to go
            Vector2D currentlyOn = DesiredTile.Copy();
            Vector2D checkedTile;
            int order = MazeMap[currentlyOn.X, currentlyOn.Y];

            while (true)
            {
                // Found the tile in path next to the ghost
                if (order == 1)
                    break;

                // TODO: Improve this mess (4x repeated code)
                checkedTile = currentlyOn + Vector2D.Up;
                if(!Vector2D.VectorOutOf2DArray(MazeMap.GetLength(0), MazeMap.GetLength(1), checkedTile))
                    if(MazeMap[checkedTile.X, checkedTile.Y] < order && MazeMap[checkedTile.X, checkedTile.Y] > 0)
                    {
                        order--;
                        currentlyOn = checkedTile;
                        continue;
                    }
                checkedTile = currentlyOn + Vector2D.Down;
                if (!Vector2D.VectorOutOf2DArray(MazeMap.GetLength(0), MazeMap.GetLength(1), checkedTile))
                    if (MazeMap[checkedTile.X, checkedTile.Y] < order && MazeMap[checkedTile.X, checkedTile.Y] > 0)
                    {
                        order--;
                        currentlyOn = checkedTile;
                        continue;
                    }
                checkedTile = currentlyOn + Vector2D.Right;
                if (!Vector2D.VectorOutOf2DArray(MazeMap.GetLength(0), MazeMap.GetLength(1), checkedTile))
                    if (MazeMap[checkedTile.X, checkedTile.Y] < order && MazeMap[checkedTile.X, checkedTile.Y] > 0)
                    {
                        order--;
                        currentlyOn = checkedTile;
                        continue;
                    }
                checkedTile = currentlyOn + Vector2D.Left;
                if (!Vector2D.VectorOutOf2DArray(MazeMap.GetLength(0), MazeMap.GetLength(1), checkedTile))
                    if (MazeMap[checkedTile.X, checkedTile.Y] < order && MazeMap[checkedTile.X, checkedTile.Y] > 0)
                    {
                        order--;
                        currentlyOn = checkedTile;
                        continue;
                    }
            }

            // Finally count the new Heading
            Vector2D positionDifference = currentlyOn - Position;
            if (positionDifference.X == 1)
            {
                if (positionDifference.Y == 0)
                    Heading = Direction.Right;
                else
                    DoNotMove();
                    //throw new ArgumentException("Incorrect result!");
            }
            else if(positionDifference.X == -1)
            {
                if (positionDifference.Y == 0)
                    Heading = Direction.Left;
                else
                    DoNotMove();
                    //throw new ArgumentException("Incorrect result!");
            }
            else if (positionDifference.X == 0)
            {
                if (positionDifference.Y == 1)
                    Heading = Direction.Down;
                else if (positionDifference.Y == -1)
                    Heading = Direction.Up;
                else
                    DoNotMove();
                    //throw new ArgumentException("Incorrect result!");
            }
            else
                DoNotMove();
                //throw new ArgumentException("Incorrect result!");

            // Sets the desired tile to be the same as current tile
            void DoNotMove()
            {
                DesiredTile = Position.Copy();
            }
        }

        /// <summary>
        /// Maze solving algorhytm - the way ghosts navigate (finds the shortest way to the desired tile)
        /// The algorhytm is rating tiles in circles (circle of tiles 1 move away, 2 moves away..) until it finds the desired tile
        /// </summary>
        private void SolveMaze()
        {
            if (Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), DesiredTile))
                throw new ArgumentOutOfRangeException("desiredTile", "desiredTile is outside of the level!");

            // Reset MazeMap
            MazeMap = new int[Level.GetLength(0), Level.GetLength(1)];

            HashSet<GameObject> nextOrderTiles = new HashSet<GameObject>(new List<GameObject>() { Level[Position.X, Position.Y] });
            HashSet<GameObject> thisOrderTiles;
            int order = 0;

            while (nextOrderTiles.Count > 0)
            {
                thisOrderTiles = nextOrderTiles;
                nextOrderTiles = new HashSet<GameObject>();

                foreach(GameObject tile in thisOrderTiles)
                {
                    MazeMap[tile.Position.X, tile.Position.Y] = order;

                    if (tile.Position.X == DesiredTile.X && tile.Position.Y == DesiredTile.Y)
                        return;

                    foreach (GameObject tile2 in GetMovableTilesAround(tile.Position))
                    {
                        if(MazeMap[tile2.Position.X, tile2.Position.Y] == 0 && !tile2.Equals(Position))
                            nextOrderTiles.Add(tile2);
                    }
                }

                order++;
            }

            // If the method reached this point, it didn't find any valid way to the desired point, so the ghost just stays at the same spot
            DesiredTile = Position.Copy();
        }

        /// <summary>
        /// Returns tiles around certain position that the ghost can move to (empty tiles, tiles with other ghosts, player...)
        /// </summary>
        /// <param name="positionAround">Position around which will be the method looking</param>
        /// <returns></returns>
        private List<GameObject> GetMovableTilesAround(Vector2D positionAround)
        {
            if (Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), positionAround))
                throw new ArgumentOutOfRangeException("positionAround", "positionAround is outside of the level!");

            List<GameObject> tilesAround = new List<GameObject>();

            Vector2D positionUp = positionAround + Vector2D.Up;
            Vector2D positionDown = positionAround + Vector2D.Down;
            Vector2D positionRight = positionAround + Vector2D.Right;
            Vector2D positionLeft = positionAround + Vector2D.Left;

            if (!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), positionUp))
                if (!(Level[positionUp.X, positionUp.Y] is Wall))
                    tilesAround.Add(Level[positionUp.X, positionUp.Y]);

            if (!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), positionDown))
                if (!(Level[positionDown.X, positionDown.Y] is Wall))
                    tilesAround.Add(Level[positionDown.X, positionDown.Y]);

            if (!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), positionRight))
                if (!(Level[positionRight.X, positionRight.Y] is Wall))
                    tilesAround.Add(Level[positionRight.X, positionRight.Y]);

            if (!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), positionLeft))
                if (!(Level[positionLeft.X, positionLeft.Y] is Wall))
                    tilesAround.Add(Level[positionLeft.X, positionLeft.Y]);

            return tilesAround;
        }

        public override MoveResult Move()
        {
            DetermineHeading();

            // If the ghost doesn't want to go anywhere
            if (DesiredTile.Equals(Position))
                return MoveResult.None;

            Vector2D moveToTile;

            if (!InvertedMove)
                moveToTile = Position + (new Vector2D(Heading));
            // Inverts the ghost's heading
            else
                moveToTile = InvertMove();

            if (Level[moveToTile.X, moveToTile.Y] is Empty empty)
            {
                // This returns the tile standing to the level[,], then saves the new TileStanding and then moves to ghost to the new position
                Empty tmp = empty;
                if(TileStanding == null)
                    Level[Position.X, Position.Y] = new Empty(Level, Position);
                else
                    Level[Position.X, Position.Y] = TileStanding;
                TileStanding = empty;

                // Redraw tile that the ghost previously stood on
                Level[Position.X, Position.Y].Print(GameManager.OFFSET);

                Position = moveToTile;
                Level[Position.X, Position.Y] = this;
            }
            else if (Level[moveToTile.X, moveToTile.Y] is Player player)
            {
                return MoveResult.Collision;
            }

            // Redraw the ghost
            Print(GameManager.OFFSET);

            return MoveResult.None;
        }

        /// <summary>
        /// Makes the ghost go in the opposite direction that it would usally
        /// </summary>
        /// <returns>In inverted position, whcih the ghost now wants to achieve</returns>
        private Vector2D InvertMove()
        {
            List<GameObject> movableTilesAround = GetMovableTilesAround(Position);
            Vector2D invertedMoveTile = Position - (new Vector2D(Heading));

            // Does the ghost have more options to go --> don't go on another ghost
            Avoid<Ghost>();

            // Only way to move --> go uninverted
            if (movableTilesAround.Count == 1)
                return Position + (new Vector2D(Heading));
            else
                movableTilesAround.Remove(Level[(Position + new Vector2D(Heading)).X, (Position + new Vector2D(Heading)).Y]);

            // Does the ghost have more options to go --> don't go next to player
            Avoid<Player>();

            if (!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), invertedMoveTile) && movableTilesAround.Contains(Level[invertedMoveTile.X, invertedMoveTile.Y]))
                return invertedMoveTile;
            else
                return movableTilesAround[0].Position;

            void Avoid<T>()
            {
                while (movableTilesAround.Count > 1)
                {
                    int movableTilesBefore = movableTilesAround.Count;

                    foreach (GameObject movableTile in movableTilesAround)
                    {
                        if (movableTile is T)
                        {
                            movableTilesAround.Remove(movableTile);
                            break;
                        }
                    }

                    if (movableTilesAround.Count == movableTilesBefore)
                        break;
                }
            }
        }

        /// <summary>
        /// Chooses tile that the ghost wants to eventually reach
        /// </summary>
        protected virtual void ChooseDesiredTile()
        {
            // Update player position
            PlayerPositionLastTurn = PlayerPositionThisTurn.Copy();
            foreach(GameObject gameObject in Level)
            {
                if (gameObject is Player)
                {
                    PlayerPositionThisTurn = gameObject.Position.Copy();
                    break;
                }
            }
        }
        #endregion
    }
}