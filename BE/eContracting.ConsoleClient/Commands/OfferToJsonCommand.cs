using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class OfferToJsonCommand : BaseCommand
    {
        readonly IApiService ApiService;
        readonly IOfferJsonDescriptor OfferJsonDescriptor;
        readonly ILogger Logger;

        public OfferToJsonCommand(
            IApiService apiService,
            IOfferJsonDescriptor offerJsonDescriptor,
            ILogger logger,
            IConsole console) : base("json", console)
        {
            this.ApiService = apiService;
            this.OfferJsonDescriptor = offerJsonDescriptor;
            this.Logger = logger;
            this.Description = "converts an offer into JSON for Web API";
        }

        [Execute]
        public async Task Execute([Argument(Description = "unique identifier for an offer")] string guid)
        {
            var offer = await this.ApiService.GetOfferAsync(guid);

            if (offer == null)
            {
                this.Console.WriteLineError("Offer not found");
            }

            if (offer.IsAccepted)
            {
                var json = await this.OfferJsonDescriptor.GetAcceptedAsync(offer);
                this.Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
            }
            else
            {
                var json = await this.OfferJsonDescriptor.GetNewAsync(offer);
                this.Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
            }
        }
    }
}
