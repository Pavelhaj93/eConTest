using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.SessionEnd;
using Sitecore.Shell.Framework.Commands.System;

namespace eContracting.Website.Pipelines.SessionEnd
{
    public class ClearUserCacheProcessor
    {
        public void Process(SessionEndArgs endArgs)
        {
            try
            {
                var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserFileCacheService>();
                cache.Clear(new Models.DbSearchParameters(null, null, endArgs.Context.Session.SessionID));
            }
            catch (Exception ex)
            {
                Log.Error("Cannot clear user file cache", ex, this);
            }
        }
    }
}
