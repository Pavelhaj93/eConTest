// <copyright file="Offer.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Services
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Implementaiton of offer.
    /// </summary>
    [Serializable()]
    [XmlRoot("abap", Namespace = "http://www.sap.com/abapxml")]
    public class Offer
    {
        /// <summary>
        /// Gets or sets internal offer.
        /// </summary>
        [XmlElement("Nabidka", Namespace = "")]
        public OfferInternal OfferInternal { get; set; }
    }

    /// <summary>
    /// Implementation of internal offer model.
    /// </summary>
    [Serializable()]
    public class OfferInternal
    {
        /// <summary>
        /// Gets or sets header.
        /// </summary>
        [XmlElement("Header")]
        public Header Header { get; set; }

        /// <summary>
        /// Gets or sets body.
        /// </summary>
        [XmlElement("Body")]
        public Body Body { get; set; }

        /// <summary>
        /// Gets or sets flag if offer is accepted.
        /// </summary>
        [XmlIgnore]
        public Boolean IsAccepted { get; set; }

        /// <summary>
        /// Gets or sets infromation when offer was accepted.
        /// </summary>
        [XmlIgnore]
        public string AcceptedAt { get; set; }

        /// <summary>
        /// Gets or sets information if offer has GDPR.
        /// </summary>
        [XmlIgnore]
        public bool HasGDPR { get; set; }

        /// <summary>
        /// Gets or sets GDPRKey.
        /// </summary>
        [XmlIgnore]
        public string GDPRKey { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        [XmlIgnore]
        public string State { get; set; }
    }

    /// <summary>
    /// Implementaiton of header model.
    /// </summary>
    [Serializable()]
    public class Header
    {
    }

    [Serializable()]
    public class Template
    {
        [XmlElement("IDATTACH")]
        public string IdAttach { get; set; }

        [XmlElement("DESCRIPTION")]
        public string Description { get; set; }

        [XmlElement("ADDINFO")]
        public string AddInfo { get; set; }

        [XmlElement("SIGN_REQ")]
        public string SignReq { get; set; }

        [XmlElement("TEMPL_ALC_ID")]
        public string TemplAlcId { get; set; }
    }

    /// <summary>
    /// Implementaion of body model.
    /// </summary>
    [Serializable()]
    public class Body
    {
        /// <summary>
        /// Gets or sets uuid.
        /// </summary>
        [XmlElement("GUID")]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets ISU_CONTRACT from SAP.
        /// </summary>
        [XmlElement("ISU_CONTRACT")]
        public string ISU_CONTRACT { get; set; }

        /// <summary>
        /// Gets or sets CONTSTART from SAP.
        /// </summary>
        [XmlElement("CONTSTART")]
        public string DatumUcinnostiDodatku { get; set; }

        /// <summary>
        /// Gets or sets DATE_TO from SAP.
        /// </summary>
        [XmlElement("DATE_TO")]
        public string DATE_TO { get; set; }

        /// <summary>
        /// Gets the flag if offer expired.
        /// </summary>
        [XmlIgnore]
        public Boolean OfferIsExpired
        {
            get
            {
                DateTime outValue = DateTime.Now.AddDays(-1);

                return DateTime.TryParseExact(DATE_TO, "yyyyMMdd",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out outValue) && (outValue.Date < DateTime.Now.Date);
            }
        }

        [XmlIgnore]
        public bool OfferIsRetention
        {
            get
            {
                return this.BusProcess == "01";
            }
        }

        /// <summary>
        /// Gets or sets STATUS from SAP.
        /// </summary>
        [XmlElement("STATUS")]
        public string STATUS { get; set; }

        /// <summary>
        /// Gets or sets PARTNER from SAP.
        /// </summary>
        [XmlElement("PARTNER")]
        public string PARTNER { get; set; }

        /// <summary>
        /// Gets or sets NAME_LAST from SAP.
        /// </summary>
        [XmlElement("NAME_LAST")]
        public string NAME_LAST { get; set; }

        /// <summary>
        /// Gets or sets NAME_FIRST from SAP.
        /// </summary>
        [XmlElement("NAME_FIRST")]
        public string NAME_FIRST { get; set; }

        /// <summary>
        /// Gets or sets NAME_ORG1 from SAP.
        /// </summary>
        [XmlElement("NAME_ORG1")]
        public string NAME_ORG1 { get; set; }

        /// <summary>
        /// Gets or sets BIRTHDT from SAP.
        /// </summary>
        [XmlElement("BIRTHDT")]
        public string BIRTHDT { get; set; }

        /// <summary>
        /// Gets or sets IC from SAP.
        /// </summary>
        [XmlElement("IC")]
        public string OrganizationNumber { get; set; }

        /// <summary>
        /// Gets or sets EMAIL from SAP.
        /// </summary>
        [XmlElement("EMAIL")]
        public string EMAIL { get; set; }

        /// <summary>
        /// Gets or sets PHONE from SAP.
        /// </summary>
        [XmlElement("PHONE")]
        public string PHONE { get; set; }

        [XmlElement("BUS_PROCESS")]
        public string BusProcess { get; set; }

        /// <summary>
        /// Gets or sets EXT_UI from SAP.
        /// </summary>
        [XmlElement("EXT_UI")]
        public string EanOrAndEic { get; set; }

        /// <summary>
        /// Gets or sets VSTELLE from SAP.
        /// </summary>
        [XmlElement("VSTELLE")]
        public string NumberOfMistoSpotreby { get; set; }

        /// <summary>
        /// Gets or sets ANLAGE from SAP.
        /// </summary>
        [XmlElement("ANLAGE")]
        public string NumberOfOdberneMisto { get; set; }

        /// <summary>
        /// Gets or sets BUAG from SAP.
        /// </summary>
        [XmlElement("BUAG")]
        public string NumberOfObchodniDohoda { get; set; }

        /// <summary>
        /// Gets or sets PSC_MS from SAP.
        /// </summary>
        [XmlElement("PSC_MS")]
        public string PscMistaSpotreby { get; set; }

        /// <summary>
        /// Gets or sets PSC_ADDR from SAP.
        /// </summary>
        [XmlElement("PSC_ADDR")]
        public string PscTrvaleBydliste { get; set; }

        /// <summary>
        /// Gets or sets ACCOUNT_NUMBER from SAP.
        /// </summary>
        [XmlElement("ACCOUNT_NUMBER")]
        public string ACCOUNT_NUMBER { get; set; }

        public Template[] Attachments { get; set; }
    }
}
