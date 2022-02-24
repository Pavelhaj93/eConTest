using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ButtonsViewModel
    {
        public IAccountButtonsModel Datasource { get; set; }

        public bool ShowRegistrationButtons { get; set; }

        public string ButtonNewAccountUrl { get; set; }

        public string ButtonLoginAccountUrl { get; set; }

        public string ButtonDashboardUrl { get; set; }

        public bool HasTooltip { get; set; }
    }
}
