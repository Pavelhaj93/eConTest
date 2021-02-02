using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                return 1;
            }

            if (attr.ATTRVAL == Constants.OfferAttributeValues.VERSION_2)
            {
                return 2;
            }

            throw new ApplicationException("Cannot determinate offer version");
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
            var file = this.GetCoreFile(response.Response);

            if (file == null)
            {
                throw new ApplicationException("Valid offer not found in the response");
            }

            var header = this.GetHeader(response.Response);
            var attributes = this.GetAttributes(response.Response);
            var rawXml = file.GetRawXml();
            var version = this.GetVersion(response.Response);
            var result = this.ProcessRootFile(file, version);
            var isAccepted = this.IsAccepted(response.Response);
            var isExpired = this.IsExpired(response.Response, header, result);
            var offer = new OfferModel(result, version, header, isAccepted, isExpired, attributes);
            offer.RawContent.Add(file.File.FILENAME, rawXml);
            return offer;
        }

        /// <summary>
        /// Gets the core file of the offer.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The file.</returns>
        /// <exception cref="System.NotSupportedException">Unknow offer version ({version})</exception>
        protected internal OfferFileXmlModel GetCoreFile(ZCCH_CACHE_GETResponse response)
        {
            if (response.ET_FILES.Length == 1)
            {
                var file = response.ET_FILES[0];
                return new OfferFileXmlModel(file);
            }

            var version = this.GetVersion(response);

            if (version == 1)
            {
                var file = response.ET_FILES[0];
                return new OfferFileXmlModel(file);
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
        protected internal OfferXmlModel ProcessRootFile(OfferFileXmlModel file, int version)
        {
            using (var stream = new MemoryStream(file.File.FILECONTENT, false))
            {
                var serializer = new XmlSerializer(typeof(OfferXmlModel), "http://www.sap.com/abapxml");
                var offerXml = (OfferXmlModel)serializer.Deserialize(stream);
                this.MakeCompatible(offerXml, version);
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
                return false;
            }

            return attr.Any(c => Char.IsDigit(c));
        }

        /// <summary>
        /// Determines whether an offer from <paramref name="response"/> is expired.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="headerModel">The header model.</param>
        /// <param name="offerXmlModel">The offer XML model.</param>
        protected internal bool IsExpired(ZCCH_CACHE_GETResponse response, OfferHeaderModel headerModel, OfferXmlModel offerXmlModel)
        {
            if (headerModel.CCHSTAT == "9")
            {
                this.Logger.Debug(response.ES_HEADER.CCHKEY, "Offer is expired (CCHSTAT = 9)");
                return true;
            }

            DateTime date = DateTime.Now.AddDays(-1);

            var value = response.ET_ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.OfferAttributes.VALID_TO)?.ATTRVAL;

            if (string.IsNullOrEmpty(value))
            {
                value = offerXmlModel.Content.Body.DATE_TO;
                this.Logger.Debug(response.ES_HEADER.CCHKEY, $"Attribute {Constants.OfferAttributes.VALID_TO} not found in offer. Using DATE_TO from body instead.");
            }

            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date.Date < DateTime.Now.Date;
            }
            else
            {
                this.Logger.Warn(response.ES_HEADER.CCHKEY, $"Cannot parse expire date ({value}). Offer set to NOT expired.");
                return false;
            }
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
        protected internal void MakeCompatible(OfferXmlModel offerXml, int version)
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
                    var d = offerXml.Content.Body.Attachments[i];

                    // exclude AD1 template from the collection because it's prescription for file text parameters and but no real file exist for it
                    if (d.IdAttach != Constants.FileAttributeValues.TEXT_PARAMETERS)
                    {
                        list.Add(d);
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

            return xmlNode.InnerXml;
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
    }
}
