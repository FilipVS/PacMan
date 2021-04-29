using System;
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

        private const int MINIMAL_DISTANCE_FROM_PLAYER_CLYDE_MODE = 4;

        /// <param name="level">The level that the GameObject is associated with</param>
        /// <param name="startingPosition">The starting position of the GameObject in the level</param>
        /// <param name="playerStartingPosition">The starting position of the player within the level</param>
        public Inky(GameObject[,] level, Vector2D startingPosition, Vector2D playerStartingPosition) : base(level, startingPosition, playerStartingPosition)
        {
        }

        #region Propeties
        private PlayStyle PlayStyle { get; set; }

        // How many updates before re-deciding which play-style to follow
        private int DecisionTimer { get; set; } = 0;
        #endregion


        #region Methods
        protected override void ChooseDesiredTile()
        {
            base.ChooseDesiredTile();

            // If inverted move, ghosts always aim at player
            if (InvertedMove)
            {
                AimAtPlayer();
                return;
            }

            #region Deciding the PlayStyle
            // If the ghost is supposed to re-decide its playstyle
            if (DecisionTimer == 0)
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
            #endregion

            // Chossing the heading
            switch (PlayStyle)
            {
                case PlayStyle.Blinky:
                    AimBehindPlayer();
                    break;
                case PlayStyle.Pinky:
                    AimInFrontOfPlayer();
                    break;
                case PlayStyle.Clyde:
                    AimBehindPlayer();

                    if (Position.DistanceTo(PlayerPositionThisTurn) < MINIMAL_DISTANCE_FROM_PLAYER_CLYDE_MODE)
                        InvertedMove = true;
                    break;
                default:
                    break;
            }

            DecisionTimer--;
        }

        protected override void Draw()
        {
            if (Style == DrawingStyle.ChasingGhosts)
                Console.ForegroundColor = GameColors.ChasingGhostMainColor;
            else if (Style == DrawingStyle.ChasingGhostsAlternate)
                Console.ForegroundColor = GameColors.ChasingGhostAlternateColor;
            else
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
