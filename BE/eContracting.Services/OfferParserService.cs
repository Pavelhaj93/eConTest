﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using eContracting.Models;

namespace eContracting.Services
{
    /// <inheritdoc/>
    public class OfferParserService : IOfferParserService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The settings reader.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferParserService"/> class.
        /// </summary>
        /// <param name="settingsReader">The settings reader.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">
        /// settingsReader
        /// or
        /// logger
        /// </exception>
        public OfferParserService(ISettingsReaderService settingsReader, ILogger logger)
        {
            this.SettingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public OfferModel GenerateOffer(ResponseCacheGetModel response)
        {
            if (!response.HasFiles)
            {
                this.Logger.Info(response.Guid, "No files in a response. Returns null offer.");
                return null;
            }

            var offerModel = this.ProcessResponse(response);
            return offerModel;
        }

        /// <inheritdoc/>
        public int GetVersion(ZCCH_CACHE_GETResponse response)
        {
            var attr = response.ET_ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.OfferAttributes.VERSION);

            if (attr == null)
            {
                this.Logger.Info(response.ES_HEADER.CCHKEY, $"Version 1 (attribute {Constants.OfferAttributes.VERSION} not found)");
                return 1;
            }

            if (attr.ATTRVAL == Constants.OfferAttributeValues.VERSION_2)
            {
                this.Logger.Info(response.ES_HEADER.CCHKEY, $"Version 2 (attribute {Constants.OfferAttributes.VERSION} = {Constants.OfferAttributeValues.VERSION_2})");
                return 2;
            }

            if (attr.ATTRVAL == Constants.OfferAttributeValues.VERSION_3)
            {
                this.Logger.Info(response.ES_HEADER.CCHKEY, $"Version 3 (attribute {Constants.OfferAttributes.VERSION} = {Constants.OfferAttributeValues.VERSION_3})");
                return 3;
            }

            throw new EcontractingApplicationException(new ErrorModel("OP_GV_X", "Cannot determinate offer version"));
        }

        /// <inheritdoc/>
        public IDictionary<string, string> GetTextParameters(OfferFileXmlModel[] files)
        {
            var parameters = new Dictionary<string, string>();

            for (int i = 0; i < files.Length; i++)
            {
                var xml = Encoding.UTF8.GetString(files[i].File.FILECONTENT);
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                var xmlParameters = doc.SelectSingleNode("form/parameters")?.ChildNodes;

                if (xmlParameters != null)
                {
                    foreach (XmlNode xmlNode in xmlParameters)
                    {
                        var key = xmlNode.Name;
                        var value = this.GetNodeValue(xmlNode);

                        if (parameters.ContainsKey(key))
                        {
                            var indexedValue = parameters[key];
                            var currentValue = value;

                            if (indexedValue != currentValue)
                            {
                                if (string.IsNullOrEmpty(indexedValue))
                                {
                                    parameters[key] = value;
                                }
                            }
                        }
                        else
                        {
                            parameters[key] = value;
                        }
                    }
                }
            }

            return parameters;
        }

        /// <inheritdoc/>
        public void MakeCompatibleParameters(IDictionary<string, string> textParameters, int version)
        {
            if (version < 2)
            {
                textParameters["COMMODITY"] = "X";
            }

            var keys = this.SettingsReader.GetBackCompatibleTextParametersKeys(version);

            for (int i = 0; i < keys.Length; i++)
            {
                var newKey = keys[i].Key;
                var oldKey = keys[i].Value;

                if (textParameters.ContainsKey(newKey) && !textParameters.ContainsKey(oldKey))
                {
                    textParameters[oldKey] = textParameters[newKey];
                }
            }

            // this is backup variant when PERSON_PREMADR is empty because it's used in rich text
            if (!textParameters.HasValue("PERSON_PREMADR"))
            {
                var address = new StringBuilder();

                if (textParameters.HasValue("PERSON_PREMSTREET"))
                {
                    address.Append(textParameters.GetValueOrDefault("PERSON_PREMSTREET"));
                }
                else
                {
                    address.Append(textParameters.GetValueOrDefault("PERSON_PREMCITY_PART"));
                }

                address.Append(" ");
                address.Append(textParameters.GetValueOrDefault("PERSON_PREMSTREET_NUMBER"));
                address.Append(", ");
                address.Append(textParameters.GetValueOrDefault("PERSON_PREMCITY"));
                address.Append(", ");
                address.Append(textParameters.GetValueOrDefault("PERSON_PREMPOSTAL_CODE"));

                textParameters["PERSON_PREMADR"] = address.ToString();
            }
        }

        /// <summary>
        /// Gets the header from <see cref="ZCCH_CACHE_GETResponse.ES_HEADER"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        protected internal OfferHeaderModel GetHeader(ZCCH_CACHE_GETResponse response)
        {
            return new OfferHeaderModel(response.ES_HEADER);
        }

        /// <summary>
        /// Gets the attributes from <see cref="ZCCH_CACHE_GETResponse.ET_ATTRIB"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Array of attributes.</returns>
        protected internal OfferAttributeModel[] GetAttributes(ZCCH_CACHE_GETResponse response)
        {
            var list = new List<OfferAttributeModel>();

            for (int i = 0; i < response.ET_ATTRIB.Length; i++)
            {
                var attrib = response.ET_ATTRIB[i];
                list.Add(new OfferAttributeModel(attrib));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Processes the response and create <see cref="OfferModel"/> or root XML exists.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Model or null.</returns>
        protected internal OfferModel ProcessResponse(ResponseCacheGetModel response)
        {
            var version = this.GetVersion(response.Response);
            var file = this.GetCoreFile(response.Response, version);

            if (file == null)
            {
                throw new ApplicationException("Valid offer not found in the response");
            }

            var header = this.GetHeader(response.Response);
            var attributes = this.GetAttributes(response.Response);
            var rawXml = file.GetRawXml();
            var result = this.ProcessRootFile(response.Guid, file, version);
            var isAccepted = this.IsAccepted(response.Response);
            var expirationDate = this.GetExpirationDate(response.Response, result);
            var isExpired = this.IsExpired(response.Response, header, expirationDate);
            var offer = new OfferModel(result, version, header, isAccepted, isExpired, expirationDate, attributes);
            offer.RawContent.Add(file.File.FILENAME, rawXml);            
            this.Logger.Info(offer.Guid, $"Process: {offer.Process}");
            this.Logger.Info(offer.Guid, $"Process type: {offer.ProcessType}");
            this.Logger.Debug(offer.Guid, this.GetLogMessage(attributes));
            return offer;
        }

        /// <summary>
        /// Gets the core file of the offer.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="version">The offer version.</param>
        /// <returns>The file.</returns>
        /// <exception cref="System.NotSupportedException">Unknow offer version ({version})</exception>
        protected internal OfferFileXmlModel GetCoreFile(ZCCH_CACHE_GETResponse response, int version)
        {
            if (response.ET_FILES.Length == 1)
            {
                var file = response.ET_FILES[0];
                return new OfferFileXmlModel(file);
            }

            if (version == 1)
            {
                var file = response.ET_FILES.FirstOrDefault(x => !x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE));

                if (file != null)
                {
                    return new OfferFileXmlModel(file);
                }

                return null;
            }
            else if (version == 2)
            {
                var file = response.ET_FILES.FirstOrDefault(x => !x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE && a.ATTRVAL == Constants.FileAttributeValues.TEXT_PARAMETERS));

                if (file != null)
                {
                    return new OfferFileXmlModel(file);
                }

                return null;
            }
            else if (version == 3)
            {
                var file = response.ET_FILES.FirstOrDefault(x => !x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE));

                if (file != null)
                {
                    return new OfferFileXmlModel(file);
                }

                return null;
            }
            else
            {
                throw new NotSupportedException($"No core file found. Unknow offer version {version}");
            }
        }

        /// <summary>
        /// Processes the root file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="version">Offer version.</param>
        /// <returns>Tuple with model and raw XML content.</returns>
        [ExcludeFromCodeCoverage]
        protected internal OfferXmlModel ProcessRootFile(string guid, OfferFileXmlModel file, int version)
        {
            using (var stream = new MemoryStream(file.File.FILECONTENT, false))
            {
                var serializer = new XmlSerializer(typeof(OfferXmlModel), "http://www.sap.com/abapxml");
                var offerXml = (OfferXmlModel)serializer.Deserialize(stream);
                this.MakeCompatible(guid, offerXml, version);
                //this.AddCustomUploadModelWhenNecessary(offerXml);
                return offerXml;
            }
        }

        /// <summary>
        /// Determines whether an offer from <paramref name="response"/> is accepted.
        /// </summary>
        /// <param name="response">The response.</param>
        protected internal bool IsAccepted(ZCCH_CACHE_GETResponse response)
        {
            var attr = response.ET_ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.OfferAttributes.ACCEPTED_DATE)?.ATTRVAL;

            if (attr == null)
            {
                this.Logger.Info(response.ES_HEADER.CCHKEY, $"Offer is not accepted due to missing attribute {Constants.OfferAttributes.ACCEPTED_DATE}");
                return false;
            }

            var result = attr.Any(c => Char.IsDigit(c));

            if (result)
            {
                this.Logger.Info(response.ES_HEADER.CCHKEY, $"Offer is accepted ({Constants.OfferAttributes.ACCEPTED_DATE} = {attr})");
            }
            else
            {
                this.Logger.Info(response.ES_HEADER.CCHKEY, $"Offer is not accepted ({Constants.OfferAttributes.ACCEPTED_DATE} = {attr})");
            }

            return result;
        }

        /// <summary>
        /// Determines whether an offer from <paramref name="response"/> is expired.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="headerModel">The header model.</param>
        /// <param name="expirationDate">The expiration date.</param>
        protected internal bool IsExpired(ZCCH_CACHE_GETResponse response, OfferHeaderModel headerModel, DateTime expirationDate)
        {
            if (headerModel.CCHSTAT == "9")
            {
                this.Logger.Debug(response.ES_HEADER.CCHKEY, "Offer is expired (CCHSTAT = 9)");
                return true;
            }

            var result = expirationDate.Date < DateTime.Now.Date;

            if (result)
            {
                this.Logger.Debug(response.ES_HEADER.CCHKEY, $"Offer is expired (date = {expirationDate.Date.ToString("dd. MM. yyyy")})");
            }
            else
            {
                this.Logger.Debug(response.ES_HEADER.CCHKEY, $"Offer is not expired (date = {expirationDate.Date.ToString("dd. MM. yyyy")})");
            }

            return result;
        }

        /// <summary>
        /// Gets expiration date from <paramref name="response"/> or from <paramref name="offerXmlModel"/>.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="offerXmlModel"></param>
        /// <returns>If date not found, returns '<c>DateTime.Now.AddDays(-1)</c>' as default.</returns>
        protected internal DateTime GetExpirationDate(ZCCH_CACHE_GETResponse response, OfferXmlModel offerXmlModel)
        {
            DateTime date = DateTime.Now.AddDays(-1);

            var value = response.ET_ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.OfferAttributes.VALID_TO)?.ATTRVAL;

            if (string.IsNullOrEmpty(value))
            {
                value = offerXmlModel.Content.Body.DATE_TO;
                this.Logger.Debug(response.ES_HEADER.CCHKEY, $"Attribute {Constants.OfferAttributes.VALID_TO} not found in offer. Using DATE_TO from body instead.");
            }

            if (!DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                this.Logger.Error(response.ES_HEADER.CCHKEY, $"Cannot parse expiration date from '{value}'");
            }

            return date;
        }

        /// <summary>
        /// Set missing required values in <paramref name="offerXml"/>.
        /// </summary>
        /// <remarks>
        /// <para>When missing bus process value, set default '<see cref="Constants.OfferDefaults.BUS_PROCESS"/>'.</para>
        /// <para>When missing bus process type, first checks if campaign is empty, if yes, set '<see cref="Constants.OfferDefaults.BUS_PROCESS_TYPE_A"/>', otherwise set '<see cref="Constants.OfferDefaults.BUS_PROCESS_TYPE_B"/>'.</para>
        /// </remarks>
        /// <param name="offerXml">The offer XML.</param>
        /// <param name="version">Offer version.</param>
        protected internal void MakeCompatible(string guid, OfferXmlModel offerXml, int version)
        {
            if (string.IsNullOrWhiteSpace(offerXml.Content.Body.BusProcess))
            {
                offerXml.Content.Body.BusProcess = Constants.OfferDefaults.BUS_PROCESS;
                this.Logger.Info(offerXml.Content.Body.Guid, $"Missing value for 'BusProcess'. Set default: '{Constants.OfferDefaults.BUS_PROCESS}'");
            }

            if (string.IsNullOrWhiteSpace(offerXml.Content.Body.BusProcessType))
            {
                if (string.IsNullOrWhiteSpace(offerXml.Content.Body.Campaign))
                {
                    offerXml.Content.Body.BusProcessType = Constants.OfferDefaults.BUS_PROCESS_TYPE_A;
                    this.Logger.Info(offerXml.Content.Body.Guid, $"Missing value for 'BusProcessType'. Set default: '{Constants.OfferDefaults.BUS_PROCESS_TYPE_A}'");
                }
                else
                {
                    offerXml.Content.Body.BusProcessType = Constants.OfferDefaults.BUS_PROCESS_TYPE_B;
                    this.Logger.Info(offerXml.Content.Body.Guid, $"Missing value for 'BusProcessType'. Set default: '{Constants.OfferDefaults.BUS_PROCESS_TYPE_B}'");
                }
            }

            if (version == 1 && offerXml.Content.Body.Attachments != null)
            {
                var list = new List<OfferAttachmentXmlModel>();

                for (int i = 0; i < offerXml.Content.Body.Attachments.Length; i++)
                {
                    var attachment = offerXml.Content.Body.Attachments[i];
                    attachment.OfferGuid = guid;

                    // exclude AD1 template from the collection because it's prescription for file text parameters and but no real file exist for it
                    if (attachment.IdAttach != Constants.FileAttributeValues.TEXT_PARAMETERS)
                    {
                        list.Add(attachment);
                    }
                }

                offerXml.Content.Body.Attachments = list.ToArray();
            }
        }

        /// <summary>
        /// Adds the custom upload model when any other upload is required.
        /// </summary>
        /// <param name="offerXml">The offer XML.</param>
        protected void AddCustomUploadModelWhenNecessary(OfferXmlModel offerXml)
        {
            if (offerXml.Content.Body.Attachments.Any(x => x.IsPrinted() == false && x.IsObligatory() == true))
            {
                var list = new List<OfferAttachmentXmlModel>(offerXml.Content.Body.Attachments);
                list.Add(this.GetCustomUploadModel());
                offerXml.Content.Body.Attachments = list.ToArray();
            }
        }

        /// <summary>
        /// Gets InnerXml from <paramref name="xmlNode"/>. If <paramref name="xmlNode"/> contains "&lt;body&gt;", it returns inner XML of it.
        /// </summary>
        /// <remarks>Some nodes can contains inner XML with '&lt;body xmlns="http://www.w3.org/1999/xhtml"&gt;'.</remarks>
        /// <param name="xmlNode">The XML node.</param>
        /// <returns>Inner xml value of <paramref name="xmlNode"/>.</returns>
        protected internal string GetNodeValue(XmlNode xmlNode)
        {
            if (xmlNode.FirstChild?.Name == "body")
            {
                return this.GetCleanedUpBodyValue(xmlNode)?.Trim();
            }
            else
            {
                return this.GetXmlNodeValue(xmlNode)?.Trim();
            }
        }

        /// <summary>
        /// Gets <paramref name="xmlNode"/> value with extra, non wanted, data.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        protected internal string GetCleanedUpBodyValue(XmlNode xmlNode)
        {
            // this is label for dialog accept dialog window and it should be without any HTML
            if (xmlNode.Name.Contains("_ACCEPT_LABEL"))
            {
                return xmlNode.InnerText?.Trim();
            }

            var names = this.SettingsReader.GetXmlNodeNamesExcludeHtml();

            if (names.Contains(xmlNode.Name))
            {
                return xmlNode.InnerText?.Trim();
            }

            // do this to check if there is empty HTML (do not return something like '<p></p>')
            if (string.IsNullOrEmpty(xmlNode.FirstChild.InnerText.Trim()))
            {
                return string.Empty;
            }

            var xml = xmlNode.FirstChild.InnerXml;
            // remove style and xmlns attributes, e.g. "<p style=\"margin-top:0pt;margin-bottom:0pt\" xmlns=\"http://www.w3.org/1999/xhtml\">\nInvestor<br /></p>"
            var clean = Utils.ReplaceXmlAttributes(xml);
            return clean;
        }

        /// <summary>
        /// Gets <see cref="XmlNode.InnerText"/> or <see cref="XmlNode.InnerXml"/> based on 'this.SettingsReader.GetXmlNodeNamesExcludeHtml()'.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <returns>Inner text or inner xml.</returns>
        protected internal string GetXmlNodeValue(XmlNode xmlNode)
        {
            var names = this.SettingsReader.GetXmlNodeNamesExcludeHtml();

            if (names.Contains(xmlNode.Name))
            {
                return xmlNode.InnerText;
            }

            // do this to check if there is empty HTML (do not return something like '<p></p>')
            if (string.IsNullOrEmpty(xmlNode.InnerText.Trim()))
            {
                return string.Empty;
            }

            var innerXml = xmlNode.InnerXml;

            if (innerXml.Contains("&amp;"))
            {
                // value is 2x encoded
                innerXml = HttpUtility.HtmlDecode(innerXml);
            }

            innerXml = HttpUtility.HtmlDecode(innerXml);

            return innerXml;
        }

        /// <summary>
        /// Creates model for custom, not mandatory, upload.
        /// </summary>
        /// <returns>The template model.</returns>
        protected internal OfferAttachmentXmlModel GetCustomUploadModel()
        {
            var template = new OfferAttachmentXmlModel();
            template.AddInfo = string.Empty;
            template.Description = "Custom upload"; //TODO: Add label from Sitecore
            template.Group = "COMMODITY";
            template.IdAttach = "CUSTOM UPLOAD";
            template.ItemGuid = "11111111111111111111111111111111";
            template.SequenceNumber = "999";
            return template;
        }

        protected internal string GetLogMessage(OfferAttributeModel[] attributes)
        {
            var log = new StringBuilder();

            if (attributes?.Length > 0)
            {
                log.AppendLine("Attributes:");

                for (int i = 0; i < attributes.Length; i++)
                {
                    log.AppendLine($" - {attributes[i].Key}: {attributes[i].Value}");
                }
            }

            return log.ToString();
        }
    }
}
