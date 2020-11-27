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
        /// Initializes a new instance of the <see cref="OfferParserService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">logger</exception>
        public OfferParserService(ILogger logger)
        {
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
        public IDictionary<string, string> GetTextParameters(ZCCH_ST_FILE[] files)
        {
            var parameters = new Dictionary<string, string>();

            for (int i = 0; i < files.Length; i++)
            {
                var xml = Encoding.UTF8.GetString(files[i].FILECONTENT);
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
            var result = this.ProcessRootFile(file);
            var rawXml = Utils.GetRawXml(file);
            var version = this.GetVersion(response.Response);
            var isAccepted = this.IsAccepted(response.Response);
            var offer = new OfferModel(result, version, header, isAccepted, attributes);
            offer.RawContent.Add(file.FILENAME, rawXml);
            return offer;
        }

        /// <summary>
        /// Gets the core file of the offer.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The file.</returns>
        /// <exception cref="System.NotSupportedException">Unknow offer version ({version})</exception>
        protected internal ZCCH_ST_FILE GetCoreFile(ZCCH_CACHE_GETResponse response)
        {
            if (response.ET_FILES.Length == 1)
            {
                return response.ET_FILES[0];
            }

            var version = this.GetVersion(response);

            if (version == 1)
            {
                return response.ET_FILES[0];
            }
            else if (version == 2)
            {
                return response.ET_FILES.FirstOrDefault(x => !x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE && a.ATTRVAL == Constants.FileAttributeValues.TEXT_PARAMETERS));
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
        /// <returns>Tuple with model and raw XML content.</returns>
        [ExcludeFromCodeCoverage]
        protected internal OfferXmlModel ProcessRootFile(ZCCH_ST_FILE file)
        {
            using (var stream = new MemoryStream(file.FILECONTENT, false))
            {
                var serializer = new XmlSerializer(typeof(OfferXmlModel), "http://www.sap.com/abapxml");
                var offerXml = (OfferXmlModel)serializer.Deserialize(stream);

                this.MakeCompatible(offerXml);

                return offerXml;
            }
        }

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
        /// Set missing required values in <paramref name="offerXml"/>.
        /// </summary>
        /// <remarks>
        /// <para>When missing bus process value, set default '<see cref="Constants.OfferDefaults.BUS_PROCESS"/>'.</para>
        /// <para>When missing bus process type, first checks if campaign is empty, if yes, set '<see cref="Constants.OfferDefaults.BUS_PROCESS_TYPE_A"/>', otherwise set '<see cref="Constants.OfferDefaults.BUS_PROCESS_TYPE_B"/>'.</para>
        /// </remarks>
        /// <param name="offerXml">The offer XML.</param>
        protected internal void MakeCompatible(OfferXmlModel offerXml)
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
                var xml = xmlNode.FirstChild.InnerXml;
                // "<p style=\"margin-top:0pt;margin-bottom:0pt\" xmlns=\"http://www.w3.org/1999/xhtml\">\nInvestor<br /></p>"
                var clean = Utils.ReplaceXmlAttributes(xml);
                return clean;
            }
            else
            {
                return xmlNode.InnerXml;
            }
        }
    }
}
