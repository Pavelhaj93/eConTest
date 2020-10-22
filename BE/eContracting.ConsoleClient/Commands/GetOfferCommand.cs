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
            var offer = await this.ApiService.GetOfferAsync(guid, OFFER_TYPES.NABIDKA);

            this.Console.WriteLine($"Birthday: {offer.Birthday}");
            this.Console.WriteLine($"Partner number: {offer.PartnerNumber}");
            this.Console.WriteLine($"Post number: {offer.PostNumber}");
            this.Console.WriteLine($"Post number consumption: {offer.PostNumberConsumption}");
            this.Console.WriteLine($"Is expired: {offer.OfferIsExpired}");
            this.Console.WriteLine($"Is accepted: {offer.IsAccepted}");
            this.Console.WriteLine($"Is retention: {offer.OfferIsRetention}");
            this.Console.WriteLine($"Is aquisition: {offer.OfferIsAcquisition}");
            this.Console.WriteLine($"Has GDPR: {offer.HasGDPR}");
            this.Console.WriteLine($"Attachments: {offer.Attachments.Length}");
        }
    }
}
