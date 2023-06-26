using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class GetOfferJsonSummaryCommand : BaseCommand
    {
        readonly IOfferService ApiService;
        readonly IOfferJsonDescriptor JsonDescriptor;
        readonly ILogger Logger;

        public GetOfferJsonSummaryCommand(
            IOfferService apiService,
            IOfferJsonDescriptor jsonDescriptor,
            ILogger logger,
            IConsole console) : base("json-summary", console)
        {
            this.ApiService = apiService;
            this.JsonDescriptor = jsonDescriptor;
            this.Logger = logger;
        }

        [Execute]
        public void Execute(
            [Argument(Description = "unique identifier for an offer")] string guid)
        {
            var user = new UserCacheDataModel();
            var offer = this.ApiService.GetOffer(guid, user);

            if (offer == null)
            {
                this.Console.WriteLineWarning("No offer");
                return;
            }

            var json = this.JsonDescriptor.GetSummary2(offer, user);
            this.Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
        }
    }
}
