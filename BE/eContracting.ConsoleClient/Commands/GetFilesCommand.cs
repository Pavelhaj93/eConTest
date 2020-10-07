using System.Linq;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Services;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient.Commands
{
    class GetFilesCommand : BaseCommand
    {
        readonly ContextData Context;
        readonly OfferParserService OfferParser;
        readonly OfferAttachmentParserService AttachmentParser;
        readonly IOptions<GlobalConfiguration> Options;
        readonly ILogger Logger;

        public GetFilesCommand(OfferParserService offerParser,
            OfferAttachmentParserService attachmentParser,
            ILogger logger,
            IOptions<GlobalConfiguration> options,
            IConsole console,
            ContextData contextData) : base("files", console)
        {
            this.OfferParser = offerParser;
            this.AttachmentParser = attachmentParser;
            this.Logger = logger;
            this.Options = options;
            this.Context = contextData;
        }

        [Execute]
        public async Task Execute([Argument(Description = "unique identifier for an offer")] string guid)
        {
            var options = new CacheApiServiceOptions(this.Options.Value.ServiceUser, this.Options.Value.ServicePassword, this.Options.Value.ServiceUrl);
            this.Logger.Suspend(true);
            var service = new CacheApiService(options, this.OfferParser, this.AttachmentParser, this.Logger);
            var files = await service.GetAttachmentsAsync(guid);
            this.Logger.Suspend(false);

            var table = new LogTable();
            table.SetHeaders(
                "Type",
                "File",
                "Size",
                Constants.FileAttributes.GROUP_OBLIG,
                Constants.FileAttributes.OBLIGATORY,
                Constants.FileAttributes.PRINTED,
                Constants.FileAttributes.SIGN_REQ,
                Constants.FileAttributes.TMST_REQ,
                Constants.FileAttributes.ADDINFO,
                Constants.FileAttributes.MAIN_DOC);

            foreach (var file in files)
            {
                table.SetValues(
                    file.Template,
                    file.FileName,
                    file.SizeLabel,
                    file.HasAttribute(Constants.FileAttributes.GROUP_OBLIG) ? "x" : "",
                    file.HasAttribute(Constants.FileAttributes.OBLIGATORY) ? "x" : "",
                    file.HasAttribute(Constants.FileAttributes.PRINTED) ? "x" : "",
                    file.HasAttribute(Constants.FileAttributes.SIGN_REQ) ? "x" : "",
                    file.HasAttribute(Constants.FileAttributes.TMST_REQ) ? "x" : "",
                    file.HasAttribute(Constants.FileAttributes.ADDINFO) ? "x" : "",
                    file.HasAttribute(Constants.FileAttributes.MAIN_DOC) ? "x" : "");
            }

            this.Console.Write(table.ToString());
        }
    }
}
