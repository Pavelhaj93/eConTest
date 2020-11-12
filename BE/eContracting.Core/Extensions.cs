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

            return 100;
        }

        /// <summary>
        /// Merges two dictionaries.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="second">The second.</param>
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> second)
        {
            if (dictionary == null || second == null)
            {
                return;
            }

            foreach (KeyValuePair<TKey, TValue> item in second)
            {
                dictionary[item.Key] = item.Value;
            }
        }
    }
}
