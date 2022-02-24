using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Models;
using eContracting.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class GetTextsCommand : BaseCommand
    {
        readonly ContextData Context;
        readonly IOfferService ApiService;
        readonly ILogger Logger;

        public GetTextsCommand(
            IOfferService apiService,
            ILogger logger,
            IConsole console,
            ContextData contextData) : base("texts", console)
        {
            this.ApiService = apiService;
            this.Logger = logger;
            this.Context = contextData;
            this.AliasKey = "t";
            this.Description = "get text data from an offer";
        }

        [Execute]
        public void Execute(
            [Argument(Description = "unique identifier for an offer")]string guid)
        {
            var user = new UserCacheDataModel();
            var offer = this.ApiService.GetOffer(guid, user);

            if (offer == null)
            {
                this.Console.WriteLineWarning("No offer");
                return;
            }

            this.Console.WriteLine(JsonConvert.SerializeObject(offer.TextParameters, Formatting.Indented));
        }
    }
}
