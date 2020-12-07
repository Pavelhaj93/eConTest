using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Pipelines.SessionEnd;

namespace eContracting.Website.Pipelines.SessionEnd
{
    public class ClearUserCacheProcessor
    {
        public void Process(SessionEndArgs endArgs)
        {
            var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserFileCacheService>();
            var task = cache.ClearAsync(new Models.DbSearchParameters(null, null, endArgs.Context.Session.SessionID));
            task.Wait();
        }
    }
}
