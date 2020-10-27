using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;
using Sitecore.Web;

namespace eContracting
{
    public static class Utils
    {
        /// <summary>
        /// Gers readable size.
        /// </summary>
        /// <seealso cref="https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net"/>
        /// <param name="size">The size.</param>
        public static string GetReadableFileSize(int size)
        {
            if (size < 0)
            {
                size = 0;
            }

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = (double)size;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        public static string AesEncrypt(string input, string key, string vector)
        {
            byte[] encrypted;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Encoding.UTF8.GetBytes(key);
                rijAlg.IV = Encoding.UTF8.GetBytes(vector);

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string GetMd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string SetQuery(Uri url, string key, string value)
        {
            var uriBuilder = new UriBuilder(url);
            var query = WebUtil.ParseQueryString(uriBuilder.Query);
            query[key] = value;
            uriBuilder.Query = WebUtil.BuildQueryString(query, false);
            return uriBuilder.Uri.ToString();
        }

        public static string SetQuery(string url, string key, string value)
        {
            return SetQuery(new Uri(url), key, value);
        }

        /// <summary>
        /// Gets the unique key for login type in a login view.
        /// </summary>
        /// <param name="loginType">Type of the login.</param>
        /// <param name="offer">The offer.</param>
        /// <returns>Generated key.</returns>
        public static string GetUniqueKey(LoginTypeModel loginType, OfferModel offer)
        {
            if (loginType == null)
            {
                throw new ArgumentNullException(nameof(loginType));
            }

            if (offer == null)
            {
                throw new ArgumentNullException(nameof(offer));
            }

            return Utils.GetMd5(loginType.ID.ToString() + offer.Guid);
        }
    }
}
