using System.Collections.Generic;
using System.Linq;
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
    class CompareIdAttachCommand : BaseCommand
    {
        readonly ContextData Context;
        readonly OfferService ApiService;
        readonly OfferAttachmentParserService AttachmentParserService;
        readonly ILogger Logger;

        public CompareIdAttachCommand(
            IOfferService apiService,
            IOfferAttachmentParserService attachmentParserService,
            ILogger logger,
            IConsole console,
            ContextData contextData) : base("idattach", console)
        {
            this.ApiService = apiService as OfferService;
            this.AttachmentParserService = attachmentParserService as OfferAttachmentParserService;
            this.Logger = logger;
            this.Context = contextData;
            this.AliasKey = "id";
            this.Description = "compares attribute IDATTACH from offer with PDFs";
        }

        [Execute]
        public async Task Execute([Argument(Description = "unique identifier for an offer")] string guid)
        {
            using (new ConsoleLoggerSuspender(this.Logger))
            {
                var user = new UserCacheDataModel();
                var offer = this.ApiService.GetOffer(guid, user);

                if (offer == null)
                {
                    this.Console.WriteLineError("Offer not found");
                    return;
                }

                if (offer.First().Documents.Length == 0)
                {
                    this.Console.WriteLineError("No attachment(s) found");
                    return;
                }

                var files = this.ApiService.GetFiles(offer.Guid, false);

                if (files == null)
                {
                    this.Console.WriteLine("No files found");
                    return;
                }

                this.AttachmentParserService.MakeCompatible(offer.First(), files);

                Utils.CompareIdAttach(this.Console, offer.First(), files);
            }
        }
    }
}
