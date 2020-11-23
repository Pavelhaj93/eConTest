using System;
using System.Linq;
using System.Threading.Tasks;
using Consinloop;
using eContracting.ConsoleClient.Commands;
using eContracting.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eContracting.ConsoleClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
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
                    services.AddScoped<ISettingsReaderService, MemorySettingsReaderService>();
                    services.AddScoped<IOfferParserService, OfferParserService>();
                    services.AddScoped<IOfferAttachmentParserService, OfferAttachmentParserService>();
                    services.AddScoped<ICache, MemoryCacheService>();
                    services.AddScoped<IApiService, SapApiService>();
                    services.AddScoped<IOfferJsonDescriptor, OfferJsonDescriptor>();
                    services.AddSingleton<ILogger, ConsoleLogger>();
                    services.AddScopedCommand<GetOfferCommand>();
                    services.AddScopedCommand<GetFilesCommand>();
                    services.AddScopedCommand<CompareIdAttachCommand>();
                    services.AddScopedCommand<OfferToJsonCommand>();
                });
                await consinloop.Run();
            }
        }
    }
}
