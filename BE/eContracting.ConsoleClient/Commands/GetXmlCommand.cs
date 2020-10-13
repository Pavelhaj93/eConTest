using System.Linq;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Services;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient.Commands
{
    class GetXmlCommand : BaseCommand
    {
        readonly ContextData Context;
        readonly IApiService ApiService;
        readonly ILogger Logger;

        public GetXmlCommand(
            IApiService apiService,
            ILogger logger,
            IConsole console,
            ContextData contextData) : base("xml", console)
        {
            this.ApiService = apiService;
            this.Logger = logger;
            this.Context = contextData;
        }

        [Execute]
        public async Task Execute([Argument(Description = "unique identifier for an offer")] string guid)
        {
            var xmls = await this.ApiService.GetXmlAsync(guid);

            foreach (var xml in xmls)
            {
                this.Console.WriteLine($"Name: {xml.Name}");
            }
        }
    }
}
