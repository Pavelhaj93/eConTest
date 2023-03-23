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
using Newtonsoft.Json;
using Sitecore.Web;

namespace eContracting
{
    public static class Utils
    {
        static Utils()
        {
            RegexAttrStyle = new Regex("( style=\"[\\d\\w\\s\\-\\:\\;]*\")", RegexOptions.Compiled);
            RegexAttrXmlNamespace = new Regex("( xmlns=\"[^\"]*\")", RegexOptions.Compiled);
            RegexHtml = new Regex("<(.|\n)*?>", RegexOptions.Compiled);
        }

        /// <summary>
        /// Regular expression for HTML attribute ' style="..."'.
        /// </summary>
        private static readonly Regex RegexAttrStyle;

        /// <summary>
        /// Regular expression for XML attribute ' xmlns="..."'.
        /// </summary>
        private static readonly Regex RegexAttrXmlNamespace;

        private static readonly Regex RegexHtml;

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
        public static string RijndaelEncrypt(string input, string key, string vector)
        {
            byte[] encrypted;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Encoding.UTF8.GetBytes(key);
                rijAlg.IV = Encoding.UTF8.GetBytes(vector);

                using (var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV))
                {
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
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Encrypts <paramref name="input"/> with AES algorithm.
        /// </summary>
        /// <typeparam name="T">The type serializable with <see cref="JsonConvert.SerializeObject(object)"/></typeparam>
        /// <param name="input">The object for encryption.</param>
        /// <param name="key">The secret key for the symmetric algorithm.</param>
        /// <param name="vector">The initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>) for the symmetric algorithm.</param>
        /// <returns>Data in base 64 format.</returns>
        public static string AesEncrypt<T>(T input, string key, string vector)
        {
            var data = JsonConvert.SerializeObject(input);
            byte[] encrypted;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(vector);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(data);
                            }

                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts <paramref name="input"/> data with AES algorithm.
        /// </summary>
        /// <typeparam name="T">The type deserializable with <see cref="JsonConvert.DeserializeObject(string)"/></typeparam>
        /// <param name="input">The data in base 64 format for encryption.</param>
        /// <param name="key">The secret key for the symmetric algorithm.</param>
        /// <param name="vector">The initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>) for the symmetric algorithm.</param>
        /// <returns>Deserialized object.</returns>
        public static T AesDecrypt<T>(string input, string key, string vector)
        {
            var data = Convert.FromBase64String(input);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(vector);

                using (var encryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var msEncrypt = new MemoryStream(data))
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Read))
                        {
                            using (var swEncrypt = new StreamReader(csEncrypt))
                            {
                                var output = swEncrypt.ReadToEnd();
                                return JsonConvert.DeserializeObject<T>(output);
                            }
                        }
                    }
                }
            }
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

        /// <summary>
        /// Add query <paramref name="key"/> and <paramref name="value"/> to <paramref name="url"/>. 
        /// </summary>
        /// <remarks>Parameter <paramref name="value"/> will be encoded.</remarks>
        /// <param name="url">The source url.</param>
        /// <param name="key">The query key.</param>
        /// <param name="value">The query value.</param>
        /// <returns><paramref name="url"/> with added query param.</returns>
        public static string SetQuery(Uri url, string key, string value)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[key] = value;
            uriBuilder.Query = GetQueryString(query);
            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// Add query <paramref name="key"/> and <paramref name="value"/> to <paramref name="url"/>. 
        /// </summary>
        /// <remarks>Parameter <paramref name="value"/> will be encoded.</remarks>
        /// <param name="url">The source url.</param>
        /// <param name="key">The query key.</param>
        /// <param name="value">The query value.</param>
        /// <returns><paramref name="url"/> with added query param.</returns>
        public static string SetQuery(string url, string key, string value)
        {
            return SetQuery(new Uri(url), key, value);
        }

        public static string SetQuery(string url, NameValueCollection query)
        {
            var newUrl = url;

            if (query?.Count > 0)
            {
                foreach (var key in query.AllKeys)
                {
                    newUrl = SetQuery(newUrl, key, query[key]);
                }
            }

            return newUrl;
        }

        public static Uri RemoveQuery(Uri url, string key)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Remove(key);
            uriBuilder.Query = GetQueryString(query);
            return uriBuilder.Uri;
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
        public static string GetUniqueKey(ILoginTypeModel loginType, OfferModel offer)
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
        /// Gets the unique MD5 hash for template document.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>Hash from merged values: "<see cref="OfferAttachmentXmlModel.IdAttach"/> + <see cref="OfferAttachmentXmlModel.Group"/> + <see cref="OfferAttachmentXmlModel.Template"/> + <see cref="OfferAttachmentXmlModel.Product"/> + <see cref="OfferAttachmentXmlModel.Description"/>".</returns>
        [ExcludeFromCodeCoverage]
        public static string GetUniqueKey(OfferAttachmentXmlModel template)
        {
            var data = template.IdAttach + template.Group + template.Template + template.Product + template.Description;
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

        public static string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var output = RegexHtml.Replace(input, string.Empty);
            return output;
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
            text = GetNonEmptyStringOrNull(text);

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

        public static string GetReplacedTextTokens(string text, IDictionary<string, string> textParameters, DateTime expirationDate)
        {
            var result = GetReplacedTextTokens(text, textParameters);
            var date = expirationDate.ToString("dd. MM. yyyy");
            result = result?.Replace("{EXPIRATION_DATE}", date);
            return result;
        }

        public static T[] GetUpdated<T>(T[] array, T newItem)
        {
            var list = new List<T>(array);
            list.Add(newItem);
            return list.ToArray();
        }

        public static NameValueCollection GetUtmQueryParams(NameValueCollection query)
        {
            var filteredQuery = new NameValueCollection();

            foreach (var k in query.AllKeys)
            {
                if (k.StartsWith("utm_"))
                {
                    filteredQuery.Add(k, query[k]);
                }
            }

            return filteredQuery;
        }

        public static string GetNonEmptyStringOrNull(string input)
        {
            if (input == null)
            {
                return null;
            }

            var noSpaces = input.Replace(" ", string.Empty);

            if (string.IsNullOrEmpty(noSpaces))
            {
                return null;
            }

            var strippedInput = Utils.StripHtml(input)?.Replace("&nbsp;", string.Empty).Replace(" ", string.Empty);

            if (string.IsNullOrEmpty(strippedInput))
            {
                return null;
            }

            return input;
        }

        public static string[] GetMimeTypesFromExtensions(string[] extensions)
        {
            var files = new List<string>();

            if (extensions != null)
            {
                foreach (var extension in extensions)
                {
                    var ext = extension.Trim('.');
                    var file = "file." + ext;
                    files.Add(file);
                }
            }

            return GetMimeTypesFromFiles(files.ToArray());
        }

        public static string[] GetMimeTypesFromFiles(string[] files)
        {
            var list = new List<string>();

            if (files != null)
            {
                foreach (var file in files)
                {
                    list.Add(MimeMapping.GetMimeMapping(file));
                }
            }

            return list.ToArray();
        }
    }
}
