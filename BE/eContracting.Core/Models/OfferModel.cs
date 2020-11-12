using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Newtonsoft.Json;
using Sitecore.Reflection.Emit;

namespace eContracting.Models
{
    [ExcludeFromCodeCoverage]
    public class OfferModel
    {
        /// <summary>
        /// Inner XML with content from SAP.
        /// </summary>
        [JsonIgnore]
        public readonly OfferXmlModel Xml;

        [JsonProperty("parameters")]
        public IDictionary<string, string> TextParameters { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the unique GUID of this offer.
        /// </summary>
        [JsonProperty("guid")]
        public string Guid
        {
            get
            {
                return this.Header.CCHKEY;
            }
        }

        /// <summary>
        /// Gets or sets flag if offer is accepted.
        /// </summary>
        [JsonProperty("accepted")]
        public bool IsAccepted
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == "ACCEPTED_AT")?.Value?.Any(c => char.IsDigit(c)) ?? false;
                //return response.Response.ET_ATTRIB != null && response.Response.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT")
                //    && !string.IsNullOrEmpty(response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL)
                //    && response.Response.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Any(c => char.IsDigit(c));
            }
        }

        /// <summary>
        /// Gets or sets infromation when offer was accepted.
        /// </summary>
        [JsonProperty("accepted_date")]
        public string AcceptedAt
        {
            get
            {
                if (!this.IsAccepted)
                {
                    return null;
                }

                var acceptedAt = this.Attributes.FirstOrDefault(x => x.Key == "ACCEPTED_AT")?.Value?.Trim();

                if (DateTime.TryParseExact(acceptedAt, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedAcceptedAt))
                {
                    return parsedAcceptedAt.ToString("d.M.yyyy");
                }

                return acceptedAt;
            }
        }

        /// <summary>
        /// Gets or sets CREATED_AT value.
        /// </summary>
        [JsonProperty("craeted_date")]
        public string CreatedAt
        {
            get
            {
                var created = this.Attributes.FirstOrDefault(x => x.Key == "CREATED_AT")?.Value;

                if (created != null && DateTime.TryParseExact(created, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createdAt))
                {
                    return createdAt.ToString("d.M.yyyy");
                }

                return created;
            }
        }

        /// <summary>
        /// Gets or sets information if offer has GDPR.
        /// </summary>
        [JsonProperty("gdpr")]
        public bool HasGDPR
        {
            get
            {
                return !string.IsNullOrEmpty(this.GDPRKey);
            }
        }

        /// <summary>
        /// Gets or sets GDPRKey.
        /// </summary>
        [JsonProperty("gdpr_key")]
        public string GDPRKey
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == "KEY_GDPR")?.Value;
            }
        }

        /// <summary>
        /// Gets or sets CAMPAIGN value.
        /// </summary>
        [JsonProperty("campaign")]
        public string Campaign
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == "CAMPAIGN")?.Value;
            }
        }

        /// <summary>
        /// Gets the commodity (EAN or EIC).
        /// </summary>
        [JsonProperty("commodity")]
        public string Commodity
        {
            get
            {
                return this.Xml.Content.Body.EanOrAndEic;
            }
        }

        [JsonProperty("isCampaign")]
        public bool IsCampaign
        {
            get
            {
                return !string.IsNullOrEmpty(this.Campaign);
            }
        }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        /// <remarks>
        ///     Description provided by Tereza:
        ///     <list type="table">
        ///         <item>1 - Zapsáno</item>    
        ///         <item>2 - Automatizace (je, že ji můžeme načíst, je jsou dokumenty v pořádku vygenerované)</item>
        ///         <item>3 - Aktivní</item>
        ///         <item>4 - Přečteno (se nastavuje, když si zákazník otevře na webu)</item>
        ///         <item>5 - Akceptováno</item>
        ///         <item>6 - Přihlášeno (když se poprvé přihlásí)</item>
        ///         <item>8 - Chyba</item>
        ///         <item>9 - Zastaralé (když je to stornované (prošlost se totiž ve skutečnosti podle mě kontrolovala podle data)</item>
        ///     </list>
        /// </remarks>
        [JsonProperty("state")]
        public string State
        {
            get
            {
                return this.Header.CCHSTAT;
            }
        }

        [JsonIgnore]
        public DocumentFileModel[] Attachments
        {
            get
            {
                return this.Xml.Content.Body.Attachments;
            }
        }

        [JsonProperty("birthdate")]
        public string Birthday
        {
            get
            {
                return this.Xml.Content.Body.BIRTHDT;
            }
        }

        [JsonProperty("pn")]
        public string PartnerNumber
        {
            get
            {
                return this.Xml.Content.Body.PARTNER;
            }
        }

        [JsonProperty("zip2")]
        public string PostNumberConsumption
        {
            get
            {
                return this.Xml.Content.Body.PscMistaSpotreby;
            }
        }

        [JsonProperty("zip1")]
        public string PostNumber
        {
            get
            {
                return this.Xml.Content.Body.PscTrvaleBydliste;
            }
        }

        [JsonProperty("process")]
        public string Process
        {
            get
            {
                return this.Xml.Content.Body.BusProcess;
            }
        }

        [JsonProperty("process_type")]
        public string ProcessType
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Xml.Content.Body.BusProcessType))
                {
                    return this.Xml.Content.Body.BusProcessType;
                }

                return string.IsNullOrEmpty(this.Campaign) ? "INDI" : "CAMPAIGN";
            }
        }

        /// <summary>
        /// Gets a value indicating whether offer already expired.
        /// </summary>
        [JsonProperty("expired")]
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

        [JsonProperty("has_voucher")]
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

        /// <summary>
        /// Attributes from the offer.
        /// </summary>
        [JsonIgnore]
        protected readonly OfferAttributeModel[] Attributes;

        /// <summary>
        /// Header values from the offer.
        /// </summary>
        [JsonProperty("header")]
        protected readonly OfferHeaderModel Header;

        [JsonIgnore]
        public readonly IDictionary<string, string> RawContent = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferModel"/> class.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="header">The header.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="textParameters">The text parameters.</param>
        /// <exception cref="ArgumentNullException">
        /// xml
        /// or
        /// header
        /// or
        /// attributes
        /// </exception>
        /// <exception cref="ArgumentException">Inner content of {nameof(xml)} cannot be null</exception>
        public OfferModel(OfferXmlModel xml, OfferHeaderModel header, OfferAttributeModel[] attributes, Dictionary<string, string> textParameters = null)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            if (xml.Content == null)
            {
                throw new ArgumentException($"Inner content of {nameof(xml)} cannot be null");
            }

            this.Xml = xml;
            this.Header = header ?? throw new ArgumentNullException(nameof(header));
            this.Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));

            if (textParameters != null)
            {
                this.TextParameters.Merge(textParameters);
            }
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

        /// <summary>
        /// Gets value from inner 'Xml.Content.Body' by XML name attribute.
        /// </summary>
        /// <param name="xmlElementName">Name of the XML element.</param>
        /// <returns>Value or null.</returns>
        public string GetValue(string xmlElementName)
        {
            return this.Xml.Content.Body.Get(xmlElementName);
        }
    }
}
