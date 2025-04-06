using System;
using System.Collections.Generic;

namespace SimToolAI.Utilities
{
    /// <summary>
    ///     General purpose extension methods for the simulation toolkit
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets a random element from a list
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="list">The list to get a random element from</param>
        /// <param name="random">Optional random number generator</param>
        /// <returns>A random element from the list</returns>
        public static T GetRandomElement<T>(this IList<T> list, Random random = null)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("Cannot get a random element from an empty or null list");

            random ??= new Random();
            return list[random.Next(list.Count)];
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        /// <returns>The clamped value</returns>
        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        /// <returns>The clamped value</returns>
        public static float Clamp(this float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Converts a direction to a vector representation
        /// </summary>
        /// <param name="dir">The direction to convert</param>
        /// <returns>A tuple containing the x and y components of the direction vector</returns>
        public static (int x, int y) ToVector(this Direction dir)
        {
            return dir switch
            {
                Direction.None => (0, 0),
                Direction.DownLeft => (-1, 1),
                Direction.Down => (0, 1),
                Direction.DownRight => (1, 1),
                Direction.Left => (-1, 0),
                Direction.Center => (0, 0),
                Direction.Right => (1, 0),
                Direction.UpLeft => (-1, -1),
                Direction.Up => (0, -1),
                Direction.UpRight => (1, -1),
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid direction value")
            };
        }

        /// <summary>
        /// Gets the opposite direction
        /// </summary>
        /// <param name="dir">The direction to invert</param>
        /// <returns>The opposite direction</returns>
        public static Direction GetOpposite(this Direction dir)
        {
            return dir switch
            {
                Direction.None => Direction.None,
                Direction.DownLeft => Direction.UpRight,
                Direction.Down => Direction.Up,
                Direction.DownRight => Direction.UpLeft,
                Direction.Left => Direction.Right,
                Direction.Center => Direction.Center,
                Direction.Right => Direction.Left,
                Direction.UpLeft => Direction.DownRight,
                Direction.Up => Direction.Down,
                Direction.UpRight => Direction.DownLeft,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid direction value")
            };
        }

        /// <summary>
        /// Checks if the direction is diagonal
        /// </summary>
        /// <param name="dir">The direction to check</param>
        /// <returns>True if the direction is diagonal, false otherwise</returns>
        public static bool IsDiagonal(this Direction dir)
        {
            return dir is Direction.DownLeft or Direction.DownRight or Direction.UpLeft or Direction.UpRight;
        }

        /// <summary>
        /// Checks if the direction is cardinal (non-diagonal)
        /// </summary>
        /// <param name="dir">The direction to check</param>
        /// <returns>True if the direction is cardinal, false otherwise</returns>
        public static bool IsCardinal(this Direction dir)
        {
            return dir is Direction.Up or Direction.Down or Direction.Left or Direction.Right;
        }
    }
}