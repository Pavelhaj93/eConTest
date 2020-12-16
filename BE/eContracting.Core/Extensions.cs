using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Glass.Mapper.Sc;

namespace eContracting
{
    /// <summary>
    /// All extensions available in core functionality.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets children of <typeparamref name="T"/> from folder <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">Sitecore item model type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="path">The path.</param>
        /// <returns>List of found children or empty list.</returns>
        public static IEnumerable<T> GetItems<T>(this ISitecoreService service, string path)
        {
            return service.GetItem<FolderItemModel<T>>(path)?.Children ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Gets the counter integer value. If attribute <see cref="Constants.OfferAttributes.COUNTER"/> is not valid, returns 100;
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Position from <see cref="Constants.OfferAttributes.COUNTER"/>, otherwise 100</returns>
        public static int GetCounter(this ZCCH_ST_FILE file)
        {
            var attr = file.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.OfferAttributes.COUNTER);

            if (attr != null && int.TryParse(attr.ATTRVAL, out int position))
            {
                return position;
            }

            return Constants.FileAttributeDefaults.COUNTER;
        }

        /// <summary>
        /// Gets value of IDATTACH from the <paramref name="file"/> if exists.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Value or null.</returns>
        public static string GetIdAttach(this ZCCH_ST_FILE file)
        {
            return file.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.TYPE)?.ATTRVAL;
        }

        /// <summary>
        /// Merges two dictionaries.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="second">The second.</param>
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> second)
        {
            if (source == null || second == null)
            {
                return;
            }

            foreach (KeyValuePair<TKey, TValue> item in second)
            {
                source[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// Determines whether the specified key has value any value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key has value; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasValue(this IDictionary<string, string> source, string key)
        {
            if (source == null || source.Count == 0)
            {
                return false;
            }

            if (!source.ContainsKey(key))
            {
                return false;
            }

            var value = source[key]?.Trim();

            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
