using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class GetOfferJsonCommand : BaseCommand
    {
        readonly IApiService ApiService;
        readonly IOfferJsonDescriptor JsonDescriptor;
        readonly ILogger Logger;

        public GetOfferJsonCommand(
            IApiService apiService,
            IOfferJsonDescriptor jsonDescriptor,
            ILogger logger,
            IConsole console) : base("json", console)
        {
            this.ApiService = apiService;
            this.JsonDescriptor = jsonDescriptor;
            this.Logger = logger;
        }

        [Execute]
        public async Task Execute(
            [Argument(Description = "unique identifier for an offer")] string guid,
            [Argument(Description = "what part to render")] string part = "all")
        {
            var offer = await this.ApiService.GetOfferAsync(guid);

            if (offer == null)
            {
                this.Console.WriteLineWarning("No offer");
                return;
            }

            var json = await this.JsonDescriptor.GetNewAsync(offer);

            if (part == "perex")
            {
                this.Console.WriteLine(JsonConvert.SerializeObject(json.Perex, Formatting.Indented));
            }
            else if (part == "benefits")
            {
                this.Console.WriteLine(JsonConvert.SerializeObject(json.Benefits, Formatting.Indented));
            }
            else if (part == "gifts")
            {
                this.Console.WriteLine(JsonConvert.SerializeObject(json.Gifts, Formatting.Indented));
            }
            else if (part == "documents")
            {
                this.Console.WriteLine(JsonConvert.SerializeObject(json.Documents, Formatting.Indented));
            }
            else if (part == "acceptance")
            {
                this.Console.WriteLine(JsonConvert.SerializeObject(json.AcceptanceDialog, Formatting.Indented));
            }
            else
            {
                this.Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
            }
        }
    }
}
