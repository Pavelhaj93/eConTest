using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using eContracting.Models;

namespace eContracting.Services.Models
{
    [Serializable]
    public class OfferBodyXmlModel
    {
        /// <summary>
        /// Gets or sets uuid.
        /// </summary>
        [XmlElement("GUID")]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets CAMPAIGN value.
        /// </summary>
        [XmlIgnore]
        public string Campaign { get; set; }

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

        ///// <summary>
        ///// Gets the flag if offer expired.
        ///// </summary>
        //[XmlIgnore]
        //public bool OfferIsExpired
        //{
        //    get
        //    {
        //        DateTime outValue = DateTime.Now.AddDays(-1);

        //        return DateTime.TryParseExact(DATE_TO, "yyyyMMdd",
        //                            CultureInfo.InvariantCulture,
        //                            DateTimeStyles.None,
        //                            out outValue) && (outValue.Date < DateTime.Now.Date);
        //    }
        //}

        //[XmlIgnore]
        //public bool OfferIsRetention
        //{
        //    get
        //    {
        //        return this.BusProcess == "01";
        //    }
        //}

        //[XmlIgnore]
        //public bool OfferIsAquisition
        //{
        //    get
        //    {
        //        return this.BusProcess == "02";
        //    }
        //}

        //[XmlIgnore]
        //public OfferTypes OfferType
        //{
        //    get
        //    {
        //        if (this.OfferIsRetention)
        //        {
        //            return OfferTypes.Retention;
        //        }
        //        else if (this.OfferIsAquisition)
        //        {
        //            return OfferTypes.Acquisition;
        //        }
        //        else
        //        {
        //            return OfferTypes.Default;
        //        }
        //    }
        //}

        //[XmlIgnore]
        //public bool OfferHasVoucher
        //{
        //    get
        //    {
        //        if (this.Attachments == null || this.Attachments.Length == 0)
        //        {
        //            return false;
        //        }

        //        return this.Attachments.Any(attachment => !string.IsNullOrEmpty(attachment.AddInfo) && attachment.AddInfo.ToLower() == "x");
        //    }
        //}

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

        [XmlElement("BUS_TYPE")]
        public string BusProcessType { get; set; }

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

        public DocumentFileModel[] Attachments { get; set; }
    }
}
