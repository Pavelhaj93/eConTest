using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;

namespace eContracting.ConsoleClient.Commands
{
    class GetLoginValuesCommand : BaseCommand
    {
        readonly IOfferService ApiService;
        readonly ILogger Logger;

        public GetLoginValuesCommand(IOfferService apiService, IConsole console, ILogger logger) : base("login", console)
        {
            this.ApiService = apiService;
            this.AliasKey = "l";
            this.Logger = logger;
            this.Description = "print values for login on the website";
        }

        [Execute]
        public void Execute(
            [Argument(Description = "unique identifier for an offer")] string guid)
        {

            using (new ConsoleLoggerSuspender(this.Logger))
            {
                var offer = this.ApiService.GetOffer(guid, false);

                if (offer == null)
                {
                    this.Console.WriteLineWarning("No offer");
                    return;
                }

                this.Console.WriteLine();
                this.Console.Write("Datum:   ");
                this.Console.WriteSuccess(offer.Birthday);
                this.Console.WriteLine();
                this.Console.Write("Zák. č.: ");
                this.Console.WriteSuccess(offer.PartnerNumber);
                this.Console.WriteLine();
                this.Console.Write("PSČ:     ");
                this.Console.WriteSuccess(offer.PostNumber);
                this.Console.WriteLine();
                this.Console.Write("PSČ MS:  ");
                this.Console.WriteSuccess(offer.PostNumberConsumption);
                this.Console.WriteLine();
                this.Console.WriteLine();
            }
        }
    }
}
