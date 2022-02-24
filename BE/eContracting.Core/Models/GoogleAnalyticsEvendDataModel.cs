using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class GoogleAnalyticsEvendDataModel
    {
        [JsonProperty("eCat")]
        public string Cat { get; set; }

        [JsonProperty("eAct")]
        public string Act { get; set; }

        [JsonProperty("eLab")]
        public string Lab { get; set; }

        public GoogleAnalyticsEvendDataModel()
        {
        }

        public GoogleAnalyticsEvendDataModel(string eCat, string eAct, string eLab)
        {
            this.Cat = eCat;
            this.Act = eAct;
            this.Lab = eLab;
        }
    }
}
