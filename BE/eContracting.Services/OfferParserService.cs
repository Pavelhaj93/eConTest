using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using eContracting.Services.Models;

namespace eContracting.Services
{
    /// <inheritdoc/>
    public class OfferParserService : IOfferParserService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferParserService"/> class.
        /// </summary>
        public OfferParserService()
        {
        }

        /// <inheritdoc/>
        public OfferModel GenerateOffer(ResponseCacheGetModel response)
        {
            if (!response.HasFiles)
            {
                return null;
            }

            using (var stream = new MemoryStream(response.Response.ET_FILES.First().FILECONTENT, false))
            {
                var serializer = new XmlSerializer(typeof(OfferXmlModel), "http://www.sap.com/abapxml");
                var offerXml = (OfferXmlModel)serializer.Deserialize(stream);
                var offer = new OfferModel(offerXml);
                offer.IsAccepted = this.GetIsAccepted(response);
                offer.AcceptedAt = this.GetAcceptedAt(response);
                offer.HasGDPR = this.GetHasGdpr(response);
                offer.CreatedAt = this.GetCreatedAt(response);
                offer.Campaign = this.GetCampaign(response);
                offer.GDPRKey = this.GetGdprKey(response);
                offer.State = this.GetState(response);
                return offer;
            }
        }

        protected internal bool GetIsAccepted(ResponseCacheGetModel response)
        {
            return response.Response.ET_ATTRIB != null && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT")
                    && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL)
                    && response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Any(c => char.IsDigit(c));
        }

        protected internal string GetAcceptedAt(ResponseCacheGetModel response)
        {
            if (!this.GetIsAccepted(response))
            {
                return null;
            }

            var acceptedAt = response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Trim();

            if (DateTime.TryParseExact(acceptedAt, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedAcceptedAt))
            {
                return parsedAcceptedAt.ToString("d.M.yyyy");
            }

            return response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL;
        }

        protected internal string GetCreatedAt(ResponseCacheGetModel response)
        {
            if (
                response.Response.ET_ATTRIB != null
                && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "CREATED_AT")
                && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "CREATED_AT").ATTRVAL))
            {
                var created = response.Response.ET_ATTRIB.First(x => x.ATTRID == "CREATED_AT").ATTRVAL;

                if (DateTime.TryParseExact(created, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createdAt))
                {
                    return createdAt.ToString("d.M.yyyy");
                }
                else
                {
                    return created;
                }
            }

            return null;
        }

        protected internal bool GetHasGdpr(ResponseCacheGetModel response)
        {
            return response.Response.ET_ATTRIB != null && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "KEY_GDPR")
                        && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "KEY_GDPR").ATTRVAL);
        }

        protected internal string GetCampaign(ResponseCacheGetModel response)
        {
            if (
                response.Response.ET_ATTRIB != null
                && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "CAMPAIGN")
                && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "CAMPAIGN").ATTRVAL))
            {
                return response.Response.ET_ATTRIB.First(x => x.ATTRID == "CAMPAIGN").ATTRVAL;
            }

            return null;
        }

        protected internal string GetGdprKey(ResponseCacheGetModel response)
        {
            if (this.GetHasGdpr(response))
            {
                return response.Response.ET_ATTRIB.First(x => x.ATTRID == "KEY_GDPR").ATTRVAL;
            }

            return null;
        }

        protected internal string GetState(ResponseCacheGetModel response)
        {
            return response.Response.ES_HEADER.CCHSTAT;
        }
    }
}
