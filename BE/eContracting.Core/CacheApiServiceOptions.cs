using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// Settings for <see cref="IApiService"/>.
    /// </summary>
    public class CacheApiServiceOptions
    {
        /// <summary>
        /// The user.
        /// </summary>
        public readonly string User;

        /// <summary>
        /// The password.
        /// </summary>
        public readonly string Password;

        /// <summary>
        /// The URL.
        /// </summary>
        public readonly Uri Url;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheApiServiceOptions"/> class.
        /// </summary>
        /// <param name="base64Username">The base64 username.</param>
        /// <param name="base64password">The base64password.</param>
        /// <param name="url">The URL.</param>
        public CacheApiServiceOptions(string base64Username, string base64password, string url)
        {
            this.User = Encoding.UTF8.GetString(Convert.FromBase64String(base64Username));
            this.Password = Encoding.UTF8.GetString(Convert.FromBase64String(base64password));
            this.Url = new Uri(url, UriKind.Absolute);
        }

        //public ICredentials GetCredentials()
        //{
        //    var username = Encoding.UTF8.GetString(Convert.FromBase64String(this.User));
        //    var password = Encoding.UTF8.GetString(Convert.FromBase64String(this.Password));

        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        throw new InvalidCredentialException("Wrong credentials for cache!");
        //    }

        //    var uri = new Uri(this.Url, UriKind.Absolute);

        //    var cred = new CredentialCache();
        //    cred.Add(uri, "Basic", new NetworkCredential(username, password));
        //    return cred;
        //}
    }
}
