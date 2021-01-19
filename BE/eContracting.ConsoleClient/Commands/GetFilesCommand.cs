using System.Collections.Generic;
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
        readonly IOfferService ApiService;
        readonly ILogger Logger;

        public GetFilesCommand(
            IOfferService apiService,
            ILogger logger,
            IConsole console,
            ContextData contextData) : base("files", console)
        {
            this.ApiService = apiService;
            this.Logger = logger;
            this.Context = contextData;
            this.AliasKey = "f";
            this.Description = "gets offer files";
        }

        [Execute]
        public async Task Execute([Argument(Description = "unique identifier for an offer")] string guid)
        {
            var offer = this.ApiService.GetOffer(guid);

            if (offer == null)
            {
                this.Console.WriteLineError("Offer not found");
                return;
            }

            this.Console.WriteLine("Offer attachments:");
            var list1 = new List<FileModel>();

            for (int i = 0; i < offer.Documents.Length; i++)
            {
                list1.Add(new FileModel(offer.Documents[i].IdAttach, offer.Documents[i].Printed, offer.Documents[i].Description));
            }

            this.Console.WriteLine(JsonConvert.SerializeObject(list1, Formatting.Indented));

            var files = this.ApiService.GetAttachments(offer);

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
