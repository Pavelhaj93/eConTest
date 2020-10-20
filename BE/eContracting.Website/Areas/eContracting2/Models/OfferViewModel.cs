using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class OfferViewModel
    {
        public OfferPageModel Datasource { get; set; }

        public string MainText { get; set; }
        public string GdprGuid { get; set; }
        public string GdprUrl { get; set; }
    }
}
