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
        /// Adds <paramref name="data"/> to session.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        void Add<T>(string key, T data);

        /// <summary>
        /// Gets data from session.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Object of type <typeparamref name="T"/> or null.</returns>
        T Get<T>(string key);
    }
}
