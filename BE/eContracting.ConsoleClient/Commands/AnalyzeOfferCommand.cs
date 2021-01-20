using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Services;

namespace eContracting.ConsoleClient.Commands
{
    class AnalyzeOfferCommand : BaseCommand
    {
        readonly OfferService ApiService;
        readonly ILogger Logger;
        readonly IOfferJsonDescriptor OfferDescriptor;

        public AnalyzeOfferCommand(
            IOfferService apiService,
            IOfferJsonDescriptor offerDescriptor,
            ILogger logger,
            IConsole console)
            : base("analyze", console)
        {
            this.ApiService = apiService as OfferService;
            this.OfferDescriptor = offerDescriptor;
            this.Logger = logger;
            this.AliasKey = "a";
            this.Description = "make full analyzis of the offer";
        }

        [Execute]
        public async Task Execute(
            [Argument(Description = "unique identifier for an offer")] string guid,
            [Argument] bool debug = true)
        {
            using (new ConsoleLoggerSuspender(this.Logger, !debug))
            {
                this.Console.WriteLine();

                var response = this.ApiService.GetResponse(guid, OFFER_TYPES.NABIDKA);

                this.Console.WriteLine("Header:");
                this.Console.WriteLine(" CCHKEY = " + response.Response.ES_HEADER.CCHKEY);
                this.Console.WriteLine(" CCHSTAT = " + response.Response.ES_HEADER.CCHSTAT);
                this.Console.WriteLine(" CCHTYPE = " + response.Response.ES_HEADER.CCHTYPE);
                this.Console.WriteLine();

                this.Console.WriteLine("Attributes:");

                for (int i = 0; i < response.Response.ET_ATTRIB.Length; i++)
                {
                    var attr = response.Response.ET_ATTRIB[i];

                    this.Console.WriteLine($" {attr.ATTRID} = {attr.ATTRVAL}");
                }

                this.Console.WriteLine();

                var offer = this.ApiService.GetOffer(response, true);

                if (offer == null)
                {
                    this.Console.WriteLineError("Offer not found");
                    return;
                }

                this.Console.WriteLine("Version: " + offer.Version);

                if (offer.Documents.Length == 0)
                {
                    this.Console.WriteLineError("No attachment(s) found");
                    return;
                }

                this.Console.WriteLine();
                this.Console.WriteLine("Loading files ...");
                this.Console.WriteLine();

                var files = this.ApiService.GetFiles(offer.Guid, false);

                if (files == null)
                {
                    this.Console.WriteLine("No files found");
                    return;
                }

                Utils.CompareIdAttach(this.Console, offer, files);

                this.Console.WriteLine();

                var model = this.OfferDescriptor.GetNew(offer);

                if (model.Perex != null)
                {
                    for (int i = 0; i < model.Perex.Parameters.Length; i++)
                    {
                        var p = model.Perex.Parameters[i];
                        this.Console.WriteLine($" - {p.Title}: {p.Value}");
                    }

                    this.Console.WriteLine();
                }

                if (model.Benefits != null)
                {
                    this.Console.WriteLineSuccess("Benefits:");

                    var benefits = model.Benefits.Params.ToArray();

                    for (int i = 0; i < benefits.Length; i++)
                    {
                        var b = benefits[i];

                        this.Console.WriteLine($" - {b.Value}");
                    }

                    this.Console.WriteLine();
                }

                if (model.Gifts != null)
                {
                    this.Console.WriteLineSuccess("Gifts:");

                    foreach (var group in model.Gifts.Groups)
                    {
                        var parameters = group.Params.ToArray();

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var p = parameters[i];
                            this.Console.WriteLine($" - {p.Count} [{p.Icon}] {p.Title}");
                        }
                    }

                    this.Console.WriteLine();
                }

                if (model.Documents != null)
                {
                    if (model.Documents.Acceptance != null)
                    {
                        if (model.Documents.Acceptance.Accept != null)
                        {
                            this.Console.WriteLineSuccess("Documents for accept:");

                            foreach (var file in model.Documents.Acceptance.Accept.Files)
                            {
                                this.Console.WriteLine($" - ({file.IdAttach}) {file.Label}");
                            }

                            this.Console.WriteLine();
                            Thread.Sleep(100);
                        }


                        if (model.Documents.Acceptance.Sign != null)
                        {
                            this.Console.WriteLineSuccess("Documents for sign:");

                            foreach (var file in model.Documents.Acceptance.Sign.Files)
                            {
                                this.Console.WriteLine($" - ({file.IdAttach}) {file.Label}");
                            }

                            this.Console.WriteLine();
                            Thread.Sleep(100);
                        }
                    }

                    if (model.Documents.Uploads != null)
                    {
                        this.Console.WriteLineSuccess("Documents for upload:");

                        foreach (var file in model.Documents.Uploads.Types)
                        {
                            this.Console.WriteLine($" - ({file.IdAttach}) {file.Title}");
                        }

                        this.Console.WriteLine();
                        Thread.Sleep(100);
                    }

                    if (model.Documents.Other != null)
                    {
                        if (model.Documents.Other.AdditionalServices != null)
                        {
                            this.Console.WriteLineSuccess("Documents for additional services:");

                            foreach (var file in model.Documents.Other.AdditionalServices.Files)
                            {
                                this.Console.WriteLine($" - ({file.IdAttach}) {file.Label}");
                            }

                            this.Console.WriteLine();
                            Thread.Sleep(100);
                        }

                        if (model.Documents.Other.OtherProducts != null)
                        {
                            this.Console.WriteLineSuccess("Documents for other files:");

                            foreach (var file in model.Documents.Other.OtherProducts.Files)
                            {
                                this.Console.WriteLine($" - ({file.IdAttach}) {file.Label}");
                            }

                            this.Console.WriteLine();
                            Thread.Sleep(100);
                        }
                    }
                }
            }
        }
    }
}
