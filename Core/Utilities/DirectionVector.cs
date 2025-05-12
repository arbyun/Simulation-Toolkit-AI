using System;
using System.Numerics;

namespace SimArena.Core.Utilities
{
    /// <summary>
    /// Utility class for direction vectors
    /// </summary>
    public static class DirectionVector
    {
        // Standard direction vectors
        public static readonly Vector3 None = new Vector3(0, 0, 0);
        public static readonly Vector3 Up = new Vector3(0, -1, 0);
        public static readonly Vector3 Down = new Vector3(0, 1, 0);
        public static readonly Vector3 Left = new Vector3(-1, 0, 0);
        public static readonly Vector3 Right = new Vector3(1, 0, 0);
        public static readonly Vector3 UpLeft = new Vector3(-1, -1, 0);
        public static readonly Vector3 UpRight = new Vector3(1, -1, 0);
        public static readonly Vector3 DownLeft = new Vector3(-1, 1, 0);
        public static readonly Vector3 DownRight = new Vector3(1, 1, 0);
        public static readonly Vector3 Center = new Vector3(0, 0, 0);

        /// <summary>
        /// Gets the opposite direction
        /// </summary>
        /// <param name="direction">The direction to invert</param>
        /// <returns>The opposite direction</returns>
        public static Vector3 GetOpposite(Vector3 direction)
        {
            return new Vector3(-direction.X, -direction.Y, -direction.Z);
        }

        /// <summary>
        /// Checks if the direction is diagonal
        /// </summary>
        /// <param name="direction">The direction to check</param>
        /// <returns>True if the direction is diagonal, false otherwise</returns>
        public static bool IsDiagonal(Vector3 direction)
        {
            return Math.Abs(direction.X) > 0.5f && Math.Abs(direction.Y) > 0.5f;
        }

        /// <summary>
        /// Checks if the direction is cardinal (non-diagonal)
        /// </summary>
        /// <param name="direction">The direction to check</param>
        /// <returns>True if the direction is cardinal, false otherwise</returns>
        public static bool IsCardinal(Vector3 direction)
        {
            return (Math.Abs(direction.X) > 0.5f && Math.Abs(direction.Y) < 0.5f) || 
                   (Math.Abs(direction.X) < 0.5f && Math.Abs(direction.Y) > 0.5f);
        }

        /// <summary>
        /// Gets a random cardinal direction
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <returns>A random cardinal direction</returns>
        public static Vector3 GetRandomCardinalDirection(Random random = null)
        {
            random ??= new Random();
            int direction = random.Next(4);
        
            return direction switch
            {
                0 => Up,
                1 => Right,
                2 => Down,
                3 => Left,
                _ => None
            };
        }

        /// <summary>
        /// Converts a Vector2 input to a direction Vector3
        /// </summary>
        /// <param name="input">The input vector</param>
        /// <returns>A direction vector</returns>
        public static Vector3 FromInput(Vector2 input)
        {
            if (input.LengthSquared() < 0.1f)
                return None;

            // Determine the direction based on input
            if (Math.Abs(input.X) > Math.Abs(input.Y))
            {
                // Horizontal movement takes precedence
                return input.X > 0 ? Right : Left;
            }

            // Vertical movement
            return input.Y > 0 ? Down : Up;
        }

        /// <summary>
        /// Gets the rotation angle in degrees for a direction
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The rotation angle in degrees</returns>
        public static float GetRotationAngle(Vector3 direction)
        {
            if (direction == Up) return 270f;
            if (direction == Right) return 0f;
            if (direction == Down) return 90f;
            if (direction == Left) return 180f;
        
            return 0f;
        }
    }
}