using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.Models;
using eContracting.Services.SAP;

namespace eContracting.Services
{
    /// <summary>
    /// Service wrapper over generated <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public class CacheApiService : IApiService
    {
        public static string[] AvailableRequestTypes = new[] { "NABIDKA", "NABIDKA_XML", "NABIDKA_PDF", "NABIDKA_ARCH" };
        public readonly CacheApiServiceOptions Options;
        protected readonly OfferParserService OfferParser;
        protected readonly OfferAttachmentParserService AttachmentParser;
        protected readonly ILogger Logger;
        private readonly ZCCH_CACHE_APIClient Api;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheApiService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="offerParser">The offer parser.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">
        /// offerParser
        /// or
        /// logger
        /// </exception>
        public CacheApiService(CacheApiServiceOptions options, OfferParserService offerParser, OfferAttachmentParserService attachmentParser, ILogger logger)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            this.Options = options;

            var binding = new BasicHttpBinding();
            binding.Name = nameof(ZCCH_CACHE_APIClient);
            binding.MaxReceivedMessageSize = 65536 * 100; // this is necessary for "NABIDKA_PDF"
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            var endpoint = new EndpointAddress(this.Options.Url);

            this.Api = new ZCCH_CACHE_APIClient(binding, endpoint);
            this.Api.ClientCredentials.UserName.UserName = this.Options.User;
            this.Api.ClientCredentials.UserName.Password = this.Options.Password;

            this.OfferParser = offerParser ?? throw new ArgumentNullException(nameof(offerParser));
            this.AttachmentParser = attachmentParser ?? throw new ArgumentNullException(nameof(attachmentParser));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<OfferModel> GetOfferAsync(string guid, string type)
        {
            ResponseCacheGetModel response = await this.GetResponseAsync(guid, type);

            if (response == null)
            {
                return null;
            }

            return this.OfferParser.GenerateOffer(response);
        }

        /// <inheritdoc/>
        public async Task<OfferTextModel[]> GetXmlAsync(string guid)
        {
            var response = await this.GetResponseAsync(guid, "NABIDKA_XML");

            var list = new List<OfferTextModel>();

            if (response.HasFiles)
            {
                for (int i = 0; i < response.Response.ET_FILES.Length; i++)
                {
                    var file = response.Response.ET_FILES[i];
                    this.Logger.Debug($"[{guid}] NABIDKA_XML response: Contains ST_FILE - {file.FILENAME}");
                    var index = i.ToString();
                    var name = file.FILENAME;
                    var text = Encoding.UTF8.GetString(file.FILECONTENT);

                    var item = new OfferTextModel(index: index, name: name, text: text);

                    if (file.ATTRIB != null)
                    {
                        foreach (var attr in file.ATTRIB)
                        {
                            item.Attributes.Add(attr.ATTRID, attr.ATTRVAL);
                        }
                    }

                    list.Add(item);
                }
            }

            return list.ToArray();
        }

        /// <inheritdoc/>
        public async Task<OfferAttachmentXmlModel[]> GetAttachmentsAsync(string guid)
        {
            var result = await this.GetResponseAsync(guid, "NABIDKA");

            if (result == null)
            {
                return null;
            }

            var offer = this.OfferParser.GenerateOffer(result);

            bool isAccepted = offer.IsAccepted;

            ZCCH_ST_FILE[] files = await this.GetFilesAsync(guid, isAccepted);

            var fileResults = new List<OfferAttachmentXmlModel>();

            if (files.Length != 0 && files.All(x => x.ATTRIB.Any(y => y.ATTRID == "COUNTER")))
            {
                int index = 0;

                foreach (ZCCH_ST_FILE f in files.OrderBy(x => int.Parse(x.ATTRIB.FirstOrDefault(y => y.ATTRID == "COUNTER").ATTRVAL)))
                {
                    try
                    {
                        var fileModel = this.AttachmentParser.Parse(f, index, offer);
                        fileResults.Add(fileModel);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error($"[{guid}] Exception occured when parsing file list", ex);
                    }
                }

                this.Logger.LogFiles(fileResults, guid, isAccepted);
                return fileResults.ToArray();
            }

            this.Logger.LogFiles(null, guid, isAccepted);
            return null;
        }

        /// <inheritdoc/>
        public async Task<bool> AcceptOfferAsync(string guid)
        {
            return await this.SetStatusAsync(guid, "NABIDKA", "5");
        }

        /// <inheritdoc/>
        public async Task<bool> ReadOfferAsync(string guid)
        {
            return await this.SetStatusAsync(guid, "NABIDKA", "4");
        }

        /// <summary>
        /// Gets data.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Type from <see cref="AvailableRequestTypes"/> collection.</param>
        /// <param name="fileType">Type of the file.</param>
        /// <returns>Instance of <see cref="ResponseCacheGetModel"/> or an exception.</returns>
        protected internal async Task<ResponseCacheGetModel> GetResponseAsync(string guid, string type, string fileType = "B")
        {
            var model = new ZCCH_CACHE_GET();
            model.IV_CCHKEY = guid;
            model.IV_CCHTYPE = type;
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

        protected internal async Task<bool> SetStatusAsync(string guid, string type, string status)
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
                this.Logger.Fatal($"Cannot parse timestamp to decimal ({timestampString})");
            }

            return false;
        }

        /// <summary>
        /// Sets the status with <see cref="ZCCH_CACHE_STATUS_SET"/> object.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">A type from <see cref="AvailableRequestTypes"/> collection.</param>
        /// <param name="timestamp">Decimal representation of a timestamp.</param>
        /// <param name="status">Value for <see cref="ZCCH_CACHE_STATUS_SET.IV_STAT"/>.</param>
        /// <returns>True if it was successfully set or false.</returns>
        protected internal async Task<bool> SetStatusAsync(string guid, string type, decimal timestamp, string status)
        {
            var model = new ZCCH_CACHE_STATUS_SET();
            model.IV_CCHKEY = guid;
            model.IV_CCHTYPE = type;
            model.IV_STAT = status;
            model.IV_TIMESTAMP = timestamp;

            var request = new ZCCH_CACHE_STATUS_SETRequest(model);
            var stop = new Stopwatch();
            stop.Start();
            var response = await this.Api.ZCCH_CACHE_STATUS_SETAsync(request);
            stop.Stop();
            this.Logger.TimeSpent(model, stop.Elapsed);

            this.Logger.Debug("");

            if (response != null)
            {
                if (response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE == 0)
                {
                    return true;
                }

                this.Logger.Error($"[{guid}] Call to the web service during Accepting returned result {response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE}.");
            }
            else
            {
                this.Logger.Fatal($"[{guid}] Call to the web service during Accepting returned null result.");
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
                var result = await this.GetResponseAsync(guid, "NABIDKA_ARCH");
                files.AddRange(result.Response.ET_FILES);
                var filenames = result.Response.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.FILENAME));
                files.AddRange(result.Response.ET_FILES);
                this.Logger.LogFiles(files, guid, isAccepted, "NABIDKA_ARCH");
            }
            else
            {
                var result = await this.GetResponseAsync(guid, "NABIDKA_PDF");

                if (result.Response?.ET_FILES?.Any() ?? false)
                {
                    files.AddRange(result.Response.ET_FILES);
                }

                this.Logger.LogFiles(files, guid, isAccepted, "NABIDKA_PDF");
            }

            return files.ToArray();
        }
    }
}
