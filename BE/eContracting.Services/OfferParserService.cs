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

        /// <summary>
        /// Processes the response and create <see cref="OfferModel"/> or root XML exists.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Model or null.</returns>
        protected internal OfferModel ProcessResponse(ResponseCacheGetModel response)
        {
            var root = response.Response.ET_FILES.FirstOrDefault(x => !x.FILENAME.EndsWith("_AD1.xml"));
            var ad1 = response.Response.ET_FILES.FirstOrDefault(x => x.FILENAME.EndsWith("_AD1.xml"));
            
            if (root == null)
            {
                return null;
            }

            var header = this.GetHeader(response);
            var attributes = this.GetAttributes(response);

            var rootResult = this.ProcessRootFile(root);

            Dictionary<string, string> parameters = null;
            string ad1RawContent = null;

            if (ad1 != null)
            {
                var ad1Result = this.ProcessAd1File(ad1);

                parameters = ad1Result.parameters;
                ad1RawContent = ad1Result.rawContent;
            }

            var offer = new OfferModel(rootResult.model, header, attributes, parameters);
            offer.RawContent.Add(root.FILENAME, rootResult.rawContent);
            offer.RawContent.Add(ad1.FILENAME, ad1RawContent);

            return offer;
        }

        /// <summary>
        /// Processes the root file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Tuple with model and raw XML content.</returns>
        protected internal (OfferXmlModel model, string rawContent) ProcessRootFile(ZCCH_ST_FILE file)
        {
            var rawContent = Encoding.UTF8.GetString(file.FILECONTENT);

            using (var stream = new MemoryStream(file.FILECONTENT, false))
            {
                var serializer = new XmlSerializer(typeof(OfferXmlModel), "http://www.sap.com/abapxml");
                var offerXml = (OfferXmlModel)serializer.Deserialize(stream);

                if (string.IsNullOrEmpty(offerXml.Content.Body.BusProcess))
                {
                    offerXml.Content.Body.BusProcess = "00";
                }

                return (offerXml, rawContent);
            }
        }

        /// <summary>
        /// Processes the ad1 file with parameters.
        /// </summary>
        /// <param name="file">The file.</param>
        /// /// <returns>Tuple with model and raw XML content.</returns>
        protected internal (Dictionary<string, string> parameters, string rawContent) ProcessAd1File(ZCCH_ST_FILE file)
        {
            var rawContent = Encoding.UTF8.GetString(file.FILECONTENT);
            var parameters = new Dictionary<string, string>();

            var doc = new XmlDocument();
            doc.LoadXml(rawContent);
            var xmlParameters = doc.SelectSingleNode("form/parameters").ChildNodes;

            if (xmlParameters != null)
            {
                foreach (XmlNode xmlNode in xmlParameters)
                {
                    var key = xmlNode.Name;
                    var value = xmlNode.InnerXml;
                    parameters.Add(key, value);
                }
            }

            return (parameters, rawContent);
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
