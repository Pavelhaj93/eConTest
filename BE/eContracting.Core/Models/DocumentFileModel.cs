using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eContracting.Models
{
    [Serializable]
    public class DocumentFileModel
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
}
