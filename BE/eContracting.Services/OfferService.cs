using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Internal;
using eContracting.Models;
using Sitecore.Data.Events;

namespace eContracting.Services
{
    /// <summary>
    /// Service wrapper over generated <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public class OfferService : IOfferService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The user data cache.
        /// </summary>
        protected readonly IUserDataCacheService UserDataCache;

        /// <summary>
        /// The user file cache.
        /// </summary>
        protected readonly IUserFileCacheService UserFileCache;

        /// <summary>
        /// The settings reader service.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// The API service factory.
        /// </summary>
        protected readonly IServiceFactory ServiceFactory;

        /// <summary>
        /// The offer parser.
        /// </summary>
        protected readonly IOfferParserService OfferParser;

        /// <summary>
        /// The attachment parser.
        /// </summary>
        protected readonly IOfferAttachmentParserService AttachmentParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userDataCache">The user data cache.</param>
        /// <param name="userFileCache">The user file cache.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <param name="factory">The factory for <see cref="ZCCH_CACHE_API"/>.</param>
        /// <param name="offerParser">The offer parser.</param>
        /// <param name="offerAttachmentParser">The offer attachment parser.</param>
        public OfferService(
            ILogger logger,
            IUserDataCacheService userDataCache,
            IUserFileCacheService userFileCache,
            ISettingsReaderService settingsReaderService,
            IServiceFactory factory,
            IOfferParserService offerParser,
            IOfferAttachmentParserService offerAttachmentParser)
        {
            this.Logger = logger;
            this.UserDataCache = userDataCache;
            this.UserFileCache = userFileCache;
            this.SettingsReaderService = settingsReaderService;
            this.OfferParser = offerParser;
            this.ServiceFactory = factory;
            this.AttachmentParser = offerAttachmentParser;
        }

        /// <inheritdoc/>
        public bool AcceptOffer(OfferModel offer, OfferSubmitDataModel data, string sessionId)
        {
            var when = DateTime.UtcNow;
            var startingLog = new StringBuilder();
            startingLog.AppendLine($"[LogAcceptance] Initializing...");
            startingLog.AppendLine($" - Guid: {offer.Guid}");
            startingLog.AppendLine($" - when: {when.ToString("yyyy-MM-dd HH:mm:ss")}");

            this.Logger.Debug(offer.Guid, startingLog.ToString());

            string timestampString = when.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;
            Decimal.TryParse(timestampString, out outValue);

            List<ZCCH_ST_ATTRIB> attributes = new List<ZCCH_ST_ATTRIB>();
            attributes.Add(new ZCCH_ST_ATTRIB()
            {
                ATTRID = "ACCEPTED_AT",
                ATTRVAL = outValue.ToString()
            });
            attributes.Add(new ZCCH_ST_ATTRIB()
            {
                ATTRID = "IP_ADDRESS",
                ATTRVAL = Utils.GetIpAddress()
            });

            this.Logger.Debug(offer.Guid, $"[LogAcceptance] Getting information about PDF files by type 'NABIDKA_PDF' ...");

            var responsePdfFiles = this.GetFiles(offer.Guid, false);
            this.AttachmentParser.MakeCompatible(offer, responsePdfFiles);

            var files = this.GetFilesForAccept(offer, data, responsePdfFiles, sessionId);

            var putResult = this.Put(offer.Guid, attributes.ToArray(), files.ToArray());

            if (putResult.ZCCH_CACHE_PUTResponse.EV_RETCODE != 0)
            {
                throw new ApplicationException("ZCCH_CACHE_PUT request failed. Code: " + putResult.ZCCH_CACHE_PUTResponse.EV_RETCODE);
            }

            return this.SetStatus(offer.Guid, OFFER_TYPES.NABIDKA, "5");
        }

        /// <inheritdoc/>
        public OfferModel GetOffer(string guid)
        {
            return this.GetOffer(guid, true);
        }

        /// <inheritdoc/>
        public OfferModel GetOffer(string guid, bool includeTextParameters)
        {
            var response = this.GetResponse(guid, OFFER_TYPES.NABIDKA);

            if (response == null)
            {
                return null;
            }

            if (response.Response.EV_RETCODE > 0)
            {
                this.Logger.Warn(guid, response.Response.ET_RETURN?.FirstOrDefault()?.MESSAGE ?? "Cannot get offer. Return code: " + response.Response.EV_RETCODE);
                return null;
            }

            return this.GetOffer(response, includeTextParameters);
        }

        protected internal OfferModel GetOffer(ResponseCacheGetModel response, bool includeTextParameters)
        {
            var offer = this.OfferParser.GenerateOffer(response);

            if (offer == null)
            {
                return null;
            }

            if (includeTextParameters)
            {
                try
                {
                    var textParameters = this.GetTextParameters(response);
                    offer.TextParameters.Merge(textParameters);
                }
                catch (XmlException ex)
                {
                    this.Logger.Fatal(offer.Guid, "XML exception when parsing text parameters", ex);
                }
            }

            return offer;
        }

        /// <inheritdoc/>
        public OfferAttachmentModel[] GetAttachments(OfferModel offer)
        {
            if (offer == null)
            {
                throw new ApplicationException("Offer is null");
            }

            bool isAccepted = offer.IsAccepted;
            ZCCH_ST_FILE[] files = this.GetFiles(offer.Guid, isAccepted);
            var attachments = this.AttachmentParser.Parse(offer, files);
            this.Logger.LogFiles(attachments, offer.Guid, isAccepted);
            return attachments;
        }

        /// <inheritdoc/>
        public bool ReadOffer(string guid)
        {
            return this.SetStatus(guid, OFFER_TYPES.NABIDKA, "4");
        }

        /// <inheritdoc/>
        public bool SignInOffer(string guid)
        {
            return this.SetStatus(guid, OFFER_TYPES.NABIDKA, "6");
        }

        /// <summary>
        /// Gets the text parameters for specific version.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Dictionary or null.</returns>
        /// <exception cref="ApplicationException">Cannot get text parameters. Unknown version {version}</exception>
        protected internal IDictionary<string, string> GetTextParameters(ResponseCacheGetModel response)
        {
            var version = this.OfferParser.GetVersion(response.Response);

            if (version == 1)
            {
                var response2 = this.GetResponse(response.Guid, OFFER_TYPES.NABIDKA_XML);

                if (response2 != null)
                {
                    var response2Files = response2.Response.ET_FILES;
                    var parameters = this.OfferParser.GetTextParameters(response2Files);
                    this.OfferParser.MakeCompatibleParameters(parameters, version);
                    return parameters;
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
            else if (version == 2)
            {
                var files = response.Response.ET_FILES.Where(x => x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE && a.ATTRVAL == Constants.FileAttributeValues.TEXT_PARAMETERS)).ToArray();

                if (!files.Any())
                {
                    this.Logger.Error(response.Guid, $"Additional file with {Constants.FileAttributes.TYPE} == {Constants.FileAttributeValues.TEXT_PARAMETERS} not found. No text parameters are available.");
                    return new Dictionary<string, string>();
                }
                else
                {
                    var parameters = this.OfferParser.GetTextParameters(files);
                    this.OfferParser.MakeCompatibleParameters(parameters, version);
                    return parameters;
                }
            }
            else
            {
                throw new ApplicationException($"Cannot get text parameters. Unknown version {version}");
            }
        }

        /// <summary>
        /// Gets all files for accept.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="data">The data.</param>
        /// <param name="responsePdfFiles">The response PDF files.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>Array of all accepted, signed and uploaded files in <paramref name="offer"/>.</returns>
        /// <exception cref="ApplicationException">
        /// Missing required file for sign: {template}
        /// or
        /// File not found in the cache: key: {uniqueKey}: template: {template}
        /// or
        /// File matching template ({template}) doesn't exist
        /// or
        /// File matching template ({template}) doesn't exist
        /// or
        /// Unknown upload group '{groupKey}'
        /// or
        /// Cannot find upload for '{groupKey}'
        /// or
        /// Group '{uploadGroup}' doesn't have content
        /// </exception>
        protected internal ZCCH_ST_FILE[] GetFilesForAccept(OfferModel offer, OfferSubmitDataModel data, ZCCH_ST_FILE[] responsePdfFiles, string sessionId)
        {
            var files = new List<ZCCH_ST_FILE>();

            // templates for sign
            var templatesRequiredForSign = offer.Documents.Where(x => x.IsSignRequired() == true && x.IsPrinted());

            foreach (var template in templatesRequiredForSign)
            {
                var uniqueKey = template.UniqueKey;

                if (!data.Signed.Any(x => x == template.UniqueKey))
                {
                    throw new ApplicationException($"Missing required file for sign: {template}");
                }

                var cacheModel = this.UserFileCache.FindSignedFile(new DbSearchParameters(uniqueKey, offer.Guid, sessionId));

                if (cacheModel == null)
                {
                    throw new ApplicationException($"File not found in the cache: key: {uniqueKey}: template: {template}");
                }

                var signedFile = cacheModel.ToAttachment();

                var file = this.AttachmentParser.GetFileByTemplate(template, responsePdfFiles);

                if (file == null)
                {
                    throw new ApplicationException($"File matching template ({template}) doesn't exist");
                }

                // replace original content with signed
                file.FILECONTENT = signedFile.FileContent;
                file.ATTRIB.Where(x => x.ATTRID.StartsWith("$TMP.ARC")).ForEach((x) => { x.ATTRVAL = string.Empty; });

                //var arccIdAttribute = file.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "$TMP.ARCCID$");
                //var arcDocAttribute = file.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "$TMP.ARCDOC$");
                //var arcArchIdAttribute = file.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "$TMP.ARCHID$");

                //arccIdAttribute.ATTRVAL = arccIdAttribute != null ? string.Empty : arccIdAttribute.ATTRVAL;
                //arcDocAttribute.ATTRVAL = arcDocAttribute != null ? string.Empty : arcDocAttribute.ATTRVAL;
                //arcArchIdAttribute.ATTRVAL = arcArchIdAttribute != null ? string.Empty : arcArchIdAttribute.ATTRVAL;

                // add signed to list of files to be sent
                files.Add(file);
            }

            // templates for accept
            var templatesOthersPrinted = offer.Documents.Where(x => x.IsSignRequired() == false && x.IsPrinted());
            var checkedFiles = data.GetCheckedFiles();

            foreach (var template in templatesOthersPrinted)
            {
                var uniqueKey = template.UniqueKey;

                if (!checkedFiles.Contains(uniqueKey))
                {
                    continue;
                }

                var file = this.AttachmentParser.GetFileByTemplate(template, responsePdfFiles);

                if (file == null)
                {
                    throw new ApplicationException($"File matching template ({template}) doesn't exist");
                }

                file.FILECONTENT = new byte[] { };
                files.Add(file);
            }

            // templates for upload
            var templatesFileForUploads = offer.Documents.Where(x => x.IsPrinted() == false);

            foreach (var uploadGroup in data.Uploaded)
            {
                var groupKey = uploadGroup.GroupKey;
                var fileKeys = uploadGroup.FileKeys;

                var template = templatesFileForUploads.FirstOrDefault(x => x.UniqueKey == groupKey);

                if (template == null)
                {
                    throw new ApplicationException($"Unknown upload group '{groupKey}'");
                }

                var uploadedFileGroup = this.UserFileCache.FindGroup(new DbSearchParameters(groupKey, offer.Guid, sessionId));

                if (uploadedFileGroup == null)
                {
                    throw new ApplicationException($"Cannot find upload for '{groupKey}'");
                }

                if (uploadedFileGroup.OutputFile.Content.Length == 0)
                {
                    throw new ApplicationException($"Group '{uploadGroup}' doesn't have content");
                }

                var file = new ZCCH_ST_FILE();
                file.ATTRIB = Utils.CreateAttributesFromTemplate(template);
                file.FILENAME = template.Description + "." + uploadedFileGroup.OutputFile.FileExtension;
                file.FILECONTENT = uploadedFileGroup.OutputFile.Content;
                file.MIMETYPE = uploadedFileGroup.OutputFile.MimeType;
                files.Add(file);
            }

            return files.ToArray();
        }

        /// <summary>
        /// Gets data.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <param name="fileType">Type of the file.</param>
        /// <returns>Instance of <see cref="ResponseCacheGetModel"/> or an exception.</returns>
        protected internal ResponseCacheGetModel GetResponse(string guid, OFFER_TYPES type, string fileType = "B")
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_GET();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
                model.IV_GEFILE = fileType;

                var request = new ZCCH_CACHE_GETRequest(model);
                var stop = new Stopwatch();
                stop.Start();
                var task = api.ZCCH_CACHE_GETAsync(request);
                task.Wait();
                var response = task.Result;
                stop.Stop();
                this.Logger.TimeSpent(model, stop.Elapsed);
                var result = response.ZCCH_CACHE_GETResponse;
                return new ResponseCacheGetModel(result);
            }
        }

        /// <summary>
        /// Sets the <paramref name="status"/> asynchronously.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="status">The status.</param>
        /// <returns><c>True</c> if status was set.</returns>
        protected internal bool SetStatus(string guid, OFFER_TYPES type, string status)
        {
            var timestampString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            decimal outValue = 1M;

            if (decimal.TryParse(timestampString, out outValue))
            {
                bool result = this.SetStatus(guid, type, outValue, status);
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
        protected internal bool SetStatus(string guid, OFFER_TYPES type, decimal timestamp, string status)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_STATUS_SET();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
                model.IV_STAT = status;
                model.IV_TIMESTAMP = timestamp;

                var request = new ZCCH_CACHE_STATUS_SETRequest(model);
                var stop = new Stopwatch();
                stop.Start();
                var task = api.ZCCH_CACHE_STATUS_SETAsync(request);
                task.Wait();
                var response = task.Result;
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
        }

        /// <summary>
        /// Gets the files asynchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="isAccepted">if set to <c>true</c> [is accepted].</param>
        /// <returns></returns>
        protected internal ZCCH_ST_FILE[] GetFiles(string guid, bool isAccepted)
        {
            List<ZCCH_ST_FILE> files = new List<ZCCH_ST_FILE>();

            if (isAccepted)
            {
                var result = this.GetResponse(guid, OFFER_TYPES.NABIDKA_ARCH);
                files.AddRange(result.Response.ET_FILES);
                var filenames = result.Response.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.FILENAME));
                files.AddRange(result.Response.ET_FILES);
                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.NABIDKA_ARCH);
            }
            else
            {
                var result = this.GetResponse(guid, OFFER_TYPES.NABIDKA_PDF);

                if (result.Response?.ET_FILES?.Any() ?? false)
                {
                    files.AddRange(result.Response.ET_FILES);
                }

                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.NABIDKA_PDF);
            }

            return files.ToArray();
        }

        /// <summary>
        /// Inserts data asynchronously.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="files">The files.</param>
        /// <returns>The response.</returns>
        protected internal ZCCH_CACHE_PUTResponse1 Put(string guid, ZCCH_ST_ATTRIB[] attributes, ZCCH_ST_FILE[] files)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_PUT();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = "NABIDKA_PRIJ";
                model.IT_ATTRIB = attributes;
                model.IT_FILES = files;

                var request = new ZCCH_CACHE_PUTRequest(model);
                var stop = new Stopwatch();
                stop.Start();
                var task = api.ZCCH_CACHE_PUTAsync(request);
                task.Wait();
                var response = task.Result;
                stop.Stop();
                //this.Logger.TimeSpent(model, stop.Elapsed);

                return response;
            }
        }
    }
}
