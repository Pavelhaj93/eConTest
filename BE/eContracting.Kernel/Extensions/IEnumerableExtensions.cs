// <copyright file="IEnumerableExtensions.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace System.Collections.Generic
{
    using System.Linq;

    /// <summary>
    /// Implementation of <see cref="IEnumerable"/> extension methods.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Checks itf the collection is empty or null. 
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="en">colelction to check</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> en)
        {
            return en == null || !en.Any();
        }

        /// <summary>
        /// Checks if the coillection is not empty.
        /// </summary>
        /// <typeparam name="T">Type of the item.</typeparam>
        /// <param name="en">Enumarable to check.</param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> en)
        {
            return !en.IsNullOrEmpty<T>();
        }
    }
}
