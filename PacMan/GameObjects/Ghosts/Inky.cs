using System;
using System.Collections.Generic;
using System.Text;
using Setnicka.AuxiliaryClasses;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Type of ghost that behaves unpredictabely (sometimes like Blinky, sometimes like Pinky, sometimes like Clyde)
    /// </summary>
    internal class Inky : Ghost
    {
        // For how many Updates does the ghost commit to a certain play-style
        private const int COMMIT_TO_MODE_FOR = 3;

        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="playerStartingPosition">The starting position of the player within the level</param>
        public Inky(GameObject[,] level, Vector2D startingPosition, Vector2D playerStartingPosition) : base(level, startingPosition, playerStartingPosition)
        {
        }

        #region Automatic propeties
        private PlayStyle PlayStyle { get; set; }

        // How many updates before redeciding which play-style to follow
        private int DecisionTimer { get; set; } = 0;
        #endregion


        #region Methods
        protected override void ChooseDesiredTile()
        {
            InvertedMove = false;

            if(DecisionTimer == 0)
            {
                Random rnd = new Random(DateTime.Now.Millisecond);

                int rndNum = rnd.Next(100);

                if (rndNum < 34)
                    PlayStyle = PlayStyle.Blinky;
                else if (rndNum < 67)
                    PlayStyle = PlayStyle.Pinky;
                else
                    PlayStyle = PlayStyle.Clyde;

                DecisionTimer = COMMIT_TO_MODE_FOR;
            }

            switch (PlayStyle)
            {
                case PlayStyle.Blinky:
                    BlinkyChoose();
                    break;
                case PlayStyle.Pinky:
                    PinkyChoose();
                    break;
                case PlayStyle.Clyde:
                    ClydeChoose();
                    break;
                default:
                    break;
            }

            DecisionTimer--;

            void BlinkyChoose()
            {
                base.ChooseDesiredTile();

                // In order to follow player from behind he aims initially to the tile that player came from and
                // switches directly to the tile with player when close
                if (Position.DistanceTo(PlayerPositionThisTurn) > 1)
                    DesiredTile = PlayerPositionLastTurn;
                else
                    DesiredTile = PlayerPositionThisTurn;
            }

            void PinkyChoose()
            {
                base.ChooseDesiredTile();

                Vector2D playerHeading = PlayerPositionThisTurn - PlayerPositionLastTurn;

                Vector2D desiredTile = PlayerPositionThisTurn + playerHeading;

                // Tries to aim to position that the player is heading towards
                if (!Vector2D.VectorOutOf2DArray(Level.GetLength(0), Level.GetLength(1), desiredTile))
                    if (!(Level[desiredTile.X, desiredTile.Y] is Wall))
                    {
                        DesiredTile = desiredTile;
                        base.ChooseDesiredTile();
                        return;
                    }

                // If that position is unachievable, he aims directly for the player
                DesiredTile = PlayerPositionThisTurn;
            }

            void ClydeChoose()
            {
                BlinkyChoose();

                if (Position.DistanceTo(PlayerPositionThisTurn) < 3)
                    InvertedMove = true;
            }
        }

        protected override void Draw()
        {
            Console.ForegroundColor = GameColors.InkyColor;
            Console.BackgroundColor = GameColors.EmptyColor;

            Console.Write(APPEARANCE);
        }
        #endregion
    }

    /// <summary>
    /// Whose play-style will the ghost follow
    /// </summary>
    public enum PlayStyle
    {
        Blinky,
        Pinky,
        Clyde
    }
}
