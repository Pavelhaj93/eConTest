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
    /// Settings for <see cref="IOfferService"/>.
    /// </summary>
    public class SignApiServiceOptions : BaseApiServiceOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignApiServiceOptions"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="base64Username">The base64 username.</param>
        /// <param name="base64password">The base64password.</param>
        public SignApiServiceOptions(string url, string base64Username, string base64password) : base(url, base64Username, base64password)
        {
        }
    }
}
