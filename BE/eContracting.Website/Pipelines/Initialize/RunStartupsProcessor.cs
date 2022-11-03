using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Pipelines;

namespace eContracting.Website.Pipelines.Initialize
{
    public class RunStartupsProcessor
    {
        [ExcludeFromCodeCoverage]
        public void Process(PipelineArgs args)
        {
            var logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.RunStartups(logger);
        }

        protected internal void RunStartups(ILogger logger)
        {
            var startups = ServiceLocator.ServiceProvider.GetServices<IStartup>();

            foreach (var startup in startups)
            {
                try
                {
                    startup.Initialize();
                }
                catch (Exception ex)
                {
                    logger.Fatal(null, $"Cannot initialize {nameof(IStartup)} service", ex);
                }
            }
        }
    }
}
