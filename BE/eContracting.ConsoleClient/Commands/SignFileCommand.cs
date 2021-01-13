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
    class SignFileCommand : BaseCommand
    {
        readonly IServiceFactory Factory;
        readonly ISignService SignService;

        public SignFileCommand(
            IServiceFactory serviceFactory,
            ISignService signService,
            IConsole console) : base("sign", console)
        {
            this.Factory = serviceFactory;
            this.SignService = signService;
        }

        [Execute]
        public void Execute(
            [Argument(Description = "file to be signed")]string file,
            [Argument(Description = "file with signature")]string signature)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException($"File '{file}' doesn't exist");
            }

            if (!File.Exists(signature))
            {
                throw new ArgumentException($"File '{signature}' doesn't exist");
            }

            var signFileContent = File.ReadAllBytes(file);
            var signatureFileContent = File.ReadAllBytes(signature);
            //var attachment = new OfferAttachmentModel();
            var url = "https://wd-wcc.rwe-services.cz:8110/sap/bc/srt/xip/sap/zcrm_sign_stamp_merge/100/crm_sign_stamp_merge/crm_sign_stamp_merge";
            var options = new SignApiServiceOptions(url, "UkZDX1NJVEVDT1JF", "QWRIYjI0Nyo=");
            //this.SignService.Sign()
        }
    }
}
