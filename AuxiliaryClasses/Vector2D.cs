using System;

namespace Setnicka.AuxiliaryClasses
{
    /// <summary>
    /// Auxiliary class for dealing with position of GameObjects (their X and Y coordinates)
    /// Basic operatios are also defined on Vector2Ds (addition, subtraction)
    /// </summary>
    public class Vector2D
    {
        // Made to work with array indexing and printing on the screen (so one move up on the screen means -1 in the array[].GetLength(1) direction etc.)
        public static readonly Vector2D Up = new Vector2D(0, -1);
        public static readonly Vector2D Down = new Vector2D(0, 1);
        public static readonly Vector2D Right = new Vector2D(1, 0);
        public static readonly Vector2D Left = new Vector2D(-1, 0);

        public static readonly Vector2D Zero = new Vector2D(0, 0);

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
        /// Creates new Vector2D with magnitude 1 and values responding to the direction
        /// </summary>
        public Vector2D(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    X = 0;
                    Y = 1;
                    break;
                case Direction.Left:
                    X = -1;
                    Y = 0;
                    break;
                case Direction.Right:
                    X = 1;
                    Y = 0;
                    break;
                case Direction.Up:
                    X = 0;
                    Y = -1;
                    break;
                default:
                    break;
            }
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

        #region Properties
        public int X { get; set; }

        public int Y { get; set; }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt((X * X) + (Y * Y));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks whether a vector coordinates are out of bounds of a two-dimensional array
        /// </summary>
        /// <param name="arrayWidth">GetLength(0) of the 2D array</param>
        /// <param name="arrayHeight">GetLength(1) of the 2D array</param>
        /// <param name="vector">Coordinates to be checked</param>
        /// <returns>True if it is out of bounds, false if it is inside the array bounds</returns>
        public static bool VectorOutOf2DArray(int arrayWidth, int arrayHeight, Vector2D vector)
        {
            if (vector.X < 0 || vector.Y < 0 || vector.X >= arrayWidth || vector.Y >= arrayHeight)
                return true;
            return false;
        }

        /// <summary>
        /// Two Vector2Ds are equl when they have the same X and Y coordinates
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Vector2D vector)
            {
                if (vector.X == this.X && vector.Y == this.Y)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Make the GetHashCode work on the same principle as Equals - objects that return Equals true return the same hash
        /// </summary>
        public override int GetHashCode()
        {
            return int.Parse($"{X}{Y}");
        }

        /// <summary>
        /// Returns new Vector2D with the same x and y values as this one
        /// </summary>
        /// <returns>New Vector2D with the same x and y values as this one</returns>
        public Vector2D Copy()
        {
            return new Vector2D(X, Y);
        }

        /// <summary>
        /// Calculates distance from coordinates of this Vector2D to coordinates of another Vector2D
        /// </summary>
        /// <param name="distanceTo">Calculate distance to this point</param>
        /// <returns>Distance from coordinates of this Vector2D to coordinates of another Vector2D</returns>
        public double DistanceTo(Vector2D distanceTo)
        {
            double x = (X - distanceTo.X);
            double y = (Y - distanceTo.Y);

            return Math.Sqrt(x * x + y * y);
        }
        #endregion

        #region OperatorOverrides
        // Definition of basic operations on Vector2Ds

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D((a.X + b.X), (a.Y + b.Y));
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D((a.X - b.X), (a.Y - b.Y));
        }

        public static Vector2D operator *(Vector2D a, int b)
        {
            return new Vector2D((a.X * b), (a.Y * b));
        }

        public static Vector2D operator *(int a, Vector2D b)
        {
            return new Vector2D((b.X * a), (b.Y * a));
        }
        #endregion
    }


    /// <summary>
    /// MovableObjects use this enum to specify which way the intend to move
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
