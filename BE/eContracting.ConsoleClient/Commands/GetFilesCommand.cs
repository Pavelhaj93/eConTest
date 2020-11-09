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
            this.Logger.Suspend(true);
            var files = await this.ApiService.GetAttachmentsAsync(guid);
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
                    file.IsGroupOblig.ToString(),
                    file.IsObligatory.ToString(),
                    file.IsPrinted.ToString(),
                    file.IsSignReq.ToString(),
                    file.IsTmstReq.ToString(),
                    file.IsAddinfo.ToString(),
                    file.IsMainDoc.ToString());
            }

            this.Console.Write(table.ToString());
        }
    }
}
