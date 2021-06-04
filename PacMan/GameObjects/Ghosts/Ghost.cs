using System;
using System.Collections.Generic;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Base class for all ghosts
    /// Contains maze-solving and navigational logic (it is the same for all ghosts)
    /// </summary>
    internal abstract class Ghost : MovableObject
    {
        #region Constants
        protected const char APPEARANCE = '&';
        private const int MAXIMUM_GHOSTS_AROUND = 1;
        #endregion

        #region Constructors
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
        #endregion

        #region Properties
        public int[,] MazeMap { get; private set; }

        public Empty TileStanding { get; set; }

        protected Vector2D PlayerPositionLastTurn { get; set; }

        public Vector2D PlayerPositionThisTurn { get; protected set; }

        protected Vector2D DesiredTile { get; set; }

        public bool InvertedMove { get; set; }

        // Used for telling the ghost to skip the next move cycle
        public bool SkipMove { get; set; } = false;

        // How will be the ghost printed
        public DrawingStyle Style { get; set; }
        #endregion


        #region Methods
        public override MoveResult Move()
        {
            DetermineHeading();

            // If the ghost doesn't want to go anywhere, skip movement
            if (DesiredTile.Equals(Position))
            {
                BeforeReturn();
                return MoveResult.None;
            }

            #region Altering movement (avoiding, inverting...)
            Vector2D moveToTile;

            // If movement is not inverted, add heading to our current position
            if (!InvertedMove)
            {
                // If there are too many ghosts around, avoid ghosts (to avoid blocking up)
                if (CountGhostsAround() > MAXIMUM_GHOSTS_AROUND)
                    moveToTile = AlterMove(false, false, true);
                // Move normally
                else
                    moveToTile = Position + (new Vector2D(Heading));
            }
            // Else invert the ghost's movement
            else
            {
                // If there are too many ghosts around, avoid ghosts (to avoid blocking up)
                if (CountGhostsAround() > MAXIMUM_GHOSTS_AROUND)
                    moveToTile = AlterMove(true, true, true);
                // Just move inverted
                else
                    moveToTile = AlterMove(true, true, false);
            }
            #endregion

            // Turn off move invertment for the next turn
            InvertedMove = false;

            // If the ghost is supposed to skip move, skip it...
            if (SkipMove)
                DesiredTile = Position.Copy();

            // If the ghost doesn't want to go anywhere
            if (DesiredTile.Equals(Position))
            {
                BeforeReturn();
                return MoveResult.None;
            }

            #region Movement itself
            if (Level[moveToTile.X, moveToTile.Y] is Empty empty)
            {
                // This returns the tile standing to the level[,], then saves the new TileStanding and then moves the ghost to the new position
                Empty tmp = empty;
                if (TileStanding == null)
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
                BeforeReturn();
                return MoveResult.Collision;
            }
            else if (Level[moveToTile.X, moveToTile.Y] is Ghost ghost)
            {
                // Switch the tiles we are standing on
                Empty myTileStanding = TileStanding;
                TileStanding = ghost.TileStanding;
                ghost.TileStanding = myTileStanding;

                // Move us
                Vector2D myPosition = Position;
                Level[ghost.Position.X, ghost.Position.Y] = this;
                Level[myPosition.X, myPosition.Y] = ghost;

                // Reset our positions
                ghost.Position = myPosition.Copy();
                Position = moveToTile.Copy();

                // Redraw us
                ghost.Print(GameManager.OFFSET);
                this.Print(GameManager.OFFSET);

                // Tell the other ghost not to move this cycle
                ghost.SkipMove = true;
            }

            // Redraw the ghost
            Print(GameManager.OFFSET);
            #endregion

            BeforeReturn();
            return MoveResult.None;

            void BeforeReturn()
            {
                SkipMove = false;
            }
        }

        /// <summary>
        /// Alters the movement of the ghost (makes it go inverted, or avoid certein GameObjects)
        /// </summary>
        /// <returns>The altered position, which the ghost now wants to achieve</returns>
        private Vector2D AlterMove(bool invertMove = false, bool avoidPlayer = false, bool avoidGhosts = false)
        {
            // Get all the movableTilesAround from our position
            List<GameObject> movableTilesAround = GetMovableTilesAround(Position);      

            // Avoid given types
            if (avoidPlayer)
                Avoid<Player>();
            if (avoidGhosts)
                Avoid<Ghost>();

            // If there is no legal move according to the rules, don't go anywhere
            if (movableTilesAround.Count < 1)
                return Position.Copy();

            // Transform the list of movableTilesAround into a list of the tile's positions
            List<Vector2D> movableTilesAroundPositions = new List<Vector2D>();
            foreach (GameObject tile in movableTilesAround)
                movableTilesAroundPositions.Add(tile.Position);

            // If invertedMove, try to invert the move
            if (InvertedMove)
            {
                Vector2D invertedMoveTile = Position - (new Vector2D(Heading));

                // If there are more than one options to move, don't go to the original DesiredTile
                if(movableTilesAround.Count > 1)
                    movableTilesAround.Remove(Level[(Position + new Vector2D(Heading)).X, (Position + new Vector2D(Heading)).Y]);

                // If the invertedMoveTile is part of the legal moves, go there
                if (movableTilesAroundPositions.Contains(invertedMoveTile))
                    return invertedMoveTile;
            }
            // If the ghost can move normaly, let it
            else
            {
                Vector2D desiredMove = Position + new Vector2D(Heading);

                if (movableTilesAroundPositions.Contains(desiredMove))
                    return desiredMove;
            }

            // If movemenet was not supposed to be inverted, or the invertedMoveTile was illegal, or the desired move was illegal, move to the first legal tile
            return movableTilesAround[0].Position;

            // Removes certein type of object from movableTilesAround
            void Avoid<T>()
            {
                while (movableTilesAround.Count > 0)
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

            #region Backtracking MazeMap
            // We go from the desiredTile down by the order to find the path's tile next to the ghost

            Vector2D currentlyOn = DesiredTile.Copy();
            Vector2D checkedTile;
            int order = MazeMap[currentlyOn.X, currentlyOn.Y];

            while (true)
            {
                // Found the tile in path next to the ghost
                if (order == 1)
                    break;

                // Check all the tiles around and try to find the next order tile
                Vector2D[] directions = new Vector2D[] { Vector2D.Up, Vector2D.Down, Vector2D.Right, Vector2D.Left};
                foreach(Vector2D direction in directions)
                {
                    checkedTile = currentlyOn + direction;
                    if (!Vector2D.VectorOutOf2DArray(MazeMap.GetLength(0), MazeMap.GetLength(1), checkedTile))
                    {
                        if(MazeMap[checkedTile.X, checkedTile.Y] < order && MazeMap[checkedTile.X, checkedTile.Y] > 0)
                        {
                            order--;
                            currentlyOn = checkedTile;
                            break;
                        }
                    }
                }

                // TODO: Delete if the alternate solution works
                /*// Check all the tiles around and try to find the next order tile
                checkedTile = currentlyOn + Vector2D.Up;
                if (!Vector2D.VectorOutOf2DArray(MazeMap.GetLength(0), MazeMap.GetLength(1), checkedTile))
                    if (MazeMap[checkedTile.X, checkedTile.Y] < order && MazeMap[checkedTile.X, checkedTile.Y] > 0)
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
                    }*/
            }
            #endregion

            #region Choosing heading
            // Finally count the new Heading
            Vector2D positionDifference = currentlyOn - Position;
            if (positionDifference.X == 1)
            {
                if (positionDifference.Y == 0)
                    Heading = Direction.Right;
                else
                    DoNotMove();
            }
            else if (positionDifference.X == -1)
            {
                if (positionDifference.Y == 0)
                    Heading = Direction.Left;
                else
                    DoNotMove();
            }
            else if (positionDifference.X == 0)
            {
                if (positionDifference.Y == 1)
                    Heading = Direction.Down;
                else if (positionDifference.Y == -1)
                    Heading = Direction.Up;
                else
                    DoNotMove();           
            }
            else
                DoNotMove();
            #endregion

            // Sets the desired tile to be the same as current tile, so the ghost won't move
            void DoNotMove()
            {
                DesiredTile = Position.Copy();
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

            // The rest is done in the overrided method of each ghost
        }

        // Methods for choosing desired tile
        /// <summary>
        /// Aims directly at player
        /// </summary>
        protected void AimAtPlayer()
        {
            DesiredTile = PlayerPositionThisTurn;
        }

        /// <summary>
        /// Aims in front of the player
        /// </summary>
        protected void AimInFrontOfPlayer()
        {
            Vector2D playerHeading = PlayerPositionThisTurn - PlayerPositionLastTurn;

            // If player moved multiple tiles (unusual) or if he is next to the ghost, follow him directly
            if(playerHeading.Magnitude > 1 || Position.DistanceTo(PlayerPositionThisTurn) < 2)
            {
                DesiredTile = PlayerPositionThisTurn;
                return;
            }

            Vector2D desiredTile = PlayerPositionThisTurn + playerHeading;

            // Tries to aim at the position that the player is heading towards
            if (!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), desiredTile))
            {
                if (!(Level[desiredTile.X, desiredTile.Y] is Wall))
                {
                    DesiredTile = desiredTile;
                    return;
                }
            }

            // If that position is unachievable, he aims directly for the player
            DesiredTile = PlayerPositionThisTurn;
        }

        /// <summary>
        /// Aims at the position that the player left
        /// </summary>
        protected void AimBehindPlayer()
        {
            // In order to follow player from behind he aims initially to the tile that player came from and
            // switches directly to the tile with player when close
            if (Position.DistanceTo(PlayerPositionThisTurn) > 1)
                DesiredTile = PlayerPositionLastTurn;
            else
                DesiredTile = PlayerPositionThisTurn;
        }

        /// <summary>
        /// Maze solving algorithm - the way ghosts navigate (finds the shortest way to the desired tile)
        /// The algorithm is rating tiles in "circles" ("circle" of tiles 1 move away, 2 moves away..) until it finds the desired tile
        /// </summary>
        private void SolveMaze()
        {
            // Check if the desired tile is within level
            if (Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), DesiredTile))
                throw new ArgumentOutOfRangeException("desiredTile", "desiredTile is outside of the level!");

            // Reset MazeMap for each solve
            MazeMap = new int[Level.GetLength(0), Level.GetLength(1)];

            // Tiles that are 1 more move away, than the previous set (initially ghosts's position)
            HashSet<GameObject> nextOrderTiles = new HashSet<GameObject>(new List<GameObject>() { Level[Position.X, Position.Y] });
            // Tiles, that are exactly 'order' moves away form the ghost
            HashSet<GameObject> thisOrderTiles;
            int order = -1;

            // If we didn't find any new tiles, the ghost is blocked in and cannot reach desired tile
            while (nextOrderTiles.Count > 0)
            {
                order++;

                // Set the order sets for each cycle
                thisOrderTiles = nextOrderTiles;
                nextOrderTiles = new HashSet<GameObject>();

                // We set the order for thisOrderTiles and find any available tiles around them
                foreach (GameObject tile in thisOrderTiles)
                {
                    MazeMap[tile.Position.X, tile.Position.Y] = order;

                    // If we found our desired tile, the path is done, we can return
                    if (tile.Position.X == DesiredTile.X && tile.Position.Y == DesiredTile.Y)
                        return;
                
                    // We find any movable tiles around (which would be 'order++' moves away), we add them to nextOrderTiles
                    foreach (GameObject tile2 in GetMovableTilesAround(tile.Position))
                    {
                        // If we haven't put order on the tile yet and the tile isn't the ghost's position
                        if (MazeMap[tile2.Position.X, tile2.Position.Y] == 0 && !tile2.Position.Equals(Position))
                            nextOrderTiles.Add(tile2);
                    }
                }

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
            // Check that the position is within level
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


        /// <summary>
        /// Counts how many ghosts are around this one
        /// </summary>
        private int CountGhostsAround()
        {
            List<GameObject> movableTilesAround = GetMovableTilesAround(Position);

            int numOfGhosts = 0;

            foreach (GameObject gameObject in movableTilesAround)
            {
                if (gameObject is Ghost)
                    numOfGhosts++;
            }

            return numOfGhosts;
        }
        #endregion

        public enum DrawingStyle
        {
            Normal,
            ChasingGhosts,
            ChasingGhostsAlternate
        }
    }
}