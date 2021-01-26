using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /// <inheritdoc/>
        public OfferAttachmentModel Sign(OfferModel offer, OfferAttachmentModel file, byte[] signature)
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

                this.Logger.Info(null, $"Connecting to sign service on '{api.Endpoint.ListenUri.ToString()}' ...");

                var log = new StringBuilder();
                log.AppendLine($"Calling sign service for file '{file.OriginalFileName}'");
                this.Logger.Info(offer.Guid, log.ToString());

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to sign service finished:");

                try
                {
                    var response = api.invoke(invoke);
                    //var task = api.invokeAsync(request);
                    //task.Wait();
                    //var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.errCode);
                    this.Logger.Info(offer.Guid, log2.ToString());

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
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(offer.Guid, log2.ToString(), ex);
                    throw ex;
                }
            }
        }
    }
}
