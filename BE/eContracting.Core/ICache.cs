using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// The cache where you can store data in a request cache or in persist cache limited by interval.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Adds <paramref name="data"/> to request cache.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        void AddToRequest<T>(string key, T data);

        /// <summary>
        /// Adds <paramref name="data"/> to session.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        void AddToSession<T>(string key, T data);

        /// <summary>
        /// Adds <paramref name="data"/> into persist cache for <paramref name="interval"/> from now.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <param name="interval">The interval.</param>
        void AddToPersist<T>(string key, T data, TimeSpan interval);

        /// <summary>
        /// Gets data from request cache.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Object of type <typeparamref name="T"/> or null.</returns>
        T GetFromRequest<T>(string key); // First try to get data from request cache, then from persist cache.

        /// <summary>
        /// Gets data from session.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Object of type <typeparamref name="T"/> or null.</returns>
        T GetFromSession<T>(string key);

        /// <summary>
        /// Gets from persist cache.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Object of type <typeparamref name="T"/> or null.</returns>
        T GetFromPersist<T>(string key);
    }
}
