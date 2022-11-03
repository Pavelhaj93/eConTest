using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Models;

namespace eContracting.ConsoleClient.Commands
{
    class DownloadOfferCommand : BaseCommand
    {
        readonly IOfferService ApiService;
        readonly IOfferParserService ParserService;

        public DownloadOfferCommand(
            IOfferService apiService,
            IOfferParserService parserService,
            IConsole console) : base("download", console)
        {
            this.ApiService = apiService;
            this.ParserService = parserService;
        }

        [Execute]
        public void Execute(
            [Argument(Description = "unique identifier for an offer")] string guid)
        {
            try
            {
                var user = new UserCacheDataModel();

                if (!(this.ApiService is OfferServiceExtended))
                {
                    throw new ApplicationException($"{nameof(IOfferService)} must be implemented by {nameof(OfferServiceExtended)}");
                }

                var dir = new DirectoryInfo(Environment.CurrentDirectory);
                var rootDir = dir.Parent.Parent.Parent.Parent;
                var downloadDir = rootDir.FullName + "\\docs\\Examples\\Versions\\{version}";

                var extendService = (OfferServiceExtended)this.ApiService;
                var filesCreated = extendService.Download(guid, downloadDir, this.Console);

                this.Console.WriteLine();
                this.Console.WriteLineSuccess($"{filesCreated.Length} files created");
            }
            catch (ApplicationException ex)
            {
                this.Console.WriteLineError(ex.Message);
            }
        }
    }
}
