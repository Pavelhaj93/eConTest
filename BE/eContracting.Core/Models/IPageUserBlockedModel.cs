using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{E4F4A320-3072-4E2D-A13C-EAEE2FD09D5A}", AutoMap = true)]
    public interface IPageUserBlockedModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string PageTitle { get; set; }

        [SitecoreField]
        string MainText { get; set; }
    }
}
