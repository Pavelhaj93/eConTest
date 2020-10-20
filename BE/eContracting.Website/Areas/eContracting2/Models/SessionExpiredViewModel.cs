using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class SessionExpiredViewModel : BaseViewModel
    {
        public string MainText { get; set; }

        public SessionExpiredViewModel() : base("Expiration")
        {
        }        
    }
}
