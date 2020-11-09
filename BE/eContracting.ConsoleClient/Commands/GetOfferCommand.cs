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
            this.Logger.Suspend(true);
            var offer = await this.ApiService.GetOfferAsync(guid, OFFER_TYPES.NABIDKA);

            if (offer == null)
            {
                this.Console.WriteLineWarning("No offer");
                return;
            }
            
            this.Console.WriteLine($"Process: {offer.Process}");
            this.Console.WriteLine($"Type: {offer.ProcessType}");
            this.Console.WriteLine($"Birthday: {offer.Birthday}");
            this.Console.WriteLine($"Partner number: {offer.PartnerNumber}");
            this.Console.WriteLine($"Post number: {offer.PostNumber}");
            this.Console.WriteLine($"Post number consumption: {offer.PostNumberConsumption}");
            this.Console.WriteLine($"Is expired: {offer.OfferIsExpired}");
            this.Console.WriteLine($"Is accepted: {offer.IsAccepted}");

            var files = await this.ApiService.GetAttachmentsAsync(guid);
            var filesCount = files?.Length ?? 0;
            this.Console.WriteLine($"Files: {filesCount}");
            
            if (filesCount > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    var f = files[i];
                    this.Console.WriteLine($" - {f.FileName}:");
                    this.Console.WriteLine($"   Size:          {f.SizeLabel}");
                    this.Console.WriteLine($"   FileType:      {f.Template}");
                    this.Console.WriteLine($"   FileNumber:    {f.FileNumber}");
                    //this.Console.WriteLine($"   Group: {f.Group}");
                    this.Console.WriteLine($"   Signed:        {f.SignedVersion}");
                    this.Console.WriteLine($"   Sign Required: {f.SignRequired}");
                }
            }

            this.Logger.Suspend(false);
        }
    }
}
