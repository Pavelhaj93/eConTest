// <copyright file="RweClient.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Xml;
    using System.Xml.Serialization;
    using eContracting.Kernel.GlassItems;
    using eContracting.Kernel.Helpers;
    using eContracting.Kernel.Models;
    using Sitecore.Analytics.Data.DataAccess.MongoDb;
    using Sitecore.Diagnostics;

    delegate T CallServiceMethod<T, P>(P inputParams);

    /// <summary>
    /// Implementation of SAP client.
    /// </summary>
    public class RweClient
    {
        #region Public methods
        /// <summary>
        /// Generates pdf files on server.
        /// </summary>
        /// <param name="guid">Uuid of the fiels to generate.</param>
        /// <returns>Rerurn list of urls of files for download.</returns>
        public List<FileToBeDownloaded> GeneratePDFFiles(string guid)
        {
            var responseObject = GetResponse(guid, "NABIDKA");

            if (responseObject == null)
            {
                return null;
            }

            var offer = GenerateXml(guid, responseObject);

            if (offer == null)
            {
                return null;
            }

            bool IsAccepted = responseObject.ET_ATTRIB != null && responseObject.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT");

            //ZCCH_CACHE_GETResponse result = null;
            //List<ZCCH_ST_FILE> files = new List<ZCCH_ST_FILE>();

            //if (IsAccepted)
            //{
            //    result = GetResponse(guid, "NABIDKA_PDF");
            //    files.AddRange(result.ET_FILES);
            //    result = GetResponse(guid, "NABIDKA_PRIJ");
            //    var filenames = result.ET_FILES.Select(file => file.FILENAME);
            //    files.RemoveAll(file => filenames.Contains(file.FILENAME));
            //    files.AddRange(result.ET_FILES);
            //}
            //else
            //{
            //    result = GetResponse(guid, "NABIDKA_PDF");
            //    files.AddRange(result.ET_FILES);
            //}

            ZCCH_ST_FILE[] files = this.GetFiles(guid, IsAccepted);

            List<FileToBeDownloaded> fileResults = new List<FileToBeDownloaded>();

            if (files.Length != 0 && files.All(x => x.ATTRIB.Any(y => y.ATTRID == "COUNTER")))
            {
                int index = 0;

                foreach (ZCCH_ST_FILE f in files.OrderBy(x => int.Parse(x.ATTRIB.FirstOrDefault(y => y.ATTRID == "COUNTER").ATTRVAL)))
                {
                    try
                    {
                        var customisedFileName = f.FILENAME;
                        var associatedAttachment = null as Template;

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

                        if (offer.OfferInternal.Body.OfferType != OfferTypes.Default)
                        {
                            if (offer.OfferInternal.Body.Attachments != null)
                            {
                                var fileTemplate = f.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "TEMPLATE");
                                if (fileTemplate != null)
                                {
                                    var correspondingAttachment = offer.OfferInternal.Body.Attachments.FirstOrDefault(attachment => attachment.IdAttach.ToLower() == fileTemplate.ATTRVAL.ToLower());

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

                        FileToBeDownloaded tempItem = new FileToBeDownloaded();
                        tempItem.Index = (++index).ToString();
                        tempItem.FileName = customisedFileName;
                        tempItem.FileNumber = f.FILEINDX;
                        tempItem.FileType = fileType;
                        tempItem.TemplAlcId = templAlcId;
                        tempItem.SignRequired = signRequired;
                        tempItem.FileContent = f.FILECONTENT.ToList();
                        tempItem.SignedVersion = false;
                        fileResults.Add(tempItem);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[{guid}] Exception occured when parsing file list", ex, this);
                    }
                }

                this.LogFiles(fileResults, guid, IsAccepted);
                return fileResults;
            }

            this.LogFiles(null, guid, IsAccepted);
            return null;
        }

        private void LogFiles(List<FileToBeDownloaded> files, string guid, bool IsAccepted)
        {
            var template = "[{0}] Offer:[{1}]{2} Generated PDF files(Documents to download):{3}{4}";
            var accept = IsAccepted ? "Accepted" : "Not Accepted";

            if (files == null || !files.Any())
            {
                var noFiles = string.Format(template, guid, accept, Environment.NewLine, "[No files generated]", Environment.NewLine);
                Log.Debug(noFiles, this);
                return;
            }

            var builder = new StringBuilder();
            var counter = 1;

            foreach (var file in files)
            {
                var line = string.Format("[{0}] Sign Required:[{1}] {2} {3}", counter++, file.SignRequired ? "Yes" : "No", file.FileName, Environment.NewLine);
                builder.Append(line);
            }

            var tmp = string.Format(template, guid, accept, Environment.NewLine, Environment.NewLine, builder.ToString());
            Log.Debug(tmp, this);
        }

        private void LogFiles(List<ZCCH_ST_FILE> files, string guid, bool IsAccepted, string responseType)
        {
            var template = "[{0}] Offer:[{1}] Response Type:[{2}]{3} Received files:{4}{5}";
            var accept = IsAccepted ? "Accepted" : "Not Accepted";

            if (files == null || !files.Any()) 
            {
                var noFiles = string.Format(template, guid, accept, responseType, Environment.NewLine, "No files received", Environment.NewLine);
                Log.Debug(noFiles, this);
                return;
            }

            var builder = new StringBuilder();
            var counter = 1;

            foreach (var file in files)
            {
                var line = string.Format("[{0}] {1}{2}", counter++, file.FILENAME, Environment.NewLine);
                builder.Append(line);
            }

            var res = string.Format(template, guid, accept, responseType, Environment.NewLine, Environment.NewLine, builder.ToString());
            Log.Debug(res, this);
        }

        public ZCCH_ST_FILE[] GetFiles(string guid, bool IsAccepted)
        {
            ZCCH_CACHE_GETResponse result = null;
            List<ZCCH_ST_FILE> files = new List<ZCCH_ST_FILE>();

            if (IsAccepted)
            {
                result = GetResponse(guid, "NABIDKA_ARCH");
                files.AddRange(result.ET_FILES);
                var filenames = result.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.FILENAME));
                files.AddRange(result.ET_FILES);
                this.LogFiles(files, guid, IsAccepted, "NABIDKA_ARCH");
            }
            else
            {
                result = GetResponse(guid, "NABIDKA_PDF");

                if (result?.ET_FILES?.Any() ?? false)
                {
                    files.AddRange(result.ET_FILES);
                }

                this.LogFiles(files, guid, IsAccepted, "NABIDKA_PDF");
            }
            
            return files.ToArray();
        }
        
        /// <summary>
        /// Generates xml for offer.
        /// </summary>
        /// <param name="guid">Uuid of offer.</param>
        /// <param name="responseObject">Already got response object.</param>
        /// <returns></returns>
        public Offer GenerateXml(string guid, ZCCH_CACHE_GETResponse responseObject)
        {
            if (responseObject.ThereAreFiles())
            {
                var file = Encoding.UTF8.GetString(responseObject.ET_FILES.First().FILECONTENT);

                using (var stream = new MemoryStream(responseObject.ET_FILES.First().FILECONTENT, false))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Offer), "http://www.sap.com/abapxml");
                    Offer offer = (Offer)serializer.Deserialize(stream);
                    offer.OfferInternal.IsAccepted = responseObject.ET_ATTRIB != null && responseObject.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT")
                        && !String.IsNullOrEmpty(responseObject.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL)
                        && responseObject.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Any(c => Char.IsDigit(c));

                    if (offer.OfferInternal.IsAccepted)
                    {
                        DateTime parsedAcceptedAt;

                        var acceptedAt = responseObject.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Trim();

                        if (DateTime.TryParseExact(acceptedAt, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedAcceptedAt))
                        {
                            offer.OfferInternal.AcceptedAt = parsedAcceptedAt.ToString("d.M.yyyy");
                        }
                        else
                        {
                            offer.OfferInternal.AcceptedAt = responseObject.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL;
                        }
                    }

                    offer.OfferInternal.HasGDPR =
                        responseObject.ET_ATTRIB != null && responseObject.ET_ATTRIB.Any(x => x.ATTRID == "KEY_GDPR")
                        && !String.IsNullOrEmpty(responseObject.ET_ATTRIB.First(x => x.ATTRID == "KEY_GDPR").ATTRVAL);


                    if (responseObject.ET_ATTRIB != null && responseObject.ET_ATTRIB.Any(x => x.ATTRID == "CREATED_AT") && !string.IsNullOrEmpty(responseObject.ET_ATTRIB.First(x => x.ATTRID == "CREATED_AT").ATTRVAL))
                    {
                        DateTime createdAt;

                        var created = responseObject.ET_ATTRIB.First(x => x.ATTRID == "CREATED_AT").ATTRVAL;

                        if (DateTime.TryParseExact(created, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out createdAt))
                        {
                            offer.OfferInternal.CreatedAt = createdAt.ToString("d.M.yyyy");
                        }
                        else
                        {
                            offer.OfferInternal.CreatedAt = created;
                        }
                    }

                    if (responseObject.ET_ATTRIB != null && responseObject.ET_ATTRIB.Any(x => x.ATTRID == "CAMPAIGN") && !string.IsNullOrEmpty(responseObject.ET_ATTRIB.First(x => x.ATTRID == "CAMPAIGN").ATTRVAL))
                    {
                        offer.OfferInternal.Body.Campaign = responseObject.ET_ATTRIB.First(x => x.ATTRID == "CAMPAIGN").ATTRVAL;
                    }

                    if (offer.OfferInternal.HasGDPR)
                    {
                        offer.OfferInternal.GDPRKey = responseObject.ET_ATTRIB.First(x => x.ATTRID == "KEY_GDPR").ATTRVAL;
                    }

                    offer.OfferInternal.State = responseObject.ES_HEADER.CCHSTAT;
                    offer.OfferInternal.IsAccepted = offer.OfferInternal.IsAccepted;

                    return offer;
                }
            }

            return null;
        }

        /// <summary>
        /// Generates xml for offer.
        /// </summary>
        /// <param name="guid">Uuid of offer.</param>
        /// <returns></returns>
        public Offer GenerateXml(string guid)
        {
            ZCCH_CACHE_GETResponse result = GetResponse(guid, "NABIDKA");

            var offer = GenerateXml(guid, result);

            return offer;
        }

        /// <summary>
        /// Implementation of accept offer process.
        /// </summary>
        /// <param name="guid">Uuid of offer.</param>
        /// <returns>Returns true if offer was succesfully sent to the SAP.</returns>
        public bool AcceptOffer(string guid)
        {
            AcceptedOffer offer = new AcceptedOffer()
            {
                Guid = guid,
                SentToService = false
            };

            var timestampString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                status.IV_CCHKEY = guid;
                status.IV_CCHTYPE = "NABIDKA";
                status.IV_STAT = "5";
                status.IV_TIMESTAMP = outValue;

                CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET> del = new CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET>(AcceptOfferDel);
                ZCCH_CACHE_STATUS_SETResponse response = null;
                Log.Debug(string.Format("[{0}] Calling service to set offer status to 'Accepted'.", guid), this);

                try
                {
                    
                    response = this.CallService(status, del);
                }
                catch (Exception ex)
                {
                    Log.Fatal($"[{guid}] Request ZCCH_CACHE_STATUS_SET failed.", ex, this);
                }

                if (response != null)
                {
                    if (response.EV_RETCODE == 0)
                    {
                        offer.SentToService = true;
                    }
                    else
                    {
                        Log.Error($"[{guid}] Call to the web service during Accepting returned result {response.EV_RETCODE}.", this);
                    }
                }
                else
                {
                    Log.Error($"[{guid}] Call to the web service during Accepting returned null result.", this);
                }
            }

            return offer.SentToService;
        }

        /// <summary>
        /// Tries to accept offer. Returns false, if some error occured and offer should not be accepted.
        /// </summary>
        /// <param name="guid">ID of the offer.</param>
        /// <param name="when">Date of acceptance.</param>
        /// <param name="context">Http context.</param>
        /// <param name="offerType">Offer type.</param>
        /// <param name="acceptedDocuments">Collection of accepted documents.</param>
        /// <returns>True if offer should be accepted. False if error occured.</returns>
        public bool LogAcceptance(string guid, DateTime when, HttpContextBase context, OfferTypes offerType, IEnumerable<string> acceptedDocuments)
        {
            StringBuilder startingLog = new StringBuilder();
            startingLog.AppendLine($"[{guid}][LogAcceptance] Initializing...");
            startingLog.AppendLine($" - Guid: {guid}");
            startingLog.AppendLine($" - when: {when.ToString("yyyy-MM-dd HH:mm:ss")}");
            Log.Debug(startingLog.ToString(), this);

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
                ATTRVAL = GetIpAddress()
            });

            var files = new List<ZCCH_ST_FILE>();

            Log.Debug($"[{guid}][LogAcceptance] Getting information about PDF files by type 'NABIDKA_PDF' ...", this);

            var responsePdfFiles = this.GetFiles(guid, false);
            var localFiles = context.Session["UserFiles"] as List<FileToBeDownloaded>;


            Log.Debug($"[{guid}][LogAcceptance] {responsePdfFiles.Length} PDF files received", this);

            if (offerType != OfferTypes.Default)
            {
                var signedLocalFiles = localFiles.Where(localFile => localFile.SignedVersion);
                var signedFilesCount = signedLocalFiles.Count();
                Log.Debug($"[{guid}][LogAcceptance] Signed files count:{signedFilesCount}", this);

                if (signedFilesCount > 0)
                {
                    var signedFilesCounter = 1;
                    var signedNames = signedLocalFiles.Select(a => string.Format("[{0}] Number:[{1}] Name:[{2}] Type:[{3}]", signedFilesCounter++, a.FileNumber, a.FileName, a.FileType));
                    var signedNamesLogEntry = string.Format("[{0}][LogAcceptance] Signed files:{1}{2}", guid, Environment.NewLine, string.Join(Environment.NewLine, signedNames));
                    Log.Debug(signedNamesLogEntry, this);
                }
                
                var signedFilesChecked = 0;

                foreach (var responseFile in responsePdfFiles)
                {
                    var existingLocalSignedVariant = signedLocalFiles.FirstOrDefault(signedLocalFile => signedLocalFile.FileNumber == responseFile.FILEINDX);

                    if (existingLocalSignedVariant != null)
                    {
                        responseFile.FILECONTENT = existingLocalSignedVariant.FileContent.ToArray();

                        var arccIdAttribute = responseFile.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "$TMP.ARCCID$");
                        var arcDocAttribute = responseFile.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "$TMP.ARCDOC$");
                        var arcArchIdAttribute = responseFile.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "$TMP.ARCHID$");

                        arccIdAttribute.ATTRVAL = arccIdAttribute != null ? string.Empty : arccIdAttribute.ATTRVAL;
                        arcDocAttribute.ATTRVAL = arcDocAttribute != null ? string.Empty : arcDocAttribute.ATTRVAL;
                        arcArchIdAttribute.ATTRVAL = arcArchIdAttribute != null ? string.Empty : arcArchIdAttribute.ATTRVAL;

                        files.Add(responseFile);
                        signedFilesChecked++;
                        Log.Debug($"[{guid}][LogAcceptance] File checked:{responseFile.FILENAME}", this);
                    }
                    else
                    {
                        if (acceptedDocuments.Contains(responseFile.FILEINDX))
                        {
                            responseFile.FILECONTENT = new byte[] { };
                            files.Add(responseFile);

                            Log.Debug($"[{guid}][LogAcceptance] PDF file received (untouched): '{responseFile.FILENAME}'", this);
                        }
                    }
                }

                if (signedFilesCount != signedFilesChecked)
                {
                    throw new ApplicationException("Number of loaded signed files ("+ signedFilesCount + ") and number of processed signed files ("+ signedFilesChecked + ") is not the same!");
                }
            }
            else
            {
                foreach (ZCCH_ST_FILE file in responsePdfFiles)
                {
                    if (file != null)
                    {
                        file.FILECONTENT = new byte[] { };
                        files.Add(file);

                        Log.Debug($"[{guid}][LogAcceptance] PDF file received: '{file.FILENAME}'", this);
                    }
                }
            }

            ZCCH_CACHE_PUT cachePut = new ZCCH_CACHE_PUT();
            cachePut.IV_CCHKEY = guid;
            cachePut.IV_CCHTYPE = "NABIDKA_PRIJ";
            cachePut.IT_ATTRIB = attributes.ToArray();
            cachePut.IT_FILES = files.ToArray();

            CallServiceMethod<ZCCH_CACHE_PUTResponse, ZCCH_CACHE_PUT> del = new CallServiceMethod<ZCCH_CACHE_PUTResponse, ZCCH_CACHE_PUT>((inputParam) =>
            {
                StringBuilder parameters = new StringBuilder();
                parameters.AppendLine($"[{guid}][LogAcceptance] Calling web service with parameter:");
                parameters.AppendFormat(" - IV_CCHKEY = {0}", inputParam.IV_CCHKEY);
                parameters.AppendLine();
                parameters.AppendFormat(" - IV_CCHTYPE = {0}", inputParam.IV_CCHTYPE);
                parameters.AppendLine();
                parameters.AppendFormat(" - IT_ATTRIB = ", string.Join(", ", inputParam.IT_ATTRIB.Select(x => x.ToString())));
                parameters.AppendLine();
                parameters.AppendFormat(" - IT_FILES = ", string.Join(", ", inputParam.IT_FILES.Select(x => x.ToString())));
                Log.Info(parameters.ToString(), this);

                using (var api = this.InitApi())
                {
                    return api.ZCCH_CACHE_PUT(inputParam);
                }
            });

            ZCCH_CACHE_PUTResponse response = null;
            var filesCount = cachePut.IT_FILES.Length;
            Log.Debug(string.Format("[{0}][LogAcceptance] Number of files to be pushed to ZCCH_CACHE_PUT:{1}", guid, filesCount), this);

            if (filesCount > 0)
            {
                var counter = 1;
                var filesToBePushed = cachePut.IT_FILES.Select(a => string.Format("[{0}] {1}", counter++, a.FILENAME));
                Log.Debug(string.Format(" Files to be pushed:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, filesToBePushed)), this);
            }

            try
            {
                response = this.CallService(cachePut, del);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[{guid}][LogAcceptance] Request ZCCH_CACHE_PUT failed.", ex, this);
            }

            if (response != null)
            {
                Log.Info($"[{guid}][LogAcceptance] Response: EV_RETCODE = {response.EV_RETCODE}", this);
                if (response.EV_RETCODE == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            else
            {
                Log.Debug($"[{guid}][LogAcceptance] Response is null", this);
                return false;
            }
        }

        /// <summary>
        /// Change the state of the offer to read.
        /// </summary>
        /// <param name="guid"></param>
        public void ReadOffer(string guid)
        {
            var timestampString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                status.IV_CCHKEY = guid;
                status.IV_CCHTYPE = "NABIDKA";
                status.IV_STAT = "4";
                status.IV_TIMESTAMP = outValue;

                CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET> del = new CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET>(AcceptOfferDel);

                try
                {
                    ZCCH_CACHE_STATUS_SETResponse response = this.CallService(status, del);
                }
                catch (Exception ex)
                {
                    Log.Fatal($"[{guid}] Request ZCCH_CACHE_STATUS_SET failed", ex, this);
                }
            }
        }

        /// <summary>
        /// Change the state of the offer to signed.
        /// </summary>
        /// <param name="guid"></param>
        public void SignOffer(string guid)
        {
            var timestampString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                status.IV_CCHKEY = guid;
                status.IV_CCHTYPE = "NABIDKA";
                status.IV_STAT = "6";
                status.IV_TIMESTAMP = outValue;

                try
                {
                    Log.Debug(string.Format("[{0}] Calling Sign Service.", guid), this);
                    CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET> del = new CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET>(AcceptOfferDel);
                    ZCCH_CACHE_STATUS_SETResponse response = this.CallService(status, del);
                }
                catch (Exception ex)
                {
                    Log.Fatal($"[{guid}] Request ZCCH_CACHE_STATUS_SET failed", ex, this);
                }
            }
        }

        /// <summary>
        /// Resets the offer.
        /// </summary>
        /// <param name="guid"></param>
        public void ResetOffer(string guid)
        {
            var timestampString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                status.IV_CCHKEY = guid;
                status.IV_CCHTYPE = "NABIDKA";
                status.IV_STAT = "1";
                status.IV_TIMESTAMP = outValue;

                CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET> del = new CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET>(AcceptOfferDel);

                try
                {
                    ZCCH_CACHE_STATUS_SETResponse response = this.CallService(status, del);
                }
                catch (Exception ex)
                {
                    Log.Fatal($"[{guid}] Request ZCCH_CACHE_STATUS_SET failed", ex, this);
                }
            }
        }

        /// <summary>
        /// Gets cxmlfor letter.
        /// </summary>
        /// <param name="texts">Enumeration of texts.</param>
        /// <returns>Retruns first text or nul lif there is no text available.</returns>
        public XmlText GetLetterXml(IEnumerable<XmlText> texts)
        {
            return texts.FirstOrDefault();
        }

        [Obsolete("Use GetAllAttributes(IEnumerable<XmlText> sources, IEnumerable<string> templateValues) instead.", false)]
        public Dictionary<string, string> GetAllAttributes(string guid, IEnumerable<XmlText> sources, string additionalInfoDocument)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (XmlText source in sources)
            {
                if (source.Attributes.ContainsKey("TEMPLATE"))
                {
                    if (source.Attributes["TEMPLATE"] == "EED" || source.Attributes["TEMPLATE"] == "EPD" || source.Attributes["TEMPLATE"] == additionalInfoDocument)
                    {
                        //Log.Debug("File used to get paramaters: " + source.NA)
                        var parameters = this.GetAllAttributes(guid, source);

                        foreach (var param in parameters)
                        {
                            if (result.ContainsKey(param.Key))
                            {
                                var existingValue = result[param.Key];
                                var newValue = param.Value;

                                if (existingValue != newValue)
                                {
                                    Log.Debug($"[{guid}] Overwriting parameter '{param.Key}' with value '{existingValue}' to new value '{newValue}'", this);
                                    result[param.Key] = param.Value;
                                }
                            }
                            else
                            {
                                result.Add(param.Key, param.Value);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all attributes from <paramref name="sources"/> which attribute "TEMPLATE" has value in <paramref name="templateValues"/>, for example "EED", "EPD" ...
        /// </summary>
        /// <param name="guid">The guid (just for logging).</param>
        /// <param name="sources">The sources.</param>
        /// <param name="templateValues">The template values, for example ["EED", "EPD"].</param>
        /// <returns>Dictionary with parameters from <paramref name="sources"/> or empty dictionary.</returns>
        public Dictionary<string, string> GetAllAttributes(string guid, IEnumerable<XmlText> sources, IEnumerable<RweClientLoadTemplateModel> templateValues)
        {
            var result = new Dictionary<string, string>();
            var enableOverwrite = true;

            foreach (XmlText source in sources)
            {
                if (source.Attributes.ContainsKey("TEMPLATE"))
                {
                    var matchFound = templateValues.FirstOrDefault(x => x.Identifier == source.Attributes["TEMPLATE"]);

                    if (matchFound != null)
                    {
                        Log.Debug($"[{guid}] Source found by TEMPLATE attribute: " + matchFound.Identifier, this);
                        var parameters = this.GetAllAttributes(guid, source);
                        var overwriteCandidates = parameters.Where(a => result.ContainsKey(a.Key));

                        if (enableOverwrite)
                        {
                            foreach (var attr in overwriteCandidates)
                            {
                                if (result[attr.Key] != attr.Value)
                                {
                                    Log.Debug($"[{guid}] Overwriting parameter '{attr.Key}' with value '{result[attr.Key]}' to new value '{attr.Value}'", this);
                                    result[attr.Key] = attr.Value;
                                }
                            }

                            ////Check if overwrite has to be disabled.
                            ////There is no way to enable it again, once it is disabled.
                            if (matchFound.StopWhenFound)
                            {
                                enableOverwrite = false;
                            }
                        }

                        foreach (var newAttr in parameters.Except(overwriteCandidates))
                        {
                            result.Add(newAttr.Key, newAttr.Value);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all attribtues for xml document.
        /// </summary>
        /// <param name="sourceXml"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllAttributes(string guid, XmlText sourceXml)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sourceXml.Text);

            var parameters = doc.DocumentElement.SelectSingleNode("parameters").ChildNodes;

            if (parameters == null)
            {
                return result;
            }

            foreach (XmlNode param in parameters)
            {
                if (result.ContainsKey(param.Name))
                {
                    var indexedValue = result[param.Name];
                    var currentValue = param.InnerXml;

                    if (indexedValue != currentValue)
                    {
                        Log.Warn($"[{guid}] XML contains duplicate node '<parameters><{param.Name}>'. Already indexed value: '{indexedValue}', current value: '{currentValue}'", this);
                    }
                }
                else
                {
                    result.Add(param.Name, param.InnerXml);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all xml texts.
        /// </summary>
        /// <param name="guid">Uuid of offer.</param>
        /// <returns></returns>
        public IEnumerable<XmlText> GetTextsXml(string guid)
        {
            ZCCH_CACHE_GETResponse result = GetResponse(guid, "NABIDKA_XML");

            List<XmlText> fileResults = new List<XmlText>();

            if (result.ThereAreFiles())
            {
                int index = 0;

                foreach (var f in result.ET_FILES)
                {
                    Log.Debug($"[{guid}] NABIDKA_XML response: Contains ST_FILE - {f.FILENAME}", this);
                    XmlText tempItem = new XmlText();
                    tempItem.Name = f.FILENAME;
                    tempItem.Index = (++index).ToString();
                    tempItem.Text = Encoding.UTF8.GetString(f.FILECONTENT);

                    if (f.ATTRIB != null)
                    {
                        foreach (var attr in f.ATTRIB)
                        {
                            tempItem.Attributes.Add(attr.ATTRID, attr.ATTRVAL);
                        }
                    }

                    fileResults.Add(tempItem);
                }
            }
            else
            {
                Log.Debug($"[{guid}] NABIDKA_XML response: No files (ET_FILES) found", this);
            }

            return fileResults;
        }

        #endregion

        public static string GetIpAddress()
        {
            string text = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return string.IsNullOrEmpty(text) ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : text.Split(',')[0];
        }

        #region Private method
        private ZCCH_CACHE_API InitApi()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ZCCH_CACHE_API api = new ZCCH_CACHE_API();

            SiteRootModel siteSettings = ConfigHelpers.GetSiteSettings();
            var userName = Encoding.UTF8.GetString(Convert.FromBase64String(siteSettings.ServiceUser));
            var password = Encoding.UTF8.GetString(Convert.FromBase64String(siteSettings.ServicePassword));
            api.Url = siteSettings.ServiceUrl;

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new InvalidCredentialException("Wrong credentials for cache!");
            }

            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(
                new Uri(api.Url),
                "Basic",
                new NetworkCredential(userName, password)
            );

            api.Credentials = credentialCache;
            return api;
        }

        private ZCCH_CACHE_GETResponse GetResponseDel(ZCCH_CACHE_GET inputParam)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.AppendLine($"[{inputParam.IV_CCHKEY}] Calling web service with parameters ");
            parameters.AppendFormat("IV_CCHKEY = {0}", inputParam.IV_CCHKEY);
            parameters.AppendLine();
            parameters.AppendFormat("IV_CCHTYPE = {0}", inputParam.IV_CCHTYPE);
            parameters.AppendLine();
            parameters.AppendFormat("IV_GEFILE = ", inputParam.IV_CCHTYPE);
            Log.Debug(parameters.ToString(), this);

            using (var api = InitApi())
            {
                ZCCH_CACHE_GETResponse result = api.ZCCH_CACHE_GET(inputParam);
                return result;
            }
        }
        private ZCCH_CACHE_GETResponse GetResponse(string guid, string type)
        {
            ZCCH_CACHE_GET inputPar = new ZCCH_CACHE_GET();
            inputPar.IV_CCHKEY = guid;
            inputPar.IV_CCHTYPE = type;
            inputPar.IV_GEFILE = "B";

            CallServiceMethod<ZCCH_CACHE_GETResponse, ZCCH_CACHE_GET> del = new CallServiceMethod<ZCCH_CACHE_GETResponse, ZCCH_CACHE_GET>(GetResponseDel);

            try
            {
                return this.CallService(inputPar, del);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[{guid}] Request ZCCH_CACHE_GET failed", ex, this);
            }

            return default(ZCCH_CACHE_GETResponse);
        }

        private ZCCH_CACHE_STATUS_SETResponse AcceptOfferDel(ZCCH_CACHE_STATUS_SET inputParam)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.AppendLine($"[{inputParam.IV_CCHKEY}] Calling web service with parameters ");
            parameters.AppendFormat("IV_CCHKEY = {0}", inputParam.IV_CCHKEY);
            parameters.AppendLine();
            parameters.AppendFormat("IV_CCHTYPE = {0}", inputParam.IV_CCHTYPE);
            parameters.AppendLine();
            parameters.AppendFormat("IV_STAT = ", inputParam.IV_STAT);
            parameters.AppendLine();
            parameters.AppendFormat("IV_TIMESTAMP = ", inputParam.IV_TIMESTAMP);
            Log.Info(parameters.ToString(), this);

            using (var api = InitApi())
            {
                var response = api.ZCCH_CACHE_STATUS_SET(inputParam);
                return response;
            }
        }

        private T CallService<T, P>(P param, CallServiceMethod<T, P> del)
        {
            T result;
            for (int callCount = 1; callCount <= 15; callCount++)
            {
                try
                {
                    result = del(param);
                    return result;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(500);
                    throw;
                }
            }
            return default(T);
        }

        #endregion


    }
}
