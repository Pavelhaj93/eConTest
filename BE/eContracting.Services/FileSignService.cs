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
        /// The settings service.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// The service factory.
        /// </summary>
        protected readonly IServiceFactory ServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSignService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <param name="factory">The factory.</param>
        public FileSignService(
            ILogger logger,
            ISettingsReaderService settingsReaderService,
            IServiceFactory factory)
        {
            this.Logger = logger;
            this.ServiceFactory = factory;
            this.SettingsReaderService = settingsReaderService;
        }

        public OfferAttachmentModel Sign(OfferAttachmentModel file, byte[] signature)
        {
            var options = this.SettingsReaderService.GetSignApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var inputPDFBlob = new BLOB() { binaryData = file.FileContent.ToArray(), contentType = "application/pdf", };
                var inputSignBlob = new BLOB() { binaryData = signature, contentType = "image/png" };

                var invoke = new invoke();
                invoke.inputPDF = inputPDFBlob;
                invoke.inputPNGSign = inputSignBlob;
                invoke.overlay = file.TemplAlcId;

                this.Logger.Debug(null, $"Connecting to signig service on '{api.Endpoint.ListenUri.ToString()}' ...");

                try
                {
                    var response = api.invoke(invoke);
                    //var task = api.invokeAsync(request);
                    //task.Wait();
                    //var response = task.Result;

                    this.Logger.Debug(null, $"Response received: {response.errCode}");

                    if (response.errCode != 0)
                    {
                        throw new EcontractingSignException(ERROR_CODES.FileNotSigned(response.errMsg));
                    }

                    if ((response.outputPDF?.binaryData?.Length ?? 0) == 0)
                    {
                        throw new EcontractingSignException(ERROR_CODES.EmptySignedFile());
                    }

                    var newFile = file.Clone(response.outputPDF.binaryData);
                    return newFile;
                }
                catch (Exception ex)
                {
                    this.Logger.Fatal(null, "Connection failed", ex);
                    throw;
                }
            }
        }
    }
}
