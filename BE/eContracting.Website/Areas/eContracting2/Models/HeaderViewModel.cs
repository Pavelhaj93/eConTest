using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class HeaderViewModel
    {
        public IPageHeaderModel Datasource { get; set; }

        public bool ShowLogoutButton { get; set; }

        public string LogoutUrl { get; set; }

        public string LogoutUrlLabel { get; set; }
    }
}
