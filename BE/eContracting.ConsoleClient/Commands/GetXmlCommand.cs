using System.Linq;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Services;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient.Commands
{
    class GetXmlCommand : BaseCommand
    {
        public override string Key { get; protected set; } = "xml";
        readonly ContextData Context;
        readonly OfferParserService OfferParser;
        readonly OfferAttachmentParserService AttachmentParser;
        readonly IOptions<GlobalConfiguration> Options;
        readonly ILogger Logger;

        public GetXmlCommand(OfferParserService offerParser,
            OfferAttachmentParserService attachmentParser,
            ILogger logger,
            IOptions<GlobalConfiguration> options,
            IConsole console,
            ContextData contextData) : base(Enumerable.Empty<IAction>(), console)
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
            var service = new CacheApiService(options, this.OfferParser, this.AttachmentParser, this.Logger);
            var xmls = await service.GetXmlAsync(guid);

            foreach (var xml in xmls)
            {
                this.Console.WriteLine($"Name: {xml.Name}");
            }
        }
    }
}
