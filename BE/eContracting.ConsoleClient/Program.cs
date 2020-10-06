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
                    services.AddOptions<GlobalConfiguration>();
                    services.AddSingleton<ContextData>();
                    services.AddScoped<OfferParserService>();
                    services.AddScoped<OfferAttachmentParserService>();
                    services.AddSingleton<ILogger, ConsoleLogger>();
                });

                consinloop.RegisterCommand<GetOfferCommand>();
                consinloop.RegisterCommand<GetXmlCommand>();
                consinloop.RegisterCommand<GetFilesCommand>();

                await consinloop.Run();
            }
        }
    }
}
