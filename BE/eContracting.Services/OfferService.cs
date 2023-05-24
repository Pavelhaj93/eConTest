using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.UI.Design.WebControls;
using System.Xml;
using eContracting.Models;
using Sitecore.Data.Validators.ItemValidators;

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

        protected readonly IOfferDataService DataService;

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

        protected readonly IRequestDataCacheService CacheService;

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
            IOfferDataService offerDataService,
            IOfferParserService offerParser,
            IOfferAttachmentParserService offerAttachmentParser,
            IRequestDataCacheService cacheService,
            IContextWrapper contextWrapper)
        {
            this.Logger = logger;
            this.UserFileCache = userFileCache;
            this.SettingsReaderService = settingsReaderService;
            this.DataService = offerDataService;
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
            var putResult = this.DataService.Put(offer.Guid, attributes.ToArray(), files.ToArray());

            if (putResult.ErrorCode != 0)
            {
                throw new EcontractingApplicationException(putResult.GetError("OS-AO-CP"));
            }

            var setResult = this.SetStatus(offer.Guid, OFFER_TYPES.QUOTPRX, "5");

            if (setResult.ErrorCode != 0)
            {
                throw new EcontractingApplicationException(setResult.GetError("OS-AO-CSS"));
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

            if (user.IsAuthFor(guid))
            {
                if (!user.IsAuthFor(guid, AUTH_METHODS.COGNITO))
                {
                    this.Logger.Debug(guid, $"'{user}' have Cognito data, but offer was authenticated not via {AUTH_METHODS.COGNITO}, can read offer");
                    return true;
                }
            }

            var userId = user.CognitoUser?.PreferredUsername;
            var cacheKey = this.GetUserCheckCacheKey(guid, userId, type);
            var response = this.CacheService.Get<ResponseAccessCheckModel>(cacheKey);

            if (response != null)
            {
                this.Logger.Info(guid, "Can read info response loaded from the cache.");
            }
            else
            {
                response = this.DataService.UserAccessCheck(guid, userId, type);
                this.CacheService.Set(cacheKey, response);
                this.Logger.Info(guid, "Can read info response stored into the cache.");
            }

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
                var canRead = this.CanReadOffer(guid, user, OFFER_TYPES.QUOTPRX);

                if (!canRead)
                {
                    this.Logger.Info(guid, $"{user} cannot read offer");
                    return null;
                }
            }

            var cacheKey = this.GetOfferCacheKey(guid, user, includeTextParameters);
            var cachedOffer = this.CacheService.Get<OfferModel>(cacheKey);

            if (cachedOffer != null)
            {
                this.Logger.Info(cachedOffer.Guid, "Offer data loaded from the cache.");
                return cachedOffer;
            }

            var response = this.DataService.GetResponse(guid, OFFER_TYPES.QUOTPRX);

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

            var offer = this.GetOffer(response, includeTextParameters);
            this.CacheService.Set(cacheKey, offer);
            this.Logger.Info(guid, "Offer data stored into the cache.");
            return offer;
        }

        /// <inheritdoc/>
        public void ReadOffer(string guid, UserCacheDataModel user)
        {
            var response = this.SetStatus(guid, OFFER_TYPES.QUOTPRX, "4");

            if (response.ErrorCode != 0)
            {
                throw new EcontractingApplicationException(response.GetError("OF-RO-CSS"));
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

            var response = this.SetStatus(guid, OFFER_TYPES.QUOTPRX, status);

            if (response.ErrorCode != 0)
            {
                throw new EcontractingApplicationException(response.GetError("OF-SIO-CSS"));
            }
        }

        /// <inheritdoc/>
        public void CancelOffer(string guid)
        {
            var response = this.SetStatus(guid, OFFER_TYPES.QUOTPRX, "9");

            if (response.ErrorCode != 0)
            {
                throw new EcontractingApplicationException(response.GetError("OF-RO-CAN"));
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
                var response2 = this.DataService.GetResponse(response.Guid, OFFER_TYPES.QUOTPRX_XML);

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
            else if (offer.Version == 3)
            {
                var parameters = new Dictionary<string, string>();
                var ad1files = response.Response.ET_FILES.Where(x => x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE && a.ATTRVAL == Constants.FileAttributeValues.TEXT_PARAMETERS)).ToArray();

                if (!ad1files.Any())
                {
                    this.Logger.Error(response.Guid, $"Additional file with {Constants.FileAttributes.TYPE} == {Constants.FileAttributeValues.TEXT_PARAMETERS} not found. No text parameters are available.");
                    return new Dictionary<string, string>();
                }
                else
                {
                    parameters.Merge(this.OfferParser.GetTextParameters(ad1files.Select(x => new OfferFileXmlModel(x)).ToArray()));
                    this.OfferParser.MakeCompatibleParameters(parameters, offer.Version);

                    var cbFiles = response.Response.ET_FILES.Where(x => x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE && (a.ATTRVAL == Constants.FileAttributeValues.EXTRA_PARAMETERS_E || a.ATTRVAL == Constants.FileAttributeValues.EXTRA_PARAMETERS_P))).ToArray();

                    if (cbFiles.Length == 0)
                    {
                        this.Logger.Error(response.Guid, $"Missing file '{Constants.FileAttributeValues.EXTRA_PARAMETERS_E}' for version {offer.Version}.");
                        //throw new EcontractingApplicationException(new ErrorModel("OF_AT_CB", $"Missing file '{Constants.FileAttributeValues.EXTRA_PARAMETERS_E}/{Constants.FileAttributeValues.EXTRA_PARAMETERS_P}' for version {offer.Version}."));
                    }
                    else
                    {
                        var preferred = cbFiles.FirstOrDefault(x => x.FILENAME.EndsWith("_PREFERRED.xml"));
                        
                        if (preferred != null)
                        {
                            parameters.Merge(this.OfferParser.GetTextParameters(new[] { new OfferFileXmlModel(preferred) }));
                            this.Logger.Debug(response.Guid, $"Additional text parameters merged from '{preferred.FILENAME}'");
                        }
                        else
                        {
                            parameters.Merge(this.OfferParser.GetTextParameters(cbFiles.Select(x => new OfferFileXmlModel(x)).ToArray()));
                            this.Logger.Debug(response.Guid, $"Additional text parameters merged from '{string.Join(", ", cbFiles.Select(x => x.FILENAME).ToArray())}'");
                        }
                    }

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

            if (user.IsCognito)
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
        /// Sets the <paramref name="status"/> asynchronously.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="status">The status.</param>
        /// <returns>Response from inner service.</returns>
        protected internal ResponseStatusSetModel SetStatus(string guid, OFFER_TYPES type, string status)
        {
            var timestampString = DateTime.UtcNow.ToString(Constants.TimeStampFormat);
            decimal outValue = 1M;
            Decimal.TryParse(timestampString, out outValue);
            var response = this.DataService.SetStatus(guid, type, outValue, status);
            return response;
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
                var result = this.DataService.GetResponse(guid, OFFER_TYPES.QUOTPRX_ARCH);

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

                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.QUOTPRX_ARCH);
            }
            else
            {
                var result = this.DataService.GetResponse(guid, OFFER_TYPES.QUOTPRX_PDF);

                if (result.Response.ET_FILES?.Length > 0)
                {
                    files.AddRange(result.Response.ET_FILES.Select(x => new OfferFileXmlModel(x)));
                }

                this.Logger.LogFiles(files, guid, isAccepted, OFFER_TYPES.QUOTPRX_PDF);
            }

            return files.ToArray();
        }

        protected internal string GetOfferCacheKey(string guid, UserCacheDataModel user, bool includeTextParameters)
        {
            var builder = new StringBuilder();
            builder.Append($"guid={guid}");
            
            if (user != null)
            {
                builder.Append($"&user={user.Id}");
            }

            builder.Append($"&includeTextParameters={includeTextParameters}");
            return builder.ToString();
        }

        protected internal string GetUserCheckCacheKey(string guid, string userId, OFFER_TYPES type)
        {
            var builder = new StringBuilder();
            builder.Append($"guid={guid}");
            builder.Append($"userId={userId}");
            builder.Append($"offerType={Enum.GetName(typeof(OFFER_TYPES), type)}");
            return builder.ToString();
        }
    }
}
