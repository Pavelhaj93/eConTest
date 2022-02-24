using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using eContracting.Models;

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

        protected readonly IDataRequestCacheService CacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userService">The user data service.</param>
        /// <param name="userFileCache">The user file cache.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <param name="factory">The factory for <see cref="ZCCH_CACHE_API"/>.</param>
        /// <param name="offerParser">The offer parser.</param>
        /// <param name="offerAttachmentParser">The offer attachment parser.</param>
        /// <param name="contextWrapper">The context wrapper.</param>
        public OfferService(
            ILogger logger,
            IUserFileCacheService userFileCache,
            ISettingsReaderService settingsReaderService,
            IServiceFactory factory,
            IOfferParserService offerParser,
            IOfferAttachmentParserService offerAttachmentParser,
            IDataRequestCacheService cacheService,
            IContextWrapper contextWrapper)
        {
            this.Logger = logger;
            this.UserFileCache = userFileCache;
            this.SettingsReaderService = settingsReaderService;
            this.OfferParser = offerParser;
            this.ServiceFactory = factory;
            this.AttachmentParser = offerAttachmentParser;
            this.Context = contextWrapper;
            this.CacheService = cacheService;
        }

        #region IOfferService

        /// <inheritdoc/>
        public void AcceptOffer(OfferModel offer, OfferSubmitDataModel data, UserCacheDataModel user, string sessionId)
        {
            var when = DateTime.UtcNow;
            var startingLog = new StringBuilder();
            startingLog.AppendLine($"[LogAcceptance] Initializing...");
            startingLog.AppendLine($" - Guid: {offer.Guid}");
            startingLog.AppendLine($" - when: {when.ToString("yyyy-MM-dd HH:mm:ss")}");

            this.Logger.Debug(offer.Guid, startingLog.ToString());

            var attributes = this.GetAttributesForAccept(offer, data, user, when);

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
        public bool CanReadOffer(string guid, UserCacheDataModel user, OFFER_TYPES type)
        {
            // this is check for anonymous user
            if (!user.IsCognito)
            {
                this.Logger.Debug(guid, $"'{user}' doens't have {AUTH_METHODS.COGNITO} data, can read offer.");
                return true;
            }

            if (user.AuthorizedGuids.ContainsKey(guid))
            {
                if (user.AuthorizedGuids[guid] != AUTH_METHODS.COGNITO)
                {
                    this.Logger.Debug(guid, $"'{user}' have Cognito data, but offer was authenticated not via {AUTH_METHODS.COGNITO}, can read offer");
                    return true;
                }
            }

            var userId = user.CognitoUser?.PreferredUsername;
            var response = this.UserAccessCheck(guid, userId, type);

            // this happens when user is not logged in
            if (response == null)
            {
                this.Logger.Info(guid, $"[{nameof(OfferService)}] User can read offer because UserAccessCheck returns null (no auth check).");
                return true;
            }

            if (response.Response.EV_RETCODE == 0)
            {
                this.Logger.Debug(guid, $"[{nameof(OfferService)}] User can read offer");
                return true;
            }

            if (response.Response.EV_RETCODE == 4)
            {
                var log = new StringBuilder();
                log.AppendLine($"[{nameof(OfferService)}] Uživatel nemá autorizaci / neexistuje Cache záznam / Neexistuje user alias");
                log.AppendLine(ERROR_CODES.FromResponse("", response.Response).Description);
                this.Logger.Info(guid, log.ToString());
                return false;
            }

            if (response.Response.EV_RETCODE == 8)
            {
                var log = new StringBuilder();
                log.AppendLine("Chyba připojení (RFC)");
                log.AppendLine(ERROR_CODES.FromResponse("", response.Response).Description);
                this.Logger.Error(guid, log.ToString());
                return false;
            }

            this.Logger.Error(guid, ERROR_CODES.FromResponse("", response.Response).Description);
            return false;
        }

        /// <inheritdoc/>
        public OfferModel GetOffer(string guid)
        {
            return this.GetOffer(guid, null);
        }

        /// <inheritdoc/>
        public OfferModel GetOffer(string guid, UserCacheDataModel user)
        {
            return this.GetOffer(guid, user, true);
        }

        /// <inheritdoc/>
        public OfferModel GetOffer(string guid, UserCacheDataModel user, bool includeTextParameters)
        {
            if (user == null)
            {
                this.Logger.Info(guid, "User is null, reading offer as anonymouse.");
            }
            else
            {
                var canRead = this.CanReadOffer(guid, user, OFFER_TYPES.NABIDKA);

                if (!canRead)
                {
                    this.Logger.Info(guid, $"{user} cannot read offer");
                    return null;
                }
            }

            var response = this.GetResponse(guid, OFFER_TYPES.NABIDKA);

            if (response == null)
            {
                this.Logger.Warn(guid, "Response is null");
                return null;
            }

            if (response.Response.EV_RETCODE > 0)
            {
                this.Logger.Warn(guid, response.Response.ET_RETURN?.FirstOrDefault()?.MESSAGE ?? "Cannot get offer. Return code: " + response.Response.EV_RETCODE);
                return null;
            }

            return this.GetOffer(response, includeTextParameters);
        }

        /// <inheritdoc/>
        public void ReadOffer(string guid, UserCacheDataModel user)
        {
            var response = this.SetStatus(guid, OFFER_TYPES.NABIDKA, "4");

            if (response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE != 0)
            {
                throw new EcontractingApplicationException(ERROR_CODES.FromResponse("OF-RO-CSS", response.ZCCH_CACHE_STATUS_SETResponse));
            }
        }

        /// <inheritdoc/>
        public void SignInOffer(string guid, UserCacheDataModel user)
        {
            var status = "6";
            
            if (user?.IsCognitoGuid(guid) ?? false)
            {
                status = "I";
            }

            var response = this.SetStatus(guid, OFFER_TYPES.NABIDKA, status);

            if (response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE != 0)
            {
                throw new EcontractingApplicationException(ERROR_CODES.FromResponse("OF-SIO-CSS", response.ZCCH_CACHE_STATUS_SETResponse));
            }
        }

        /// <inheritdoc/>
        public OfferAttachmentModel[] GetAttachments(OfferModel offer, UserCacheDataModel user)
        {
            if (offer == null)
            {
                throw new ApplicationException("Offer is null");
            }

            bool isAccepted = offer.IsAccepted;
            var files = this.GetFiles(offer.Guid, isAccepted);
            return this.GetAttachments(offer, files);
        }

        #endregion

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
                    var textParameters = this.GetTextParameters(offer, response);
                    offer.TextParameters.Merge(textParameters);
                }
                catch (XmlException ex)
                {
                    this.Logger.Fatal(offer.Guid, "XML exception when parsing text parameters", ex);
                }
            }

            return offer;
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

        /// <summary>
        /// Gets the text parameters for specific version.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="response">The response.</param>
        /// <returns>Dictionary or null.</returns>
        /// <exception cref="ApplicationException">Cannot get text parameters. Unknown version {version}</exception>
        protected internal IDictionary<string, string> GetTextParameters(OfferModel offer, ResponseCacheGetModel response)
        {
            if (offer.Version == 1)
            {
                var response2 = this.GetResponse(response.Guid, OFFER_TYPES.NABIDKA_XML);

                if (response2 != null && response2.Response.ET_FILES?.Length > 0)
                {
                    var response2Files = response2.Response.ET_FILES.Select(x => new OfferFileXmlModel(x)).ToArray();
                    var parameters = this.OfferParser.GetTextParameters(response2Files);
                    this.OfferParser.MakeCompatibleParameters(parameters, offer.Version);
                    return parameters;
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
            else if (offer.Version == 2)
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
                    this.OfferParser.MakeCompatibleParameters(parameters, offer.Version);
                    return parameters;
                }
            }
            else
            {
                throw new ApplicationException($"Cannot get text parameters. Unknown version {offer.Version}");
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

                var file = this.AttachmentParser.GetFileByTemplate(offer, template, responsePdfFiles);

                // replace original content with signed
                file.File.FILECONTENT = signedFile.FileContent;

                foreach(var x in file.File.ATTRIB.Where(x => x.ATTRID.StartsWith("$TMP.ARC")))
                {
                    x.ATTRVAL = string.Empty;
                }

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

                var file = this.AttachmentParser.GetFileByTemplate(offer, template, responsePdfFiles);
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

        protected internal ZCCH_ST_ATTRIB[] GetAttributesForAccept(OfferModel offer, OfferSubmitDataModel data, UserCacheDataModel user, DateTime when)
        {
            string timestampString = when.ToString(Constants.TimeStampFormat);
            Decimal outValue = 1M;
            Decimal.TryParse(timestampString, out outValue);

            var attributes = new List<ZCCH_ST_ATTRIB>();
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
            attributes.Add(new ZCCH_ST_ATTRIB()
            {
                ATTRID = Constants.OfferAttributes.ZIDENTITYID,
                ATTRVAL = offer.GdprIdentity
            });

            if (!string.IsNullOrEmpty(data.Supplier))
            {
                attributes.Add(new ZCCH_ST_ATTRIB()
                {
                    ATTRID = "SERVPROV_OLD",
                    ATTRVAL = data.Supplier
                });
            }

            if (user.HasAuth(AUTH_METHODS.COGNITO))
            {
                attributes.Add(new ZCCH_ST_ATTRIB()
                {
                    ATTRID = "ACCEPTED_BY_ALIAS",
                    ATTRVAL = user.CognitoUser.PreferredUsername
                });
            }

            return attributes.ToArray();
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

                this.Logger.Info(guid, this.GetLogMessage(model));

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

                    if (result == null)
                    {
                        this.Logger.Error(guid, "Response ZCCH_CACHE_GETResponse1 is null");
                        return null;
                    }

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

                this.Logger.Info(guid, this.GetLogMessage(model));

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
        /// Calling <see cref="ZCCH_CACHE_ACCESS_CHECK"/> to check if Cognito user has access to offer with <paramref name="guid"/>.
        /// </summary>
        /// <param name="user">User data with <see cref="UserCacheDataModel.Guid"/>.</param>
        /// <param name="type">A type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <returns><see cref="ResponseAccessCheckModel"/> model when request was successful. If user is not Cognito, returns <c>null</c>.</returns>
        /// <exception cref="EcontractingDataException">When call to <see cref="ZCCH_CACHE_ACCESS_CHECKRequest"/> failed.</exception>
        protected internal ResponseAccessCheckModel UserAccessCheck(string guid, string userId, OFFER_TYPES type)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_ACCESS_CHECK();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);

                var attributes = new List<ZCCH_ST_ATTRIB>();
                attributes.Add(new ZCCH_ST_ATTRIB()
                {
                    ATTRID = "USER_ALIAS",
                    ATTRVAL = userId
                });

                model.IT_ATTRIB = attributes.ToArray();

                this.Logger.Info(guid, this.GetLogMessage(model));

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_ACCESS_CHECK)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_ACCESS_CHECKRequest(model);
                    var task = api.ZCCH_CACHE_ACCESS_CHECKAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_ACCESS_CHECKResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    var result = response.ZCCH_CACHE_ACCESS_CHECKResponse;
                    return new ResponseAccessCheckModel(result);
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw new EcontractingDataException(new ErrorModel("ZCCH_CACHE_ACCESS_CHECK", "Cannot check user"), ex);
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
            //TODO: var user = this.AuthService.GetCurrentUser();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_PUT();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = "NABIDKA_PRIJ";
                model.IT_ATTRIB = attributes;
                model.IT_FILES = files.Select(x => x.File).ToArray();

                this.Logger.Info(guid, this.GetLogMessage(model));

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

        private string GetLogMessage(ZCCH_CACHE_GET model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_GET)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:  {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE: {model.IV_CCHTYPE}");
            log.AppendLine($" - IV_GEFILE:  {model.IV_GEFILE}");
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_CACHE_PUT model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_PUT)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:  {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE: {model.IV_CCHTYPE}");
            log.Append(this.GetLogMessage(model.IT_ATTRIB));
            log.Append(this.GetLogMessage(model.IT_FILES));
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_CACHE_STATUS_SET model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_STATUS_SET)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:    {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE:   {model.IV_CCHTYPE}");
            log.AppendLine($" - IV_STAT:      {model.IV_STAT}");
            log.AppendLine($" - IV_TIMESTAMP: {model.IV_TIMESTAMP}");
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_CACHE_ACCESS_CHECK model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_ACCESS_CHECK)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:  {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE: {model.IV_CCHTYPE}");
            log.Append(this.GetLogMessage(model.IT_ATTRIB));
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_ST_ATTRIB[] attributes)
        {
            var log = new StringBuilder();

            if (attributes != null && attributes.Length > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    var attr = attributes[i];

                    if (attr != null)
                    {
                        log.AppendLine($" - IT_ATTRIB[{i}]{attr.ATTRID}: {attr.ATTRVAL}");
                    }
                }
            }

            return log.ToString();
        }

        private string GetLogMessage(ZCCH_ST_FILE[] files)
        {
            var log = new StringBuilder();

            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    var f = files[i];

                    if (f != null)
                    {
                        log.AppendLine($" - IT_FILES[{i}]FILENAME:  {f.FILENAME}");
                    }
                }
            }

            return log.ToString();
        }
    }
}
