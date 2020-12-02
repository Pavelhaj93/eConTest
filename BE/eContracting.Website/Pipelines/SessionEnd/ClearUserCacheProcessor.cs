﻿using System;
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
            var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserCacheService>();
            cache.Clear();
        }
    }
}
