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
    /// <summary>
    /// Represents single offer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class OfferModel : IOfferDataModel
    {
        /// <summary>
        /// Inner XML with content from SAP.
        /// </summary>
        [JsonIgnore]
        public readonly OfferXmlModel Xml;

        [JsonIgnore]
        //[JsonProperty("parameters")]
        public IDictionary<string, string> TextParameters { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the version of the offer.
        /// </summary>
        /// <remarks>
        ///     <list type="table">
        ///         <item>1 - old offer without bus process type and with text parameters</item>
        ///         <item>2 - new offer MODELO_OFERTA = 01</item>
        ///         <item>3 - new offer MODELO_OFERTA = 02</item>
        ///         <item>4 - new offer MODELO_OFERTA = 03</item>
        ///     </list>
        /// </remarks>
        [JsonProperty("version")]
        public readonly int Version;

        /// <summary>
        /// Gets the unique GUID of this offer.
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
        /// Gets guid of sibling offer to this one.
        /// </summary>
        public string SiblingGuid
        {
            get
            {
                return this.Xml.Content.Body.SiblingGuid;
            }
        }

        /// <summary>
        /// Gets or sets flag if offer is accepted.
        /// </summary>
        [JsonProperty("accepted")]
        public bool IsAccepted { get; }

        [JsonProperty("expiration_date")]
        public DateTime ExpirationDate { get; }

        /// <summary>
        /// Gets a value indicating whether offer already expired.
        /// </summary>
        [JsonProperty("expired")]
        public bool IsExpired { get; }

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
        [JsonProperty("created_date")]
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
        [JsonProperty("has_gdpr")]
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
        [JsonProperty("gdpr")]
        public string GDPRKey
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == Constants.OfferAttributes.KEY_GDPR)?.Value;
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
                return this.Attributes.FirstOrDefault(x => x.Key == Constants.OfferAttributes.CAMPAIGN)?.Value;
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

        [JsonProperty("templates")]
        public OfferAttachmentXmlModel[] Documents
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

        [JsonProperty("zip")]
        public string PostNumber
        {
            get
            {
                return this.Xml.Content.Body.PscTrvaleBydliste;
            }
        }

        /// <summary>
        /// Gets value of BUS_PROCESS.
        /// </summary>
        [JsonProperty("process")]
        public string Process
        {
            get
            {
                return this.Xml.Content.Body.BusProcess;
            }
        }

        /// <summary>
        /// Gets value of BUS_TYPE.
        /// </summary>
        [JsonProperty("process_type")]
        public string ProcessType
        {
            get
            {
                return this.Xml.Content.Body.BusProcessType;
            }
        }

        [JsonProperty("has_voucher")]
        public bool OfferHasVoucher
        {
            get
            {
                if (this.Documents == null || this.Documents.Length == 0)
                {
                    return false;
                }

                return this.Documents.Any(attachment => !string.IsNullOrEmpty(attachment.AddInfo) && attachment.AddInfo.ToLower() == "x");
            }
        }

        /// <summary>
        /// Actual value of customer GRPD identity number from <see cref="Constants.OfferAttributes.ZIDENTITYID"/>.
        /// </summary>
        [JsonProperty("gdrp")]
        public string GdprIdentity
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == Constants.OfferAttributes.ZIDENTITYID)?.Value;
            }
        }

        /// <summary>
        /// Gets status of customer registration in app portal. 
        /// </summary>
        /// <value>Possible values: PORTAL, PORTAL|APP, APP</value>
        /// <remarks>If attribute <see cref="Constants.OfferAttributes.MCFU_REG_STAT"/>  doesn't exist or is empty, customer is not able to log in via innosvet login.</remarks>
        [JsonProperty("mcfu")]  
        public string McfuRegStat
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == Constants.OfferAttributes.MCFU_REG_STAT)?.Value;
            }
        }

        /// <summary>
        /// Determinates if the offer has attribute <see cref="Constants.OfferAttributes.MCFU_REG_STAT"/> or not saying this offer is assigned to Cognito user.
        /// </summary>
        [JsonProperty("has_mcfu")]
        public bool HasMcfu
        {
            get
            {
                return !string.IsNullOrEmpty(this.McfuRegStat);
            }
        }

        public string RegistrationLink
        {
            get
            {
                if (this.TextParameters.HasValue(Constants.OfferTextParameters.REGISTRATION_LINK))
                {
                    var value = this.TextParameters[Constants.OfferTextParameters.REGISTRATION_LINK];

                    if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    {
                        return value;
                    }

                    if (!value.StartsWith("http"))
                    {
                        value = value.TrimStart('/'); // is url starts with '//', for example: //test.innogy.cz
                        value = "https://" + value;

                        if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                        {
                            return value;
                        }
                    }

                    return null;
                }

                return null;
            }
        }

        /// <summary>
        /// Determinates if user can cancel the offer.
        /// </summary>
        [JsonProperty("cancelable")]
        public bool CanBeCanceled
        {
            get
            {
                return this.Process == "01" && this.Attributes.Any(x => x.Key == Constants.OfferAttributes.ORDER_ORIGIN && x.Value == "2");
            }
        }

        [JsonProperty("show_prices")]
        public bool ShowPrices
        {
            get
            {
                return !this.Attributes.Any(x => x.Key == Constants.OfferAttributes.NO_PROD_CHNG && x.Value == Constants.CHECKED);
            }
        }

        /// <summary>
        /// Attributes from the offer.
        /// </summary>
        [JsonIgnore]
        public OfferAttributeModel[] Attributes { get; private set; }

        /// <summary>
        /// Header values from the offer.
        /// </summary>
        [JsonProperty("header")]
        protected OfferHeaderModel Header { get; private set; }

        [JsonIgnore]
        public IDictionary<string, string> RawContent { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferModel"/> class.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="version">The version.</param>
        /// <param name="header">The header.</param>
        /// <param name="isAccepted">if set to <c>true</c> [is accepted].</param>
        /// <param name="isExpired">if set to <c>true</c> [is expired].</param>
        /// <param name="attributes">The attributes.</param>
        /// <exception cref="ArgumentNullException">
        /// xml
        /// or
        /// header
        /// or
        /// attributes
        /// </exception>
        /// <exception cref="ArgumentException">Inner content of {nameof(xml)} cannot be null</exception>
        public OfferModel(OfferXmlModel xml, int version, OfferHeaderModel header, bool isAccepted, bool isExpired, DateTime expirationDate, OfferAttributeModel[] attributes)
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
            this.Version = version;
            this.Header = header ?? throw new ArgumentNullException(nameof(header));
            this.IsAccepted = isAccepted;
            this.ExpirationDate = expirationDate;
            this.IsExpired = isExpired;
            this.Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
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
