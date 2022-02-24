using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Sites;

namespace eContracting.ConsoleClient
{
    class MemoryContextWrapper : IContextWrapper
    {
        public string GetBrowserAgent()
        {
            return "Console client";
        }

        public HttpCookieCollection GetCookies()
        {
            return new HttpCookieCollection();
        }

        public string GetIpAddress()
        {
            return "10.0.0.0";
        }

        public string GetQueryValue(string key)
        {
            return "value";
        }

        public string GetSetting(string name)
        {
            return name;
        }

        public string GetSiteRoot()
        {
            return "/sitecore/content";
        }

        public bool IsEditMode()
        {
            return false;
        }

        public bool IsNormalMode()
        {
            return true;
        }

        public bool IsPreviewMode()
        {
            return false;
        }

        public void SetDisplayMode(DisplayMode model)
        {
        }
    }
}
