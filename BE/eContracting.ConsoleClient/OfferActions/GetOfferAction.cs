using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Services;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient.OfferActions
{
    class GetOfferAction : BaseOfferAction
    {
        public override string Key { get; } = "NABIDKA";
        readonly OfferParserService OfferParser;
        readonly ILogger Logger;

        public GetOfferAction(
            OfferParserService offerParser,
            ILogger logger,
            IConsole console,
            IOptions<GlobalConfiguration> options,
            ContextData contextData) : base(console, options, contextData)
        {
            this.OfferParser = offerParser;
            this.Logger = logger;
        }

        [Execute]
        public async Task Execute()
        {
            var options = new CacheApiServiceOptions(this.Options.Value.ServiceUser, this.Options.Value.ServicePassword, this.Options.Value.ServiceUrl);
            var service = new CacheApiService(options, this.OfferParser, this.Logger);
            var response = await service.GetResponse(this.Context.Guid, this.Key);
        }
    }
}
