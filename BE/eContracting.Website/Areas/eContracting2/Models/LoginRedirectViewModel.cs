using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class LoginRedirectViewModel
    {
        public readonly string RedirectUrl;

        public readonly int RedirectTimeout;

        public LoginRedirectViewModel(string redirectUrl, int redirectTimeout)
        {
            this.RedirectUrl = redirectUrl;
            this.RedirectTimeout = redirectTimeout;
        }
    }
}
