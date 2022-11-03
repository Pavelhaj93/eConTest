using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Sites;

namespace eContracting
{
    /// <summary>
    /// Wrapper over 'Sitecore.Context'.
    /// </summary>
    /// <seealso cref="eContracting.IContextWrapper" />
    [ExcludeFromCodeCoverage]
    public class SitecoreContextWrapper : IContextWrapper
    {
        /// <inheritdoc/>
        public string GetBrowserAgent()
        {
            return HttpContext.Current?.Request?.UserAgent;
        }

        /// <inheritdoc/>
        public string GetIpAddress()
        {
            string value = HttpContext.Current?.Request?.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Split(',')[0];
            }
            else
            {
                value = HttpContext.Current?.Request?.ServerVariables["REMOTE_ADDR"];
            }

            return value;
        }

        /// <inheritdoc/>
        public string GetSetting(string name)
        {
            return Sitecore.Configuration.Settings.GetSetting(name);
        }

        /// <inheritdoc/>
        public string GetSiteRoot()
        {
            //return Sitecore.Context.Site.RootPath;
            return Constants.RootSitePath; // this will change only when another site will be created
        }

        /// <inheritdoc/>
        public string GetQueryValue(string key)
        {
            return HttpContext.Current?.Request?.QueryString[key];
        }

        /// <inheritdoc/>
        public bool IsEditMode()
        {
            return Sitecore.Context.PageMode.IsExperienceEditorEditing;
        }

        /// <inheritdoc/>
        public bool IsNormalMode()
        {
            return Sitecore.Context.PageMode.IsNormal;
        }

        /// <inheritdoc/>
        public bool IsPreviewMode()
        {
            return Sitecore.Context.PageMode.IsPreview;
        }

        /// <inheritdoc/>
        public void SetDisplayMode(DisplayMode model)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public HttpCookieCollection GetCookies()
        {
            return HttpContext.Current?.Request?.Cookies;
        }

        /// <inheritdoc/>
        public NameValueCollection GetQueryParams()
        {
            return HttpContext.Current?.Request?.QueryString ?? new NameValueCollection();
        }
    }
}
