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
            //configuration.ServiceUrl = "https://wti.rwe-services.cz:51018/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            // test
            //configuration.ServiceUrl = "https://wti.rwe-services.cz:51012/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            // test
            configuration.ServiceUrl = "https://wti.rwe-services.cz:51018/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            configuration.ServiceUser = "UkZDX1NJVEVDT1JF";
            configuration.ServicePassword = "QWRIYjI0Nyo=";

            configuration.ServiceSignUrl = "https://wti.rwe-services.cz:51018/sap/bc/srt/xip/sap/zcrm_sign_stamp_merge/100/crm_sign_stamp_merge/crm_sign_stamp_merg";
            configuration.ServiceSignUser = "UkZDX1NJVEVDT1JF";
            configuration.ServiceSignPassword = "QWRIYjI0Nyo=";

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
                    services.AddScoped<ISignService, FileSignService>();
                    services.AddScoped<ISitecoreContext>(service => { return sitecoreContext; });
                    services.AddScoped<IOfferJsonDescriptor, OfferJsonDescriptor>();
                    services.AddScoped<IContextWrapper, MemoryContextWrapper>();
                    services.AddSingleton<ILogger, ConsoleLogger>();
                    services.AddScopedCommand<AnalyzeOfferCommand>();
                    services.AddScopedCommand<GetOfferCommand>();
                    services.AddScopedCommand<GetFilesCommand>();
                    services.AddScopedCommand<GetTextsCommand>();
                    services.AddScopedCommand<CompareIdAttachCommand>();
                    services.AddScopedCommand<GetOfferJsonCommand>();
                    services.AddScopedCommand<SwitchServerCommand>();
                    services.AddScopedCommand<AcceptOfferCommand>();
                    services.AddScopedCommand<GetLoginValuesCommand>();
                });
                await consinloop.Run();
            }
        }
    }
}
