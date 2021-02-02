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
        /// The context wrapper.
        /// </summary>
        protected readonly IContextWrapper Context;

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
        /// <param name="contextWrapper">The context wrapper.</param>
        public OfferService(
            ILogger logger,
            IUserDataCacheService userDataCache,
            IUserFileCacheService userFileCache,
            ISettingsReaderService settingsReaderService,
            IServiceFactory factory,
            IOfferParserService offerParser,
            IOfferAttachmentParserService offerAttachmentParser,
            IContextWrapper contextWrapper)
        {
            this.Logger = logger;
            this.UserDataCache = userDataCache;
            this.UserFileCache = userFileCache;
            this.SettingsReaderService = settingsReaderService;
            this.OfferParser = offerParser;
            this.ServiceFactory = factory;
            this.AttachmentParser = offerAttachmentParser;
            this.Context = contextWrapper;
        }

        /// <inheritdoc/>
        public void AcceptOffer(OfferModel offer, OfferSubmitDataModel data, string sessionId)
        {
            var when = DateTime.UtcNow;
            var startingLog = new StringBuilder();
            startingLog.AppendLine($"[LogAcceptance] Initializing...");
            startingLog.AppendLine($" - Guid: {offer.Guid}");
            startingLog.AppendLine($" - when: {when.ToString("yyyy-MM-dd HH:mm:ss")}");

            this.Logger.Debug(offer.Guid, startingLog.ToString());

            string timestampString = when.ToString(Constants.TimeStampFormat);
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
                ATTRVAL = this.Context.GetIpAddress()
            });

            this.Logger.Debug(offer.Guid, $"[LogAcceptance] Getting information about PDF files by type 'NABIDKA_PDF' ...");

            var responsePdfFiles = this.GetFiles(offer.Guid, false);
            this.AttachmentParser.MakeCompatible(offer, responsePdfFiles);

            var files = this.GetFilesForAccept(offer, data, responsePdfFiles, sessionId);

            var putResult = this.Put(offer.Guid, attributes.ToArray(), files.ToArray());

            if (putResult.ZCCH_CACHE_PUTResponse.EV_RETCODE != 0)
            {
                throw new EcontractingApplicationException(ERROR_CODES.FromResponse("OS-AO-CP", putResult.ZCCH_CACHE_PUTResponse));
            }

            var setResult = this.SetStatus(offer.Guid, OFFER_TYPES.NABIDKA, "5");

            if (setResult.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE != 0)
            {
                throw new EcontractingApplicationException(ERROR_CODES.FromResponse("OS-AO-CSS", setResult.ZCCH_CACHE_STATUS_SETResponse));
            }
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

        /// <summary>
        /// Gets the offer from given <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="includeTextParameters">if set to <c>true</c> [include text parameters].</param>
        /// <returns></returns>
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
            var files = this.GetFiles(offer.Guid, isAccepted);
            return this.GetAttachments(offer, files);
        }

        /// <summary>
        /// Gets offer attachments generated from <paramref name="files"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="files">The files.</param>
        /// <returns>Array of attachments.</returns>
        protected internal OfferAttachmentModel[] GetAttachments(OfferModel offer, OfferFileXmlModel[] files)
        {
            var attachments = this.AttachmentParser.Parse(offer, files);
            this.Logger.LogFiles(attachments, offer.Guid, offer.IsAccepted);
            return attachments;
        }

        /// <inheritdoc/>
        public void ReadOffer(string guid)
        {
            var response = this.SetStatus(guid, OFFER_TYPES.NABIDKA, "4");

            if (response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE != 0)
            {
                throw new EcontractingApplicationException(ERROR_CODES.FromResponse("OF-RO-CSS", response.ZCCH_CACHE_STATUS_SETResponse));
            }
        }

        /// <inheritdoc/>
        public void SignInOffer(string guid)
        {
            var response = this.SetStatus(guid, OFFER_TYPES.NABIDKA, "6");

            if (response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE != 0)
            {
                throw new EcontractingApplicationException(ERROR_CODES.FromResponse("OF-SIO-CSS", response.ZCCH_CACHE_STATUS_SETResponse));
            }
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

                if (response2 != null && response2.Response.ET_FILES?.Length > 0)
                {
                    var response2Files = response2.Response.ET_FILES.Select(x => new OfferFileXmlModel(x)).ToArray();
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
                    var parameters = this.OfferParser.GetTextParameters(files.Select(x => new OfferFileXmlModel(x)).ToArray());
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
        protected internal OfferFileXmlModel[] GetFilesForAccept(OfferModel offer, OfferSubmitDataModel data, OfferFileXmlModel[] responsePdfFiles, string sessionId)
        {
            var files = new List<OfferFileXmlModel>();

            // templates for sign
            var templatesRequiredForSign = offer.Documents.Where(x => x.IsSignRequired() == true && x.IsPrinted());

            foreach (var template in templatesRequiredForSign)
            {
                var uniqueKey = template.UniqueKey;

                if (!data.Signed.Any(x => x == template.UniqueKey)) // check data.Signed == null
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
                file.File.FILECONTENT = signedFile.FileContent;
                file.File.ATTRIB.Where(x => x.ATTRID.StartsWith("$TMP.ARC")).ForEach((x) => { x.ATTRVAL = string.Empty; });

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

                file.File.FILECONTENT = new byte[] { };
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
                files.Add(new OfferFileXmlModel(file));
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

                var log = new StringBuilder();
                log.AppendLine($"Calling {nameof(ZCCH_CACHE_GET)} with parameters:");
                log.AppendLine(" IV_CCHKEY: " + model.IV_CCHKEY);
                log.AppendLine(" IV_CCHTYPE: " + model.IV_CCHTYPE);
                log.AppendLine(" IV_GEFILE: " + model.IV_GEFILE);
                this.Logger.Info(guid, log.ToString());

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_GET)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_GETRequest(model);
                    var task = api.ZCCH_CACHE_GETAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_GETResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    var result = response.ZCCH_CACHE_GETResponse;
                    return new ResponseCacheGetModel(result);
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Sets the <paramref name="status"/> asynchronously.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="status">The status.</param>
        /// <returns>Response from inner service.</returns>
        protected internal ZCCH_CACHE_STATUS_SETResponse1 SetStatus(string guid, OFFER_TYPES type, string status)
        {
            var timestampString = DateTime.UtcNow.ToString(Constants.TimeStampFormat);
            decimal outValue = 1M;
            Decimal.TryParse(timestampString, out outValue);
            var response = this.SetStatus(guid, type, outValue, status);
            return response;
        }

        /// <summary>
        /// Sets the <paramref name="status"/> asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">A type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <param name="timestamp">Decimal representation of a timestamp.</param>
        /// <param name="status">Value for <see cref="ZCCH_CACHE_STATUS_SET.IV_STAT"/>.</param>
        /// <returns>Response from inner service.</returns>
        protected internal ZCCH_CACHE_STATUS_SETResponse1 SetStatus(string guid, OFFER_TYPES type, decimal timestamp, string status)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_STATUS_SET();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
                model.IV_STAT = status;
                model.IV_TIMESTAMP = timestamp;

                var log = new StringBuilder();
                log.AppendLine($"Calling {nameof(ZCCH_CACHE_STATUS_SET)} with parameters:");
                log.AppendLine(" IV_CCHKEY: " + model.IV_CCHKEY);
                log.AppendLine(" IV_CCHTYPE: " + model.IV_CCHTYPE);
                log.AppendLine(" IV_STAT: " + model.IV_STAT);
                log.AppendLine(" IV_TIMESTAMP: " + model.IV_TIMESTAMP);
                this.Logger.Info(guid, log.ToString());

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_GET)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_STATUS_SETRequest(model);
                    var task = api.ZCCH_CACHE_STATUS_SETAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    return response;
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the files asynchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="isAccepted">if set to <c>true</c> [is accepted].</param>
        /// <returns></returns>
        protected internal OfferFileXmlModel[] GetFiles(string guid, bool isAccepted)
        {
            var files = new List<OfferFileXmlModel>();

            if (isAccepted)
            {
                var result = this.GetResponse(guid, OFFER_TYPES.NABIDKA_ARCH);

                if (result.Response.ET_FILES?.Length > 0)
                {
                    files.AddRange(result.Response.ET_FILES.Select(x => new OfferFileXmlModel(x)));
                }

                var filenames = result.Response.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.File.FILENAME));

                if (result.Response.ET_FILES?.Length > 0)
                {
                    files.AddRange(result.Response.ET_FILES.Select(x => new OfferFileXmlModel(x)));
                }

                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.NABIDKA_ARCH);
            }
            else
            {
                var result = this.GetResponse(guid, OFFER_TYPES.NABIDKA_PDF);

                if (result.Response.ET_FILES?.Length > 0)
                {
                    files.AddRange(result.Response.ET_FILES.Select(x => new OfferFileXmlModel(x)));
                }

                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.NABIDKA_PDF);
            }

            return files.ToArray();
        }

        /// <summary>
        /// Inserts data with 'NABIDKA_PRIJ'.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="files">The files.</param>
        /// <returns>The response.</returns>
        protected internal ZCCH_CACHE_PUTResponse1 Put(string guid, ZCCH_ST_ATTRIB[] attributes, OfferFileXmlModel[] files)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_PUT();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = "NABIDKA_PRIJ";
                model.IT_ATTRIB = attributes;
                model.IT_FILES = files.Select(x => x.File).ToArray();

                var log = new StringBuilder();
                log.AppendLine($"Calling {nameof(ZCCH_CACHE_PUT)} with parameters:");
                log.AppendLine($" - IV_CCHKEY: {model.IV_CCHKEY}");
                log.AppendLine($" - IV_CCHTYPE: {model.IV_CCHTYPE}");

                try
                {
                    log.AppendLine(" Attributes:");

                    for (int i = 0; i < attributes.Length; i++)
                    {
                        log.AppendLine($"  - {attributes[i].ATTRID}: {attributes[i].ATTRVAL}");
                    }

                    log.AppendLine(" Files:");

                    for (int i = 0; i < files.Length; i++)
                    {
                        log.AppendLine($"  - {files[i].File.FILENAME}");
                    }

                    this.Logger.Info(guid, log.ToString());
                }
                catch (Exception ex)
                {
                    this.Logger.Error(guid, $"Cannot log information about '{nameof(ZCCH_CACHE_PUT)}'", ex);
                }

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_PUT)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_PUTRequest(model);
                    var task = api.ZCCH_CACHE_PUTAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_PUTResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    return response;
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw ex;
                }
            }
        }
    }
}
