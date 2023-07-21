using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Models;
using eContracting.Models.JsonDescriptor;
using eContracting.Services;

namespace eContracting.ConsoleClient.Commands
{
    class AcceptOfferCommand : BaseCommand
    {
        readonly IOfferService ApiService;
        readonly ISignService SignService;
        readonly OfferJsonDescriptor JsonDescriptor;
        readonly MemoryUserFileCacheService UserFileCache;

        public AcceptOfferCommand(
            IOfferService apiService,
            ISignService signService,
            IOfferJsonDescriptor jsonDescriptor,
            IUserFileCacheService userFileCache,
            IConsole console) : base("accept", console)
        {
            this.ApiService = apiService;
            this.SignService = signService;
            this.JsonDescriptor = jsonDescriptor as OfferJsonDescriptor;
            this.UserFileCache = userFileCache as MemoryUserFileCacheService;
            this.Description = "accept offer (only simple version with not sign and no upload)";
        }

        [Execute]
        public void Execute(
            [Argument(Description = "unique identifier for an offer")] string guid)
        {
            var user = new UserCacheDataModel();
            var offer = this.ApiService.GetOffer(guid, user);

            if (offer == null)
            {
                this.Console.WriteLineWarning("No offer");
                return;
            }

            //var signature = "data:image/png;base64,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";
            var signature = "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";
            var signatureBytes = Encoding.UTF8.GetBytes(signature);
            var attachments = this.ApiService.GetAttachments(offer, user);

            var attachmentsForSign = attachments.Where(x => x.IsSignReq);

            if (attachmentsForSign.Any())
            {
                foreach (var attachment in attachmentsForSign)
                {
                    var signedAttachment = this.SignService.Sign(offer.First(), attachment, signatureBytes);
                    this.UserFileCache.SignedFiles.Add(new DbSignedFileModel() { Key = attachment.UniqueKey, File = new DbFileModel() { Content = signedAttachment.FileContent } });
                }
            }

            var json = this.JsonDescriptor.GetNew2(offer, attachments);

            var data = new OfferSubmitDataModel();

            var signFiles = new List<string>();

            var signedDataModels = json.Data.Where(x => x.Type == Constants.JsonDocumentDataModelType.DOCS_SIGN);
            if (signedDataModels.Any())
            {
                foreach(var signedDataModel in signedDataModels)
                {
                    var bodyModel = (DocumentDataBodyModel)((DocumentDataModel)signedDataModels).Body;
                    if (bodyModel.Docs.Files.Any())
                    {
                        foreach (var file in bodyModel.Docs.Files)
                        {
                            signFiles.Add(file.Key);
                        }
                    }
                }
            }

            if (signFiles.Count > 0)
            {
                data.Signed = signFiles;
                this.Console.WriteLine(signFiles.Count + " sign files");
            }

            var acceptedFiles = new List<string>();
            var acceptedDataModels = json.Data.Where(
                x => x.Type == Constants.JsonDocumentDataModelType.DOCS_CHECK_E 
                || x.Type == Constants.JsonDocumentDataModelType.DOCS_CHECK_G 
                || x.Type == Constants.JsonDocumentDataModelType.DOCS_CHECK_E_G);
            if (acceptedDataModels.Any())
            {
                foreach( var acceptedDataModel in acceptedDataModels)
                {
                    var bodyModel = (DocumentDataBodyModel)((DocumentDataModel)acceptedDataModel).Body;
                    if (bodyModel.Docs.Files.Any())
                    {
                        foreach (var file in bodyModel.Docs.Files)
                        {
                            acceptedFiles.Add(file.Key);
                        }
                    }
                }
            }            

            if (acceptedFiles.Count > 0)
            {
                data.Accepted = acceptedFiles;                
                this.Console.WriteLine(acceptedFiles.Count + " checked files (accepted files + other files)"); // accepted files + doplnkove sluzby + ostatni sluzby
            }            

            this.ApiService.AcceptOffer(offer.First(), data, user, "FB057B3ED3E24872AF79CE1FD77BCA46");
        }
    }
}
