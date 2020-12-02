using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// Cache relevant only for current user. Data cannot be shared among sessions.
    /// </summary>
    public interface IUserCacheService
    {
        /// <summary>
        /// Adds or rewrite <paramref name="data"/> under specific <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        void Set<T>(string key, T data);

        /// <summary>
        /// Gets data by <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Object of type <typeparamref name="T"/> or null.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Removes data under specific <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        /// <summary>
        /// Removes all data.
        /// </summary>
        void Clear();
    }
}
