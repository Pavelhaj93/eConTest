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
        readonly OfferParserService OfferParser;
        readonly OfferAttachmentParserService AttachmentParser;
        readonly IOptions<GlobalConfiguration> Options;
        readonly ILogger Logger;

        public GetOfferCommand(OfferParserService offerParser,
            OfferAttachmentParserService attachmentParser,
            ILogger logger,
            IOptions<GlobalConfiguration> options,
            IConsole console,
            ContextData contextData) : base("nabidka", console)
        {
            this.OfferParser = offerParser;
            this.AttachmentParser = attachmentParser;
            this.Logger = logger;
            this.Options = options;
            this.Context = contextData;
        }

        [Execute]
        public async Task Execute(
            [Argument(Description = "unique identifier for an offer")]string guid)
        {
            var options = new CacheApiServiceOptions(this.Options.Value.ServiceUser, this.Options.Value.ServicePassword, this.Options.Value.ServiceUrl);
            var service = new CacheApiService(options, this.OfferParser, this.AttachmentParser, this.Logger);
            var offer = await service.GetOfferAsync(guid, "NABIDKA");

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
