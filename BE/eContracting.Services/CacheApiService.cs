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
    /// Wrapper over <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public class CacheApiService
    {
        public readonly CacheApiServiceOptions Options;

        private readonly ZCCH_CACHE_APIClient Api;
        protected readonly OfferParserService OfferParser;
        protected readonly ILogger Logger;
        public static string[] AvailableRequestTypes = new[] { "NABIDKA", "NABIDKA_XML", "NABIDKA_PDF", "NABIDKA_ARCH" };

        public CacheApiService(CacheApiServiceOptions options, OfferParserService offerParser, ILogger logger)
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
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OfferModel> GetOffer(string guid, string type)
        {
            var response = await this.GetResponse(guid, type);

            if (response == null)
            {
                return null;
            }

            return this.OfferParser.GenerateOffer(response);
        }

        /// <summary>
        /// Gets the files asynchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Files in array or null</returns>
        public async Task<AttachmentModel[]> GetPdfFilesForOfferAsync(string guid)
        {
            var result = await this.GetResponse(guid, "NABIDKA");

            if (result == null)
            {
                return null;
            }

            var offer = this.OfferParser.GenerateOffer(result);

            bool isAccepted = offer.IsAccepted;

            ZCCH_ST_FILE[] files = await this.GetFiles(guid, isAccepted);

            List<AttachmentModel> fileResults = new List<AttachmentModel>();

            if (files.Length != 0 && files.All(x => x.ATTRIB.Any(y => y.ATTRID == "COUNTER")))
            {
                int index = 0;

                foreach (ZCCH_ST_FILE f in files.OrderBy(x => int.Parse(x.ATTRIB.FirstOrDefault(y => y.ATTRID == "COUNTER").ATTRVAL)))
                {
                    try
                    {
                        var customisedFileName = f.FILENAME;

                        if (f.ATTRIB.Any(any => any.ATTRID == "LINK_LABEL"))
                        {
                            var linkLabel = f.ATTRIB.FirstOrDefault(where => where.ATTRID == "LINK_LABEL");
                            customisedFileName = linkLabel.ATTRVAL;

                            var extension = Path.GetExtension(f.FILENAME);
                            customisedFileName = string.Format("{0}{1}", customisedFileName, extension);
                        }

                        var signRequired = false;
                        var fileType = string.Empty;
                        var templAlcId = string.Empty;

                        if (offer.OfferType != OfferTypes.Default)
                        {
                            if (offer.Attachments != null)
                            {
                                var fileTemplate = f.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "TEMPLATE");
                                if (fileTemplate != null)
                                {
                                    var correspondingAttachment = offer.Attachments.FirstOrDefault(attachment => attachment.IdAttach.ToLower() == fileTemplate.ATTRVAL.ToLower());

                                    if (correspondingAttachment != null)
                                    {
                                        if (correspondingAttachment.SignReq.ToLower() == "x")
                                        {
                                            signRequired = true;
                                            templAlcId = correspondingAttachment.TemplAlcId;
                                            fileType = correspondingAttachment.IdAttach;
                                        }
                                    }
                                }
                            }
                        }

                        var tempItem = new AttachmentModel();
                        tempItem.Index = (++index).ToString();
                        tempItem.FileName = customisedFileName;
                        tempItem.FileNumber = f.FILEINDX;
                        tempItem.FileType = fileType;
                        tempItem.TemplAlcId = templAlcId;
                        tempItem.SignRequired = signRequired;
                        tempItem.FileContent = f.FILECONTENT.ToArray();
                        tempItem.SignedVersion = false;
                        fileResults.Add(tempItem);
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

        public async Task<ZCCH_ST_FILE[]> GetFiles(string guid, bool isAccepted)
        {
            List<ZCCH_ST_FILE> files = new List<ZCCH_ST_FILE>();

            if (isAccepted)
            {
                var result = await this.GetResponse(guid, "NABIDKA_ARCH");
                files.AddRange(result.Response.ET_FILES);
                var filenames = result.Response.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.FILENAME));
                files.AddRange(result.Response.ET_FILES);
                //TODO: this.LogFiles(files, guid, isAccepted, "NABIDKA_ARCH");
            }
            else
            {
                var result = await this.GetResponse(guid, "NABIDKA_PDF");

                if (result.Response?.ET_FILES?.Any() ?? false)
                {
                    files.AddRange(result.Response.ET_FILES);
                }

                //TODO: this.LogFiles(files, guid, isAccepted, "NABIDKA_PDF");
            }

            return files.ToArray();
        }

        public async Task<bool> AcceptOffer(string guid)
        {
            var timestampString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            decimal outValue = 1M;

            if (decimal.TryParse(timestampString, out outValue))
            {
                bool result = await this.SetStatus(guid, "NABIDKA", outValue);
                return result;
            }
            else
            {
                this.Logger.Fatal($"Cannot parse timestamp to decimal ({timestampString})");
            }

            return false;
        }

        protected internal async Task<ResponseCacheGetModel> GetResponse(string guid, string type, string fileType = "B")
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

        protected internal async Task<bool> SetStatus(string guid, string type, decimal timestamp)
        {
            var model = new ZCCH_CACHE_STATUS_SET();
            model.IV_CCHKEY = guid;
            model.IV_CCHTYPE = type;
            model.IV_STAT = "5";
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
    }
}
