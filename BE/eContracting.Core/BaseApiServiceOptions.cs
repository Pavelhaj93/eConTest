﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    public abstract class BaseApiServiceOptions
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
        /// Initializes a new instance of the <see cref="BaseApiServiceOptions"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="base64Username">The base64 username.</param>
        /// <param name="base64password">The base64password.</param>
        /// <exception cref="ArgumentException">
        /// Url cannot be empty - url
        /// or
        /// Username cannot be empty - base64Username
        /// or
        /// Password cannot be empty - base64password
        /// </exception>
        public BaseApiServiceOptions(string url, string base64Username, string base64password)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Url cannot be empty", nameof(url));
            }

            if (string.IsNullOrEmpty(base64Username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(base64Username));
            }

            if (string.IsNullOrEmpty(base64password))
            {
                throw new ArgumentException("Password cannot be empty", nameof(base64password));
            }

            this.Url = new Uri(url, UriKind.Absolute);
            this.User = Encoding.UTF8.GetString(Convert.FromBase64String(base64Username));
            this.Password = Encoding.UTF8.GetString(Convert.FromBase64String(base64password));
        }
    }
}
