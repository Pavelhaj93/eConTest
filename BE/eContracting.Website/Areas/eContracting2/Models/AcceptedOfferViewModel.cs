using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class AcceptedOfferViewModel
    {
        public readonly PageAcceptedOfferModel Datasource;
        public readonly string AppUnAvailableTitle;
        public readonly string AppUnAvailableText;

        public AcceptedOfferViewModel(PageAcceptedOfferModel datasource, string appUnAvailableTitle, string appUnAvailableText)
        {
            this.Datasource = datasource;
            this.AppUnAvailableTitle = appUnAvailableTitle;
            this.AppUnAvailableText = appUnAvailableText;
        }
    }
}
