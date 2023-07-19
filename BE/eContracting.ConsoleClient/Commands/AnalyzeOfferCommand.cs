using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Models;
using eContracting.Services;

namespace eContracting.ConsoleClient.Commands
{
    class AnalyzeOfferCommand : BaseCommand
    {
        readonly IOfferDataService OfferDataService;
        readonly OfferService ApiService;
        readonly ILogger Logger;
        readonly OfferJsonDescriptor OfferDescriptor;

        public AnalyzeOfferCommand(
            IOfferDataService offerDataService,
            IOfferService apiService,
            IOfferJsonDescriptor offerDescriptor,
            ILogger logger,
            IConsole console)
            : base("analyze", console)
        {
            this.OfferDataService = offerDataService;
            this.ApiService = apiService as OfferService;
            this.OfferDescriptor = offerDescriptor as OfferJsonDescriptor;
            this.Logger = logger;
            this.AliasKey = "a";
            this.Description = "make full analyzis of the offer";
        }

        [Execute]
        public async Task Execute(
            [Argument(Description = "unique identifier for an offer")] string guid,
            [Argument(Description = "print output as for not accepted version")] bool asNew = true,
            [Argument(Description = "add log data into output")] bool debug = true)
        {
            using (new ConsoleLoggerSuspender(this.Logger, !debug))
            {
                this.Console.WriteLine();

                var response = this.OfferDataService.GetResponse(guid, OFFER_TYPES.QUOTPRX);

                if (response.Response.EV_RETCODE != 0)
                {
                    var error = response.Response.ET_RETURN.First();

                    this.Console.WriteLine();
                    this.Console.WriteLineError(response.Response.ET_RETURN.First().MESSAGE);
                    this.Console.WriteLineWarning(" - ID: " + error.ID);
                    this.Console.WriteLineWarning(" - FIELD: " + error.FIELD);
                    this.Console.WriteLineWarning(" - LOG_MSG_NO: " + error.LOG_MSG_NO);
                    this.Console.WriteLineWarning(" - LOG_NO: " + error.LOG_NO);
                    this.Console.WriteLineWarning(" - MESSAGE_V1: " + error.MESSAGE_V1);
                    this.Console.WriteLineWarning(" - MESSAGE_V2: " + error.MESSAGE_V2);
                    this.Console.WriteLineWarning(" - MESSAGE_V3: " + error.MESSAGE_V3);
                    this.Console.WriteLineWarning(" - MESSAGE_V4: " + error.MESSAGE_V4);
                    this.Console.WriteLineWarning(" - NUMBER: " + error.NUMBER);
                    this.Console.WriteLineWarning(" - PARAMETER: " + error.PARAMETER);
                    this.Console.WriteLineWarning(" - ROW: " + error.ROW);
                    this.Console.WriteLineWarning(" - SYSTEM: " + error.SYSTEM);
                    this.Console.WriteLineWarning(" - TYPE: " + error.TYPE);
                    return;
                }

                this.Console.WriteLine("Header:");
                this.Console.WriteLine(" CCHKEY = " + response.Response.ES_HEADER.CCHKEY);
                this.Console.WriteLine(" CCHSTAT = " + response.Response.ES_HEADER.CCHSTAT);
                this.Console.WriteLine(" CCHTYPE = " + response.Response.ES_HEADER.CCHTYPE);
                this.Console.WriteLine();

                //this.Console.WriteLine("Attributes:");

                //for (int i = 0; i < response.Response.ET_ATTRIB.Length; i++)
                //{
                //    var attr = response.Response.ET_ATTRIB[i];

                //    this.Console.WriteLine($" {attr.ATTRID} = {attr.ATTRVAL}");
                //}

                //this.Console.WriteLine();

                this.Console.WriteLine("Text files");

                for (int i = 0; i < response.Response.ET_FILES.Length; i++)
                {
                    var file = response.Response.ET_FILES[i];
                    this.Console.WriteLine(" - " + file.FILENAME);
                }

                this.Console.WriteLine();
                var offer = this.ApiService.GetOffer(response, true);

                if (offer == null)
                {
                    this.Console.WriteLineError("Offer not found");
                    return;
                }

                if (offer.Documents.Length == 0)
                {
                    this.Console.WriteLineError("No attachment(s) found");
                    return;
                }

                this.Console.WriteLine();
                this.Console.WriteLine("Loading files ...");
                this.Console.WriteLine();

                var files = this.ApiService.GetFiles(offer.Guid, offer.IsAccepted);

                if (files == null)
                {
                    this.Console.WriteLine("No files found");
                    return;
                }

                //Utils.CompareIdAttach(this.Console, offer, files);

                this.Console.WriteLine();

                OfferAttachmentModel[] attachments = null;

                try
                {
                    attachments = this.ApiService.GetAttachments(offer, files);
                }
                catch (Exception ex)
                {
                    this.Console.WriteLine();
                    this.Console.WriteLineError(ex.Message);
                    this.Console.WriteLine();
                    return;
                }

                this.Console.WriteLine();

                if (!offer.IsAccepted || asNew)
                {
                    this.PrintNewOffer(offer, attachments);
                }
                else
                {
                    this.PrintAcceptedOffer(offer, attachments);
                }
            }
        }

        protected void PrintNewOffer(OfferModel offer, OfferAttachmentModel[] attachments)
        {
            var model = this.OfferDescriptor.GetNew(new OffersModel(offer), attachments); // ToDo: Replace to new JSON structure

            if (model.Perex != null)
            {
                this.Console.WriteLineSuccess("Perex:");

                for (int i = 0; i < model.Perex.Parameters.Length; i++)
                {
                    var p = model.Perex.Parameters[i];
                    this.Console.WriteLine($" - {p.Title}: {p.Value}");
                }

                this.Console.WriteLine();
            }

            if (model.SalesArguments != null)
            {
                this.Console.WriteLineSuccess("Benefits:");

                var benefits = model.SalesArguments.Arguments.ToArray();

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
                            this.Console.WriteLine($" - ({file.IdAttach}) ({file.Product}) {file.Label} (group: {file.Group}, required: {file.Mandatory})");
                        }

                        this.Console.WriteLine();
                        Thread.Sleep(100);
                    }


                    if (model.Documents.Acceptance.Sign != null)
                    {
                        this.Console.WriteLineSuccess("Documents for sign:");

                        foreach (var file in model.Documents.Acceptance.Sign.Files)
                        {
                            this.Console.WriteLine($" - ({file.IdAttach}) ({file.Product}) {file.Label} (group: {file.Group}, required: {file.Mandatory})");
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
                        this.Console.WriteLine($" - ({file.IdAttach}) ({file.Product}) {file.Title} (group: {file.GroupId}, required: {file.Mandatory})");
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
                            this.Console.WriteLine($" - ({file.IdAttach}) ({file.Product}) {file.Label} (group: {file.Group}, required: {file.Mandatory})");
                        }

                        this.Console.WriteLine();
                        Thread.Sleep(100);
                    }

                    if (model.Documents.Other.OtherProducts != null)
                    {
                        this.Console.WriteLineSuccess("Documents for other files:");

                        foreach (var file in model.Documents.Other.OtherProducts.Files)
                        {
                            this.Console.WriteLine($" - ({file.IdAttach}) ({file.Product}) {file.Label} (group: {file.Group}, required: {file.Mandatory})");
                        }

                        this.Console.WriteLine();
                        Thread.Sleep(100);
                    }
                }
            }
        }

        protected void PrintAcceptedOffer(OfferModel offer, OfferAttachmentModel[] attachments)
        {
            var model = this.OfferDescriptor.GetAccepted(new OffersModel(offer), attachments);

            this.Console.WriteLine();

            foreach (var group in model.Groups)
            {
                this.Console.WriteLineSuccess(group.Title + ":");

                foreach (var file in group.Files)
                {
                    this.Console.WriteLine(" - " + file.Label);
                }

                this.Console.WriteLine();
            }
        }
    }
}
