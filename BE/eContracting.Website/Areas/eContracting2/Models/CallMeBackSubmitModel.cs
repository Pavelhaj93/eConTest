using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class CallMeBackSubmitModel
    {
        public string Phone { get; set; }
        public string Time { get; set; }

        public string Note { get; set; }
    }
}
