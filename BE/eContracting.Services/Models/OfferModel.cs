using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services.Models
{
    public class OfferModel
    {

        public readonly OfferXmlModel Xml;

        /// <summary>
        /// Gets or sets flag if offer is accepted.
        /// </summary>
        public bool IsAccepted { get; set; }

        /// <summary>
        /// Gets or sets infromation when offer was accepted.
        /// </summary>
        public string AcceptedAt { get; set; }

        /// <summary>
        /// Gets or sets CREATED_AT value.
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets information if offer has GDPR.
        /// </summary>
        public bool HasGDPR { get; set; }

        /// <summary>
        /// Gets or sets GDPRKey.
        /// </summary>
        public string GDPRKey { get; set; }

        /// <summary>
        /// Gets or sets CAMPAIGN value.
        /// </summary>
        public string Campaign { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        public string State { get; set; }

        public DocumentFileModel[] Attachments
        {
            get
            {
                return this.Xml.Content.Body.Attachments;
            }
        }

        public string Birthday
        {
            get
            {
                return this.Xml.Content.Body.BIRTHDT;
            }
        }

        public string PartnerNumber
        {
            get
            {
                return this.Xml.Content.Body.PARTNER;
            }
        }

        public string PostNumberConsumption
        {
            get
            {
                return this.Xml.Content.Body.PscMistaSpotreby;
            }
        }

        public string PostNumber
        {
            get
            {
                return this.Xml.Content.Body.PscTrvaleBydliste;
            }
        }

        public bool OfferIsExpired
        {
            get
            {
                DateTime outValue = DateTime.Now.AddDays(-1);

                return DateTime.TryParseExact(this.Xml.Content.Body.DATE_TO, "yyyyMMdd",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out outValue) && (outValue.Date < DateTime.Now.Date);
            }
        }

        public bool OfferIsRetention
        {
            get
            {
                return this.Xml.Content.Body.BusProcess == "01";
            }
        }

        public bool OfferIsAcquisition
        {
            get
            {
                return this.Xml.Content.Body.BusProcess == "02";
            }
        }

        public OfferTypes OfferType
        {
            get
            {
                if (this.OfferIsRetention)
                {
                    return OfferTypes.Retention;
                }
                else if (this.OfferIsAcquisition)
                {
                    return OfferTypes.Acquisition;
                }
                else
                {
                    return OfferTypes.Default;
                }
            }
        }

        public bool OfferHasVoucher
        {
            get
            {
                if (this.Attachments == null || this.Attachments.Length == 0)
                {
                    return false;
                }

                return this.Attachments.Any(attachment => !string.IsNullOrEmpty(attachment.AddInfo) && attachment.AddInfo.ToLower() == "x");
            }
        }

        public OfferModel(OfferXmlModel xml)
        {
            this.Xml = xml;
        }

        public string GetCodeOfAdditionalInfoDocument()
        {
            if (this.Xml.Content?.Body?.Attachments?.Any() ?? false)
            {
                return string.Empty;
            }

            foreach (var attachment in this.Xml.Content.Body.Attachments)
            {
                if (attachment.AddInfo == null)
                {
                    continue;
                }

                if (attachment.AddInfo.ToLower() == "x")
                {
                    return attachment.IdAttach;
                }
            }

            return string.Empty;
        }
    }
}
