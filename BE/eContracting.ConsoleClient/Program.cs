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

            var configuration = new GlobalConfiguration();
            // dev
            //configuration.ServiceUrl = "http://lv423075.aci3.rwegroup.cz:8001/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            // test
            //configuration.ServiceUrl = "http://wd-wcc.rwe-services.cz:8112/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            // test
            configuration.ServiceUrl = "https://wd-wcc.rwe-services.cz:8110/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            configuration.ServiceUser = "UkZDX1NJVEVDT1JF";
            configuration.ServicePassword = "QWRIYjI0Nyo=";

            using (var consinloop = new ConsinloopRunner(args))
            {
                consinloop.ConfigureServices((services) =>
                {
                    services.AddSingleton<GlobalConfiguration>(configuration);
                    services.AddSingleton<ContextData>();
                    services.AddScoped<IServiceFactory, ServiceFactory>();
                    services.AddScoped<ISettingsReaderService, MemorySettingsReaderService>();
                    services.AddScoped<IOfferParserService, OfferParserService>();
                    services.AddScoped<IOfferAttachmentParserService, OfferAttachmentParserService>();
                    services.AddScoped<IUserDataCacheService, MemoryUserDataCacheService>();
                    services.AddScoped<IUserFileCacheService, MemoryUserFileCacheService>();
                    services.AddScoped<ITextService, MemoryTextService>();
                    services.AddScoped<IOfferService, OfferService>();
                    services.AddScoped<ISitecoreContext>(service => { return sitecoreContext; });
                    services.AddScoped<IOfferJsonDescriptor, FixedOfferJsonDescriptor>();
                    services.AddSingleton<ILogger, ConsoleLogger>();
                    services.AddScopedCommand<AnalyzeOfferCommand>();
                    services.AddScopedCommand<GetOfferCommand>();
                    services.AddScopedCommand<GetFilesCommand>();
                    services.AddScopedCommand<GetTextsCommand>();
                    services.AddScopedCommand<CompareIdAttachCommand>();
                    services.AddScopedCommand<GetOfferJsonCommand>();
                    services.AddScopedCommand<SwitchServerCommand>();
                });
                await consinloop.Run();
            }
        }
    }
}
