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

        public static string GetLogoutLinkUrl(this IPageHeaderModel model)
        {
            if (Uri.TryCreate(model.LogoutLink?.Url, UriKind.RelativeOrAbsolute, out Uri linkUrl))
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.LogoutLink.Query))
                    {
                        if (linkUrl.ToString().Contains("?"))
                            linkUrl = new Uri($"{linkUrl}&{model.LogoutLink.Query}");
                        else
                            linkUrl = new Uri($"{linkUrl}?{model.LogoutLink.Query}");
                    }
                    return linkUrl.ToString();
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("PageHeaderModel: Error when processing LogoutLink.", ex, model);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string GetImageLinkUrl(this IPageHeaderModel model)
        {
            return model.ImageLink?.Url;
        }
    }
}
