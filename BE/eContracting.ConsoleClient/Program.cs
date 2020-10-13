﻿using System;
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
                    services.AddScoped<ISettingsReaderService, MemorySettingsReaderService>();
                    services.AddScoped<IOfferParserService, OfferParserService>();
                    services.AddScoped<IOfferAttachmentParserService, OfferAttachmentParserService>();
                    services.AddScoped<IApiService, CacheApiService>();
                    services.AddSingleton<ILogger, ConsoleLogger>();
                    services.AddScopedCommand<GetOfferCommand>();
                    services.AddScopedCommand<GetXmlCommand>();
                    services.AddScopedCommand<GetFilesCommand>();
                });
                await consinloop.Run();
            }
        }
    }
}
