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

            var file1 = Encoding.UTF8.GetString(response.Response.ET_FILES[0].FILECONTENT);
            OfferXmlModel offerXml = null;
            var header = this.GetHeader(response);
            var attributes = this.GetAttributes(response);
            var parameters = new Dictionary<string, string>();
            var rawXml = new Dictionary<string, string>();
            rawXml.Add(response.Response.ET_FILES[0].FILENAME, file1);

            using (var stream = new MemoryStream(response.Response.ET_FILES[0].FILECONTENT, false))
            {
                var serializer = new XmlSerializer(typeof(OfferXmlModel), "http://www.sap.com/abapxml");
                offerXml = (OfferXmlModel)serializer.Deserialize(stream);
                
                if (string.IsNullOrEmpty(offerXml.Content.Body.BusProcess))
                {
                    offerXml.Content.Body.BusProcess = "00";
                }
            }

            if (response.Response.ET_FILES.Length > 1)
            {
                try
                {
                    var file2 = Encoding.UTF8.GetString(response.Response.ET_FILES[1].FILECONTENT);
                    rawXml.Add(response.Response.ET_FILES[1].FILENAME, file2);

                    var doc = new XmlDocument();
                    var xrs = new XmlReaderSettings() { NameTable = new NameTable() };
                    var xnsm = new XmlNamespaceManager(xrs.NameTable);
                    xnsm.AddNamespace("BENEFITS", "");
                    xnsm.AddNamespace("COMMODITY", "");
                    xnsm.AddNamespace("NONCOMMODITY", "");
                    xnsm.AddNamespace("PERSON", "");
                    var xcp = new XmlParserContext(null, xnsm, "", XmlSpace.Default);
                    using (var strReader = new StringReader(file2))
                    {
                        var t = System.Xml.Linq.XDocument.Load(strReader, System.Xml.Linq.LoadOptions.PreserveWhitespace);
                        var xr = XmlReader.Create(strReader, xrs, xcp);
                        doc.Load(xr);

                        var xmlParameters = doc.DocumentElement.SelectSingleNode("form/parameters").ChildNodes;

                        if (xmlParameters != null)
                        {
                            foreach (XmlNode xmlNode in xmlParameters)
                            {
                                var key = xmlNode.Name;
                                var value = xmlNode.InnerXml;
                                parameters.Add(key, value);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }

            var offer = new OfferModel(offerXml, header, attributes, parameters);

            foreach (var raw in rawXml)
            {
                offer.RawContent.Add(raw.Key, raw.Value);
            }

            return offer;
        }

        /// <summary>
        /// Gets the header from <see cref="ZCCH_CACHE_GETResponse.ES_HEADER"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        protected internal OfferHeaderModel GetHeader(ResponseCacheGetModel response)
        {
            return new OfferHeaderModel(response.Response.ES_HEADER);
        }

        /// <summary>
        /// Gets the attributes from <see cref="ZCCH_CACHE_GETResponse.ET_ATTRIB"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Array of attributes.</returns>
        protected internal OfferAttributeModel[] GetAttributes(ResponseCacheGetModel response)
        {
            var list = new List<OfferAttributeModel>();

            for (int i = 0; i < response.Response.ET_ATTRIB.Length; i++)
            {
                var attrib = response.Response.ET_ATTRIB[i];
                list.Add(new OfferAttributeModel(attrib));
            }

            return list.ToArray();
        }

        //protected internal bool GetIsAccepted(ResponseCacheGetModel response)
        //{
        //    return response.Response.ET_ATTRIB != null && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT")
        //            && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL)
        //            && response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Any(c => char.IsDigit(c));
        //}

        //protected internal string GetAcceptedAt(ResponseCacheGetModel response)
        //{
        //    if (!this.GetIsAccepted(response))
        //    {
        //        return null;
        //    }

        //    var acceptedAt = response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Trim();

        //    if (DateTime.TryParseExact(acceptedAt, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedAcceptedAt))
        //    {
        //        return parsedAcceptedAt.ToString("d.M.yyyy");
        //    }

        //    return response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL;
        //}

        //protected internal string GetCreatedAt(ResponseCacheGetModel response)
        //{
        //    if (
        //        response.Response.ET_ATTRIB != null
        //        && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "CREATED_AT")
        //        && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "CREATED_AT").ATTRVAL))
        //    {
        //        var created = response.Response.ET_ATTRIB.First(x => x.ATTRID == "CREATED_AT").ATTRVAL;

        //        if (DateTime.TryParseExact(created, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createdAt))
        //        {
        //            return createdAt.ToString("d.M.yyyy");
        //        }
        //        else
        //        {
        //            return created;
        //        }
        //    }

        //    return null;
        //}

        //protected internal bool GetHasGdpr(ResponseCacheGetModel response)
        //{
        //    return response.Response.ET_ATTRIB != null && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "KEY_GDPR")
        //                && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "KEY_GDPR").ATTRVAL);
        //}

        //protected internal string GetCampaign(ResponseCacheGetModel response)
        //{
        //    if (
        //        response.Response.ET_ATTRIB != null
        //        && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "CAMPAIGN")
        //        && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "CAMPAIGN").ATTRVAL))
        //    {
        //        return response.Response.ET_ATTRIB.First(x => x.ATTRID == "CAMPAIGN").ATTRVAL;
        //    }

        //    return null;
        //}

        //protected internal string GetGdprKey(ResponseCacheGetModel response)
        //{
        //    if (this.GetHasGdpr(response))
        //    {
        //        return response.Response.ET_ATTRIB.FirstOrDefault(x => x.ATTRID == "KEY_GDPR")?.ATTRVAL;
        //    }

        //    return null;
        //}

        //protected internal string GetState(ResponseCacheGetModel response)
        //{
        //    return response.Response.ES_HEADER.CCHSTAT;
        //}
    }
}
