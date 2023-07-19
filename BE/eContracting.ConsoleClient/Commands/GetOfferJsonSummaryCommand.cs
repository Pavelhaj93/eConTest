using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class GetOfferJsonCommand : BaseCommand
    {
        readonly IOfferService ApiService;
        readonly IOfferJsonDescriptor JsonDescriptor;
        readonly ILogger Logger;

        public GetOfferJsonCommand(
            IOfferService apiService,
            IOfferJsonDescriptor jsonDescriptor,
            ILogger logger,
            IConsole console) : base("json", console)
        {
            this.ApiService = apiService;
            this.JsonDescriptor = jsonDescriptor;
            this.Logger = logger;
        }

        [Execute]
        public void Execute(
            [Argument(Description = "unique identifier for an offer")] string guid,
            [Argument(Description = "what part to render")] string part = "all")
        {
            var user = new UserCacheDataModel();
            var offer = this.ApiService.GetOffer(guid, user);

            if (offer == null)
            {
                this.Console.WriteLineWarning("No offer");
                return;
            }

            //var json = this.JsonDescriptor.GetNew(offer, user);
            var json = this.JsonDescriptor.GetNew2(offer, user);

            if (part == "perex")
            {
                var perex = json.Data.Where(x => x.Type == "perex");
                this.Console.WriteLine(JsonConvert.SerializeObject(perex, Formatting.Indented));
            }
            else if (part == "benefits")
            {
                var salesArguments = json.Data.Where(x => x.Type == "benefit");
                this.Console.WriteLine(JsonConvert.SerializeObject(salesArguments, Formatting.Indented));
            }
            // ToDo: Finalize GetOfferJsonCommand
            /*else if (part == "gifts")
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
            }*/
        }
    }
}
