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

namespace eContracting.ConsoleClient.OfferActions
{
    class GetOfferAction : BaseOfferAction
    {
        public override string Key { get; } = "NABIDKA";
        readonly OfferParserService OfferParser;
        readonly ILogger Logger;

        public GetOfferAction(
            OfferParserService offerParser,
            ILogger logger,
            IConsole console,
            IOptions<GlobalConfiguration> options,
            ContextData contextData) : base(console, options, contextData)
        {
            this.OfferParser = offerParser;
            this.Logger = logger;
        }

        [Execute]
        public async Task Execute(string guid = null)
        {
            var g = guid ?? this.Context.Guid;
            var options = new CacheApiServiceOptions(this.Options.Value.ServiceUser, this.Options.Value.ServicePassword, this.Options.Value.ServiceUrl);
            var service = new CacheApiService(options, this.OfferParser, this.Logger);
            var offer = await service.GetOffer(g, this.Key);

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
