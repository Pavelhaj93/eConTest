using System;
using System.Configuration;
using System.Linq;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Actum.RweClient
{
    public static class RweHelpers
    {
        public static string ReadConfig(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? String.Empty;
        }

        public static string GetPath(Sitecore.Data.Fields.LinkField lf)
        {
            switch (lf.LinkType.ToLower())
            {
                case "internal":
                    // Use LinkMananger for internal links, if link is not empty
                    return lf.TargetItem != null ? LinkManager.GetItemUrl(lf.TargetItem).Replace("/en/", "/") : string.Empty;
                case "media":
                    // Use MediaManager for media links, if link is not empty
                    return lf.TargetItem != null ? MediaManager.GetMediaUrl(lf.TargetItem) : string.Empty;
                case "external":
                    return lf.Url;
                case "anchor":
                    return !string.IsNullOrEmpty(lf.Anchor) ? "#" + lf.Anchor : string.Empty;
                case "mailto":
                    return lf.Url;
                case "javascript":
                    return lf.Url;
                default:
                    return lf.Url;
            }
        }

        public static Boolean IsAccountNumberValid(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            var account = input.TrimStart('0');

            return account.All(x => Char.IsDigit(x) || (x == '-' || x == '\\'));
        }
    }
}
