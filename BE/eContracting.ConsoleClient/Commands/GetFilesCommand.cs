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
    class GetFilesCommand : BaseCommand
    {
        readonly ContextData Context;
        readonly IApiService ApiService;
        readonly ILogger Logger;

        public GetFilesCommand(
            IApiService apiService,
            ILogger logger,
            IConsole console,
            ContextData contextData) : base("files", console)
        {
            this.ApiService = apiService;
            this.Logger = logger;
            this.Context = contextData;
        }

        [Execute]
        public async Task Execute([Argument(Description = "unique identifier for an offer")] string guid)
        {
            var offer = await this.ApiService.GetOfferAsync(guid);

            if (offer == null)
            {
                this.Console.WriteLineError("Offer not found");
            }

            this.Console.WriteLine("Offer attachments:");
            var list1 = new List<FileModel>();

            for (int i = 0; i < offer.Attachments.Length; i++)
            {
                list1.Add(new FileModel(offer.Attachments[i].IdAttach, offer.Attachments[i].Printed, offer.Attachments[i].Description));
            }

            this.Console.WriteLine(JsonConvert.SerializeObject(list1, Formatting.Indented));

            var files = await this.ApiService.GetAttachmentsAsync(offer);

            if (files == null)
            {
                this.Console.WriteLine("No files");
            }
            else
            {
                this.Console.WriteLine("Offer files:");
                var list2 = new List<FileModel>();

                for (int i = 0; i < files.Length; i++)
                {
                    list2.Add(new FileModel(files[i].Attributes?.FirstOrDefault(x => x.Key == "IDATTACH")?.Value, files[i].IsPrinted ? "X" : "", files[i].FileName));
                }

                this.Console.WriteLine(JsonConvert.SerializeObject(list2, Formatting.Indented));
                //this.Console.WriteLine(JsonConvert.SerializeObject(files, Formatting.Indented));
            }

            //var table = new LogTable();
            //table.SetHeaders(
            //    "Type",
            //    "File",
            //    "Size",
            //    Constants.FileAttributes.GROUP_OBLIG,
            //    Constants.FileAttributes.OBLIGATORY,
            //    Constants.FileAttributes.PRINTED,
            //    Constants.FileAttributes.SIGN_REQ,
            //    Constants.FileAttributes.TMST_REQ,
            //    Constants.FileAttributes.ADDINFO,
            //    Constants.FileAttributes.MAIN_DOC);

            //foreach (var file in files)
            //{
            //    table.SetValues(
            //        file.Template,
            //        file.FileName,
            //        file.SizeLabel,
            //        file.IsGroupOblig.ToString(),
            //        file.IsObligatory.ToString(),
            //        file.IsPrinted.ToString(),
            //        file.IsSignReq.ToString(),
            //        file.IsTmstReq.ToString(),
            //        file.IsAddinfo.ToString(),
            //        file.IsMainDoc.ToString());
            //}

            //this.Console.Write(table.ToString());
        }
    }
}
