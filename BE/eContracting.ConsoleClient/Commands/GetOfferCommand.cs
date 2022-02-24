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
    class GetOfferCommand : BaseCommand
    {
        readonly IOfferService ApiService;

        public GetOfferCommand(
            IOfferService apiService,
            IConsole console) : base("offer", console)
        {
            this.ApiService = apiService;
            this.AliasKey = "o";
            this.Description = "get basic data about an offer";
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

            this.Console.WriteLine(JsonConvert.SerializeObject(offer, Formatting.Indented));
        }
    }
}
