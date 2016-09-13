using System;
using System.Configuration;

namespace rweClient
{
    public static class rweHelpers
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
                    return lf.TargetItem != null ? Sitecore.Links.LinkManager.GetItemUrl(lf.TargetItem).Replace("/en/", "/") : string.Empty;
                case "media":
                    // Use MediaManager for media links, if link is not empty
                    return lf.TargetItem != null ? Sitecore.Resources.Media.MediaManager.GetMediaUrl(lf.TargetItem) : string.Empty;
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
    }
}
