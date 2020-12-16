using System;
using System.Linq;
using System.Threading.Tasks;
using Consinloop;
using eContracting.ConsoleClient.Commands;
using eContracting.Services;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace eContracting.ConsoleClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var fakeSitecoreContent = new Mock<ISitecoreContext>();
            var sitecoreContext = fakeSitecoreContent.Object;

            using (var consinloop = new ConsinloopRunner(args))
            {
                consinloop.ConfigureServices((services) =>
                {
                    services.AddOptions<GlobalConfiguration>().Configure((config) =>
                    {
                        //config.ServiceUrl = "http://lv423075.aci3.rwegroup.cz:8001/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
                        config.ServiceUrl = "http://wd-wcc.rwe-services.cz:8112/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
                        config.ServiceUser = "UkZDX1NJVEVDT1JF";
                        config.ServicePassword = "QWRIYjI0Nyo=";
                    });
                    services.AddSingleton<ContextData>();
                    services.AddScoped<IOfferServiceFactory, OfferServiceFactory>();
                    services.AddScoped<ISettingsReaderService, MemorySettingsReaderService>();
                    services.AddScoped<IOfferParserService, OfferParserService>();
                    services.AddScoped<IOfferAttachmentParserService, OfferAttachmentParserService>();
                    services.AddScoped<IUserDataCacheService, MemoryUserDataCacheService>();
                    services.AddScoped<IUserFileCacheService, MemoryUserFileCacheService>();
                    services.AddScoped<IOfferService, OfferService>();
                    services.AddScoped<ISitecoreContext>(service => { return sitecoreContext; });
                    services.AddScoped<IOfferJsonDescriptor, FixedOfferJsonDescriptor>();
                    services.AddSingleton<ILogger, ConsoleLogger>();
                    services.AddScopedCommand<GetOfferCommand>();
                    services.AddScopedCommand<GetFilesCommand>();
                    services.AddScopedCommand<CompareIdAttachCommand>();
                    services.AddScopedCommand<GetOfferJsonCommand>();
                });
                await consinloop.Run();
            }
        }
    }
}
