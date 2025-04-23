using System;
using System.Collections.Generic;

namespace SimToolAI.Utilities
{
    /// <summary>
    /// General purpose extension methods for the simulation toolkit
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
    }
}