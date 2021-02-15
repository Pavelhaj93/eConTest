using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace eContracting
{
    /// <summary>
    /// Represents wrapper over <see cref="HttpSessionState"/>.
    /// </summary>
    public interface ISessionProvider
    {
        /// <summary>
        /// Gets session ID.
        /// </summary>
        /// <returns>Session ID or null.</returns>
        string GetId();

        /// <summary>
        /// Gets the value from session.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Typed value or null.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Sets <paramref name="value"/> under specific <paramref name="key"/> into session. If <paramref name="key"/> exist, value will be overwriten.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="throwException">The exception when session is not accessible.</param>
        /// <exception cref="InvalidOperationException">When <paramref name="throwException"/> = true and session is not accessible.</exception>
        void Set<T>(string key, T value, bool throwException = false);

        /// <summary>
        /// Removes value from session under specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);

        /// <summary>
        /// Sets the timeout in minutes.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        void SetTimeout(int minutes);
    }
}
