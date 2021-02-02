using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;
using eContracting.Services;
using Sitecore.Web;

namespace eContracting
{
    public static class Utils
    {
        /// <summary>
        /// Regular expression for HTML attribute ' style="..."'.
        /// </summary>
        private static Regex RegexAttrStyle = new Regex("( style=\"[\\d\\w\\s\\-\\:\\;]*\")", RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for XML attribute ' xmlns="..."'.
        /// </summary>
        private static Regex RegexAttrXmlNamespace = new Regex("( xmlns=\"[^\"]*\")", RegexOptions.Compiled);

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
            return string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1}", len, sizes[order]);
        }

        [ExcludeFromCodeCoverage]
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
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[key] = value;
            uriBuilder.Query = GetQueryString(query);
            return uriBuilder.Uri.ToString();
        }

        public static string SetQuery(string url, string key, string value)
        {
            return SetQuery(new Uri(url), key, value);
        }

        /// <summary>
        /// Gets the query string from <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>Query string representation from <paramref name="collection"/> items.</returns>
        public static string GetQueryString(NameValueCollection collection)
        {
            if ((collection?.Count ?? 0) == 0)
            {
                return string.Empty;
            }

            var items = new List<string>();

            foreach (var key in collection.AllKeys)
            {
                items.Add(key + "=" + HttpUtility.UrlEncode(collection[key]));
            }

            var query = string.Join("&", items);
            return query;
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

        /// <summary>
        /// Gets the unique key for template document.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>Hash of <see cref="OfferAttachmentXmlModel.IdAttach"/> + <see cref="OfferAttachmentXmlModel.Group"/> + <see cref="OfferAttachmentXmlModel.Description"/></returns>
        [ExcludeFromCodeCoverage]
        public static string GetUniqueKey(OfferAttachmentXmlModel template)
        {
            var data = template.IdAttach + template.Group + template.Description;
            return GetMd5(data);
        }

        /// <summary>
        /// Replaces the HTML attribute <c>style</c> and XML attribute <c>xmlns</c> from given <paramref name="input"/> string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Clean up string.</returns>
        public static string ReplaceXmlAttributes(string input)
        {
            input = RegexAttrStyle.Replace(input, "");
            input = RegexAttrXmlNamespace.Replace(input, "");
            return input;
        }

        /// <summary>
        /// Gets value from 'HttpContext.Current.Request', first try take it from 'ServerVariables["HTTP_X_FORWARDED_FOR"]', if it's empty, try take it from 'ServerVariables["REMOTE_ADDR"]'.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static string GetIpAddress()
        {
            string text = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return string.IsNullOrEmpty(text) ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : text.Split(',')[0];
        }

        /// <summary>
        /// Creates the attribute(s) from template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>Collection with IDATTACH attribute.</returns>
        public static ZCCH_ST_ATTRIB[] CreateAttributesFromTemplate(OfferAttachmentXmlModel template)
        {
            var attributes = new List<ZCCH_ST_ATTRIB>();
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.SEQUENCE_NUMBER, ATTRVAL = template.SequenceNumber ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = template.IdAttach ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.DESCRIPTION, ATTRVAL = template.Description ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.OBLIGATORY, ATTRVAL = template.Obligatory ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.PRINTED, ATTRVAL = template.Printed ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.SIGN_REQ, ATTRVAL = template.SignReq ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TMST_REQ, ATTRVAL = template.TimeStampRequired ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.ADDINFO, ATTRVAL = template.AddInfo ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TEMPL_ALC_ID, ATTRVAL = template.TemplAlcId ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.GROUP, ATTRVAL = template.Group ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.GROUP_OBLIG, ATTRVAL = template.GroupObligatory ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.ITEM_GUID, ATTRVAL = template.ItemGuid ?? string.Empty });
            attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.CONSENT_TYPE, ATTRVAL = template.ConsentType ?? string.Empty });
            return attributes.ToArray();
        }

        /// <summary>
        /// Replaces placeholders in <paramref name="text"/> (e.g.: {PERSON_ADDRESS}) with key / value from <paramref name="textParameters"/>.
        /// </summary>
        /// <param name="text">The original text.</param>
        /// <param name="textParameters">The text parameters.</param>
        /// <returns>Modified string.</returns>
        public static string GetReplacedTextTokens(string text, IDictionary<string, string> textParameters)
        {
            if (string.IsNullOrWhiteSpace(text) || (textParameters?.Count ?? 0) < 1)
            {
                return text;
            }

            foreach (var textParam in textParameters)
            {
                text = text.Replace("{" + textParam.Key + "}", textParam.Value);
            }

            return text;
        }

        public static T[] GetUpdated<T>(T[] array, T newItem)
        {
            var list = new List<T>(array);
            list.Add(newItem);
            return list.ToArray();
        }
    }
}
