using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class GetOfferCommand : BaseCommand
    {
        readonly ContextData Context;
        readonly IApiService ApiService;
        readonly ILogger Logger;

        public GetOfferCommand(
            IApiService apiService,
            ILogger logger,
            IConsole console,
            ContextData contextData) : base("nabidka", console)
        {
            this.ApiService = apiService;
            this.Logger = logger;
            this.Context = contextData;
        }

        [Execute]
        public async Task Execute(
            [Argument(Description = "unique identifier for an offer")]string guid)
        {
            //this.Logger.Suspend(true);
            var offer = await this.ApiService.GetOfferAsync(guid);

            if (offer == null)
            {
                this.Console.WriteLineWarning("No offer");
                return;
            }

            this.Console.WriteLine(JsonConvert.SerializeObject(offer, Formatting.Indented));
            //this.Logger.Suspend(false);
        }
    }
}
