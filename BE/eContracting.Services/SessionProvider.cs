using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace eContracting.Services
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public class SessionProvider : ISessionProvider
    {
        protected readonly IRespApiService ApiService;
        protected readonly ISettingsReaderService SettingReader;

        public SessionProvider(IRespApiService apiService, ISettingsReaderService settingReader)
        {
            this.ApiService = apiService;
            this.SettingReader = settingReader;
        }

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
                Thread.Sleep(500);
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

        /// <inheritdoc/>
        public void RefreshSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();

            var cookieName = this.GetSessionCookieName();
            var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Remove(cookieName);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        protected string GetSessionCookieName()
        {
            var configuration = ConfigurationManager.GetSection("system.web/sessionState") as System.Web.Configuration.SessionStateSection;

            if (configuration != null)
            {
                return configuration.CookieName;
            }

            throw new ApplicationException("Cannot read session cookie name from the configuration");
        }
    }
}
