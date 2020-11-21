using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

            return 1;
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
                var xmlParameters = doc.SelectSingleNode("form/parameters").ChildNodes;

                if (xmlParameters != null)
                {
                    foreach (XmlNode xmlNode in xmlParameters)
                    {
                        var key = xmlNode.Name;
                        var value = xmlNode.InnerXml;

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
                return response.ET_FILES.FirstOrDefault(x => !x.ATTRIB.Any(a => a.ATTRID == Constants.FileAttributes.TYPE && a.ATTRVAL == "AD1"));
            }
            else
            {
                throw new NotSupportedException($"No core file found. Unknow offer version {version}");
            }
        }

        [Obsolete]
        protected internal (ZCCH_ST_FILE root, ZCCH_ST_FILE ad1) GetSeparatedFiles(ResponseCacheGetModel response)
        {
            ZCCH_ST_FILE root = null;
            ZCCH_ST_FILE ad1 = null;

            for (int i = 0; i < response.Response.ET_FILES.Take(2).ToArray().Length; i++)
            {
                if (response.Response.ET_FILES[i].ATTRIB.Any(x => x.ATTRID == Constants.FileAttributes.TYPE && x.ATTRVAL == "AD1"))
                {
                    ad1 = ad1 ?? response.Response.ET_FILES[i];
                }
                else
                {
                    root = root ?? response.Response.ET_FILES[i];
                }
            }

            return (root, ad1);
        }

        /// <summary>
        /// Processes the root file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Tuple with model and raw XML content.</returns>
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
    }
}
