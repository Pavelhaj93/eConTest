﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class CompareIdAttach : BaseCommand
    {
        readonly ContextData Context;
        readonly SapApiService ApiService;
        readonly ILogger Logger;

        public CompareIdAttach(
            IApiService apiService,
            ILogger logger,
            IConsole console,
            ContextData contextData) : base("idattach", console)
        {
            this.ApiService = apiService as SapApiService;
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
                var offer = await this.ApiService.GetOfferAsync(guid);

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

                var files = await this.ApiService.GetFilesAsync(offer.Guid, false);

                if (files == null)
                {
                    this.Console.WriteLine("No files found");
                    return;
                }

                for (int i = 0; i < offer.Documents.Length; i++)
                {
                    var attachment = offer.Documents[i];
                    bool fileFound = false;

                    for (int y = 0; y < files.Length; y++)
                    {
                        var file = files[y];
                        var fileIdAttach = file.GetIdAttach();

                        if (fileIdAttach == attachment.IdAttach)
                        {
                            fileFound = true;
                        }
                    }

                    if (fileFound)
                    {
                        this.Console.WriteLineSuccess($"Attachment {attachment.IdAttach} found");
                    }
                    else
                    {
                        this.Console.WriteLineError($"Attachment {attachment.IdAttach} ({attachment.Description}) not found in files");
                    }
                }

                for (int i = 0; i < files.Length; i++)
                {
                    var file = files[i];

                    if (!offer.Documents.Any(x => x.IdAttach == file.GetIdAttach()))
                    {
                        this.Console.WriteLineError($"File {file.FILENAME} doesn't exist in attachments");
                    }
                }
            }
        }
    }
}
