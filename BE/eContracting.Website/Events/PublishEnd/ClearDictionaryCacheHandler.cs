using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Globalization;

namespace eContracting.Website.Events.PublishEnd
{
    public class ClearDictionaryCacheHandler
    {
        public void Run(object sender, EventArgs args)
        {
            Translate.ResetCache(true);
            Log.Info("Dictionary cache cleared", this);
        }
    }
}
