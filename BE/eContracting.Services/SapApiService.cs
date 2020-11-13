using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    /// <summary>
    /// Service wrapper over generated <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public class SapApiService : IApiService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The cache.
        /// </summary>
        protected readonly ICache Cache;

        /// <summary>
        /// The settings reader service.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// The offer parser.
        /// </summary>
        protected readonly IOfferParserService OfferParser;

        /// <summary>
        /// The attachment parser.
        /// </summary>
        protected readonly IOfferAttachmentParserService AttachmentParser;

        /// <summary>
        /// The API client.
        /// </summary>
        private readonly ZCCH_CACHE_APIClient Api;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapApiService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <param name="offerParser">The offer parser.</param>
        /// <param name="offerAttachmentParser">The offer attachment parser.</param>
        public SapApiService(
            ILogger logger,
            ICache cache,
            ISettingsReaderService settingsReaderService,
            IOfferParserService offerParser,
            IOfferAttachmentParserService offerAttachmentParser)
        {
            this.Logger = logger;
            this.Cache = cache;
            this.SettingsReaderService = settingsReaderService;
            this.OfferParser = offerParser;
            this.AttachmentParser = offerAttachmentParser;
            
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var options = this.SettingsReaderService.GetApiServiceOptions();

            var binding = new BasicHttpBinding();
            binding.Name = nameof(ZCCH_CACHE_APIClient);
            binding.MaxReceivedMessageSize = 65536 * 100; // this is necessary for "NABIDKA_PDF"
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            var endpoint = new EndpointAddress(options.Url);
            
            this.Api = new ZCCH_CACHE_APIClient(binding, endpoint);
            this.Api.ClientCredentials.UserName.UserName = options.User;
            this.Api.ClientCredentials.UserName.Password = options.Password;
        }

        /// <inheritdoc/>
        public bool AcceptOffer(string guid)
        {
            var task = Task.Run(() => this.AcceptOfferAsync(guid));
            task.Wait();
            var result = task.Result;
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> AcceptOfferAsync(string guid)
        {
            return await this.SetStatusAsync(guid, OFFER_TYPES.NABIDKA, "5");
        }

        /// <inheritdoc/>
        public OfferModel GetOffer(string guid, OFFER_TYPES type)
        {
            var task = Task.Run(() => this.GetOfferAsync(guid, type));
            task.Wait();
            var result = task.Result;
            return result;
        }

        /// <inheritdoc/>
        public async Task<OfferModel> GetOfferAsync(string guid, OFFER_TYPES type)
        {
            var response = await this.GetResponseAsync(guid, type);

            if (response == null)
            {
                return null;
            }

            var offer = this.OfferParser.GenerateOffer(response);

            if (offer == null)
            {
                return null;
            }

            var textParameters = await this.GetTextParametersAsync(response);
            offer.TextParameters.Merge(textParameters);
            return offer;
        }

        protected async Task<IDictionary<string, string>> GetTextParametersAsync(ResponseCacheGetModel response)
        {
            var version = this.OfferParser.GetVersion(response.Response);

            if (version == 1)
            {
                var response2 = await this.GetResponseAsync(response.Guid, OFFER_TYPES.NABIDKA_XML);

                if (response2 != null)
                {
                    var response2Files = response2.Response.ET_FILES;
                    return this.OfferParser.GetTextParameters(response2Files);
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
            else if (version == 2)
            {
                var files = response.Response.ET_FILES.Where(x => x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE && a.ATTRVAL == "AD1")).ToArray();
                return this.OfferParser.GetTextParameters(files);
            }
            else
            {
                throw new ApplicationException($"Cannot get text parameters. Unknown version {version}");
            }
        }

        /// <inheritdoc/>
        public OfferAttachmentXmlModel[] GetAttachments(string guid)
        {
            var task = Task.Run(() => this.GetAttachmentsAsync(guid));
            task.Wait();
            var result = task.Result;
            return result;
        }

        /// <inheritdoc/>
        public async Task<OfferAttachmentXmlModel[]> GetAttachmentsAsync(string guid)
        {
            var result = await this.GetResponseAsync(guid, OFFER_TYPES.NABIDKA);

            if (result == null)
            {
                return null;
            }

            var offer = this.OfferParser.GenerateOffer(result);
            bool isAccepted = offer.IsAccepted;

            ZCCH_ST_FILE[] stFiles = await this.GetFilesAsync(guid, isAccepted);

            if (stFiles.Length == 0)
            {
                this.Logger.LogFiles(null, guid, isAccepted);
                return null;
            }

            var orderedFiles = stFiles.OrderBy(x => x.GetCounter()).ToArray();
            var fileModels = new List<OfferAttachmentXmlModel>();

            for (int index = 0; index < orderedFiles.Length; index++)
            {
                var file = orderedFiles[index];

                try
                {
                    var fileModel = this.AttachmentParser.Parse(file, index, offer);
                    fileModels.Add(fileModel);
                }
                catch (Exception ex)
                {
                    this.Logger.Error(guid, $"Exception occured when parsing file '{file.FILENAME}'", ex);
                    throw;
                }
            }

            this.Logger.LogFiles(fileModels, guid, isAccepted);
            return fileModels.ToArray();
        }

        /// <inheritdoc/>
        public bool ReadOffer(string guid)
        {
            var task = Task.Run(() => this.ReadOfferAsync(guid));
            task.Wait();
            var result = task.Result;
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> ReadOfferAsync(string guid)
        {
            return await this.SetStatusAsync(guid, OFFER_TYPES.NABIDKA, "4");
        }

        /// <inheritdoc/>
        public bool SignInOffer(string guid)
        {
            var task = Task.Run(() => this.SignInOfferAsync(guid));
            task.Wait();
            var result = task.Result;
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> SignInOfferAsync(string guid)
        {
            return await this.SetStatusAsync(guid, OFFER_TYPES.NABIDKA, "6");
        }

        /// <summary>
        /// Gets data.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <param name="fileType">Type of the file.</param>
        /// <returns>Instance of <see cref="ResponseCacheGetModel"/> or an exception.</returns>
        protected internal async Task<ResponseCacheGetModel> GetResponseAsync(string guid, OFFER_TYPES type, string fileType = "B")
        {
            var model = new ZCCH_CACHE_GET();
            model.IV_CCHKEY = guid;
            model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
            model.IV_GEFILE = fileType;

            var request = new ZCCH_CACHE_GETRequest(model);
            var stop = new Stopwatch();
            stop.Start();
            var response = await this.Api.ZCCH_CACHE_GETAsync(request);
            stop.Stop();
            this.Logger.TimeSpent(model, stop.Elapsed);
            var result = response.ZCCH_CACHE_GETResponse;
            return new ResponseCacheGetModel(result);
        }

        /// <summary>
        /// Sets the <paramref name="status"/> asynchronously.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        protected internal async Task<bool> SetStatusAsync(string guid, OFFER_TYPES type, string status)
        {
            var timestampString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            decimal outValue = 1M;

            if (decimal.TryParse(timestampString, out outValue))
            {
                bool result = await this.SetStatusAsync(guid, type, outValue, status);
                return result;
            }
            else
            {
                this.Logger.Fatal(guid, $"Cannot parse timestamp to decimal ({timestampString})");
            }

            return false;
        }

        /// <summary>
        /// Sets the <paramref name="status"/> asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">A type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <param name="timestamp">Decimal representation of a timestamp.</param>
        /// <param name="status">Value for <see cref="ZCCH_CACHE_STATUS_SET.IV_STAT"/>.</param>
        /// <returns>True if it was successfully set or false.</returns>
        protected internal async Task<bool> SetStatusAsync(string guid, OFFER_TYPES type, decimal timestamp, string status)
        {
            var model = new ZCCH_CACHE_STATUS_SET();
            model.IV_CCHKEY = guid;
            model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
            model.IV_STAT = status;
            model.IV_TIMESTAMP = timestamp;

            var request = new ZCCH_CACHE_STATUS_SETRequest(model);
            var stop = new Stopwatch();
            stop.Start();
            var response = await this.Api.ZCCH_CACHE_STATUS_SETAsync(request);
            stop.Stop();
            this.Logger.TimeSpent(model, stop.Elapsed);

            this.Logger.Debug(guid, $"Status changed to {status}");

            if (response != null)
            {
                if (response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE == 0)
                {
                    return true;
                }

                this.Logger.Error(guid, $"Call to the web service during Accepting returned result {response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE}.");
            }
            else
            {
                this.Logger.Fatal(guid, $"Call to the web service during Accepting returned null result.");
            }

            return false;
        }

        /// <summary>
        /// Gets the files asynchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="isAccepted">if set to <c>true</c> [is accepted].</param>
        /// <returns></returns>
        protected internal async Task<ZCCH_ST_FILE[]> GetFilesAsync(string guid, bool isAccepted)
        {
            List<ZCCH_ST_FILE> files = new List<ZCCH_ST_FILE>();

            if (isAccepted)
            {
                var result = await this.GetResponseAsync(guid, OFFER_TYPES.NABIDKA_ARCH);
                files.AddRange(result.Response.ET_FILES);
                var filenames = result.Response.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.FILENAME));
                files.AddRange(result.Response.ET_FILES);
                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.NABIDKA_ARCH);
            }
            else
            {
                var result = await this.GetResponseAsync(guid, OFFER_TYPES.NABIDKA_PDF);

                if (result.Response?.ET_FILES?.Any() ?? false)
                {
                    files.AddRange(result.Response.ET_FILES);
                }

                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.NABIDKA_PDF);
            }

            return files.ToArray();
        }
    }
}
