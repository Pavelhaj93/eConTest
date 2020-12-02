using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.SignStamp;

namespace eContracting.Services
{
    public class FileSignService : ISignService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The settings reader service.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// The API client.
        /// </summary>
        private readonly CRM_SIGN_STAMP_MERGEClient Api;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSignService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        public FileSignService(
            ILogger logger,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger;
            this.SettingsReaderService = settingsReaderService;

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var options = this.SettingsReaderService.GetSignApiServiceOptions();

            var binding = new BasicHttpBinding();
            binding.Name = nameof(CRM_SIGN_STAMP_MERGEClient);
            binding.MaxReceivedMessageSize = 65536 * 100; // this is necessary for "NABIDKA_PDF"
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            var endpoint = new EndpointAddress(options.Url);

            this.Api = new CRM_SIGN_STAMP_MERGEClient(binding, endpoint);
            this.Api.ClientCredentials.UserName.UserName = options.User;
            this.Api.ClientCredentials.UserName.Password = options.Password;
        }

        public async Task<OfferAttachmentModel> SignAsync(OfferAttachmentModel file, byte[] signature)
        {
            var inputPDFBlob = new BLOB() { binaryData = file.FileContent.ToArray(), contentType = "application/pdf", };
            var inputSignBlob = new BLOB() { binaryData = signature, contentType = "image/png" };
            
            var request = new invoke();
            request.inputPDF = inputPDFBlob;
            request.inputPNGSign = inputSignBlob;
            request.overlay = file.TemplAlcId;
            var response = await this.Api.invokeAsync(request);

            if (response.invokeResponse.errCode != 0)
            {
                this.Logger.Fatal("", "Sign the file failed: " + response.invokeResponse.errMsg);
                return null;
            }

            var newFile = file.Clone(response.invokeResponse.outputPDF.binaryData);
            return newFile;
        }
    }
}
