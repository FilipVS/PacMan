using System;
using System.Collections.Generic;
using System.Text;

namespace Setnicka.PacMan
{
    /// <summary>
    /// Auxiliary class for dealing with position of GameObjects (their X and Y coordinates)
    /// Basic operatios are also defined on Vector2Ds (addition, subtraction)
    /// </summary>
    class Vector2D
    {
        #region Constructors
        /// <summary>
        /// Creates new Vector2D with its coordinates set to [0, 0]
        /// </summary>
        public Vector2D()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Creates new Vector2D with the specified parametres as coordinates
        /// </summary>
        /// <param name="x">X coordinate value</param>
        /// <param name="y">Y coordinate value</param>
        public Vector2D(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Fields
        private int x;

        private int y;
        #endregion

        #region Properties
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        #endregion

        #region OperatorOverrides
        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D((a.X + b.X), (a.Y + b.Y));
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D((a.X - b.X), (a.Y - b.Y));
        }
        #endregion
    }
}
