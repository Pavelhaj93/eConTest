using System;
using System.Globalization;
using System.Xml.Serialization;

namespace rweClient.SerializationClasses
{
    [Serializable()]
    [XmlRoot("Nabidka")]
    public class Offer
    {
        [XmlElement("Header")]
        public Header Header { get; set; }

        [XmlElement("Body")]
        public Body Body { get; set; }

        [XmlIgnore]
        public Boolean IsAccepted { get; set; }
    }

    [Serializable()]
    public class Header
    {
    }

    [Serializable()]
    public class Body
    {
        [XmlElement("GUID")]
        public string Guid { get; set; }

        [XmlElement("ISU_CONTRACT")]
        public string ISU_CONTRACT { get; set; }

        [XmlElement("CONTSTART")]
        public string DatumUcinnostiDodatku { get; set; }

        [XmlElement("DATE_TO")]
        public string DATE_TO { get; set; }

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

        [XmlElement("STATUS")]
        public string STATUS { get; set; }

        [XmlElement("PARTNER")]
        public string PARTNER { get; set; }

        [XmlElement("NAME_LAST")]
        public string NAME_LAST { get; set; }

        [XmlElement("NAME_FIRST")]
        public string NAME_FIRST { get; set; }

        [XmlElement("NAME_ORG1")]
        public string NAME_ORG1 { get; set; }

        [XmlElement("BIRTHDT")]
        public string BIRTHDT { get; set; }

        [XmlElement("IC")]
        public string OrganizationNumber { get; set; }

        [XmlElement("EMAIL")]
        public string EMAIL { get; set; }

        [XmlElement("PHONE")]
        public string PHONE { get; set; }

        [XmlElement("EXT_UI")]
        public string EanOrAndEic { get; set; }

        [XmlElement("VSTELLE")]
        public string NumberOfMistoSpotreby { get; set; }

        [XmlElement("ANLAGE")]
        public string NumberOfOdberneMisto { get; set; }

        [XmlElement("BUAG")]
        public string NumberOfObchodniDohoda { get; set; }

        [XmlElement("PSC_MS")]
        public string PscMistaSpotreby { get; set; }

        [XmlElement("PSC_ADDR")]
        public string PscTrvaleBydliste { get; set; }

        [XmlElement("ACCOUNT_NUMBER")]
        public string ACCOUNT_NUMBER { get; set; }
    }
}
