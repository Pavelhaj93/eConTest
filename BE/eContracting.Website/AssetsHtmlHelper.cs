using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using eContracting.Website.Areas.eContracting2.Models;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Mvc;
using Sitecore.Web.UI.HtmlControls;

namespace eContracting.Website
{    
    public class AssetsHtmlHelper
    {
        protected readonly HtmlHelper HtmlHelper;
        private readonly static Dictionary<string, CacheValue> _hashedPathCache = new Dictionary<string, CacheValue>();


        public AssetsHtmlHelper(HtmlHelper htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }

        private struct CacheValue
        {
            public string FilePath { get; }
            public long TimeStamp { get; }

            public CacheValue(string filePath, long timeStamp)
            {
                FilePath = filePath;
                TimeStamp = timeStamp;
            }
        }


        public HtmlString PathWithHash(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException(nameof(virtualPath));

            string physicalPath = HostingEnvironment.MapPath(virtualPath);
            FileInfo file = new FileInfo(physicalPath);

            if (!file.Exists)
            {
                Sitecore.Diagnostics.Log.Error($"File '{physicalPath}' referenced as '{virtualPath}' does not exist.",this);
                return new HtmlString(virtualPath);
            }

            long timeStamp = file.LastWriteTimeUtc.ToBinary();

            CacheValue value;
            if (!_hashedPathCache.TryGetValue(file.FullName, out value) || value.TimeStamp != timeStamp)
            {
                lock (_hashedPathCache)
                {
                    if (!_hashedPathCache.TryGetValue(file.FullName, out value) || value.TimeStamp != timeStamp)
                    {
                        // generate hash
                        string hash = ComputeFileHash(file);
                        string hashedPath = virtualPath + (virtualPath.Contains('?') ? '&' : '?') + "h=" + Uri.EscapeDataString(hash);
                        value = new CacheValue(hashedPath, timeStamp);
                        _hashedPathCache[file.FullName] = value;
                    }
                }
            }

            return new HtmlString(value.FilePath);
        }

        private static string ComputeFileHash(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            byte[] hash;
            using (var md5 = MD5.Create())
            using (var stream = file.OpenRead())
                hash = md5.ComputeHash(stream);

            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

    }
}
