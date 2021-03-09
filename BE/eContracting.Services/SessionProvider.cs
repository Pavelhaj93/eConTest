using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eContracting.Services
{
    /// <inheritdoc/>
    public class SessionProvider : ISessionProvider
    {
        /// <inheritdoc/>
        public string GetId()
        {
            return HttpContext.Current?.Session.SessionID;
        }

        /// <inheritdoc/>
        public T GetValue<T>(string key)
        {
            if (HttpContext.Current?.Session != null)
            {
                return (T)HttpContext.Current.Session[key];
            }

            return default(T);
        }

        /// <inheritdoc/>
        public void Set<T>(string key, T value, bool throwException = false)
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            HttpContext.Current?.Session?.Remove(key);
        }

        /// <inheritdoc/>
        public void SetTimeout(int minutes)
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session.Timeout = minutes;
            }
        }

        /// <inheritdoc/>
        public void Abandon()
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session.Abandon();
            }
        }

    }
}
