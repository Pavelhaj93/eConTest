using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Tests;
using eContracting.Website.Pipelines.Initialize;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sitecore.DependencyInjection;
using Sitecore.Pipelines;
using Xunit;

namespace eContracting.Website.Tests.Pipelines.Initialize
{
    public class RunStartupsProcessorTests
    {
        [Fact]
        public void RunStartups_Executes_IStartup_Instances()
        {
            var startup1called = false;
            var startup2called = false;
            var mockStartup1 = new Mock<IStartup>();
            mockStartup1.Setup(x => x.Initialize()).Callback(() => { startup1called = true; });
            var mockStartup2 = new Mock<IStartup>();
            mockStartup2.Setup(x => x.Initialize()).Callback(() => { startup2called = true; });

            var services = new ServiceCollection();
            services.AddSingleton<IStartup>(mockStartup1.Object);
            services.AddSingleton<IStartup>(mockStartup2.Object);
            services.AddSingleton<ILogger>(new MemoryLogger());

            using (var provider = services.BuildServiceProvider())
            {
                ServiceLocator.SetServiceProvider(provider);

                var processor = new RunStartupsProcessor();
                processor.RunStartups(new MemoryLogger());

                Assert.True(startup1called);
                Assert.True(startup2called);
            }
        }
    }
}
