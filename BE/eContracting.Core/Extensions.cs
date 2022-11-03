using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Glass.Mapper.Sc;
using Sitecore.Pipelines.RenderField;
using Sitecore.Web.UI.XamlSharp.Xaml;

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
        public static IEnumerable<T> GetItems<T>(this ISitecoreService service, string path) where T : IBaseSitecoreModel
        {
            return service.GetItem<IFolderItemModel<T>>(path)?.Children ?? Enumerable.Empty<T>();
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
        /// Determines whether the specified key has any value.
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

        public static double GetDoubleValue(this IDictionary<string, string> source, string key)
        {
            if (!source.HasValue(key))
            {
                return default(double);
            }

            var d = source[key].Replace(" ", string.Empty).Replace(',', '.');

            if (double.TryParse(d, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }

            return default(double);
        }

        public static TValue GetValueOrDefault<TValue>(this IDictionary<string, TValue> source, string key)
        {
            return source.TryGetValue(key, out TValue value) ? value : default(TValue);
        }

        /// <summary>
        /// Determinates whether the specified key is value higher or equal than <paramref name="minValue"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="minValue">Minimal value.</param>
        public static bool HasValue(this IDictionary<string, string> source, string key, double minValue)
        {
            if (!source.HasValue(key))
            {
                return false;
            }

            var d = source[key].Replace(" ", string.Empty).Replace(',', '.');

            if (double.TryParse(d, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                if (value >= minValue)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsVisible(this IDictionary<string, string> source, string keyPrefix)
        {
            var key = keyPrefix + "_VISIBILITY";

            if (!source.ContainsKey(key))
            {
                return true;
            }

            return source[key] != Constants.HIDDEN;
        }

        /// <summary>
        /// Gets differences between double values.
        /// </summary>
        public static bool IsDifferentDoubleValue(this IDictionary<string, string> source, string left, string right)
        {
            var leftValue = source.GetDoubleValue(left);
            var rightValue = source.GetDoubleValue(right);
            return leftValue != rightValue;
        }

        /// <summary>
        /// Gets a value indicating whether this definition is default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDefault(this IDefinitionCombinationModel model)
        {
            return string.IsNullOrEmpty(model.Process?.Code) || string.IsNullOrEmpty(model.ProcessType?.Code);
        }

        /// <summary>
        /// Gets attributes collection from checked values in <paramref name="model"/>.
        /// </summary>
        public static NameValueCollection GetXmlAttributes(this IDefinitionCombinationModel model)
        {
            var collection = new NameValueCollection();

            if (model.IsOrderOrigin)
            {
                collection.Add(Constants.OfferAttributes.ORDER_ORIGIN, "2");
            }

            if (model.IsNoProductChange)
            {
                collection.Add(Constants.OfferAttributes.NO_PROD_CHNG, Constants.CHECKED);
            }

            return collection;
        }

        /// <summary>
        /// Compares if <paramref name="source"/> contains all keys and values from <paramref name="matches"/>.
        /// </summary>
        /// <param name="source">Collection as source for comparison.</param>
        /// <param name="matches">Collection to compare with <paramref name="source"/>.</param>
        /// <returns>True if <paramref name="source"/> contains all keys and values from <paramref name="matches"/>.</returns>
        public static bool Contains(this NameValueCollection source, NameValueCollection matches)
        {
            if (matches?.Count == 0)
            {
                return false;
            }

            var targetKeys = source.AllKeys;

            var count = 0;

            foreach (var key in matches.AllKeys)
            {
                var value = matches[key];

                if (source.AllKeys.Contains(key) && source[key] == value)
                {
                    count++;
                }
            }

            return count == matches.Count;
        }

        public static TimeSpan GetDelayAfterFailedAttemptsTimeSpan(this IPageLoginModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.DelayAfterFailedAttempts))
            {
                TimeSpan value;

                if (TimeSpan.TryParse(model.DelayAfterFailedAttempts, out value))
                {
                    return value;
                }
            }

            return new TimeSpan(0, 15, 0);
        }

        /// <summary>
        /// Gets the color in hexadecimal format with leading <c>#</c>.
        /// </summary>
        public static string GetColor(this IPromoBoxModel model)
        {
            var value = model.Color?.Value;

            if (string.IsNullOrEmpty(value))
            {
                value = "64b42d";
            }

            return "#" + value;
        }

        public static string GetLinkUrl(this IPromoBoxModel model)
        {
            return model.Link?.Url;
        }

        public static string GetLinkText(this IPromoBoxModel model)
        {
            return model.Link?.Text ?? string.Empty;
        }
        public static string GetLinkTitle(this IPromoBoxModel model)
        {
            return model.Link?.Title ?? string.Empty;
        }

        public static string[] AllowedDocumentTypesList(this ISiteSettingsModel model)
        {
            var list = new List<string>();

            var ar = model.AllowedDocumentTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < ar.Length; i++)
            {
                var v = ar[i].Trim();

                if (!string.IsNullOrEmpty(v))
                {
                    list.Add(v);
                }
            }

            return list.ToArray();
        }

        public static string GetDisclaimerLinkUrl(this IPageFooterModel model)
        {
            return model.DisclaimerLink?.Url;
        }
        
        public static string GetImageLinkUrl(this IPageHeaderModel model)
        {
            return model.ImageLink?.Url;
        }

        /// <summary>
        /// Gets property value by <paramref name="fieldName"/>.
        /// </summary>
        /// <typeparam name="T">Type of expected value.</typeparam>
        /// <param name="sitecoreModel">Sitecore base model.</param>
        /// <param name="fieldName">Name of field.</param>
        /// <returns>Value or null.</returns>
        public static T GetFieldValueByName<T>(this IBaseSitecoreModel sitecoreModel, string fieldName)
        {
            if (sitecoreModel == null || string.IsNullOrEmpty(fieldName))
            {
                return default(T);
            }

            var type = sitecoreModel.GetType();
            var properties = type.GetProperties();
            
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                if (property.Name == fieldName)
                {
                    return (T)property.GetValue(sitecoreModel);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Determinates if any of <see cref="IStepsModel.Steps"/> has <see cref="IStepModel.IsSelected"/> equals true.
        /// </summary>
        /// <param name="steps">Container for steps.</param>
        public static bool AnySelected(this IStepsModel steps)
        {
            return steps?.Steps?.Any(x => x.IsSelected) ?? false;
        }
    }
}
