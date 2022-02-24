using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class InfoBoxViewModel
    {
        public string Title { get; set; }

        public IEnumerable<string> Items { get; set; }

        public Link ButtonLink { get; set; }

        public string ButtonLabel { get; set; }
    }
}
