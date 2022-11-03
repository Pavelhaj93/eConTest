using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{64A979C7-3F85-49C5-A53C-93CE66A68DA5}", AutoMap = true)]
    public interface IModalDialogSignOfferModel : IModalDialogBaseModel
    {
        [SitecoreField]
        string Text { get; set; }

        [SitecoreField]
        string ThumbnailText { get; set; }

        [SitecoreField]
        string PenArea { get; set; }

        [SitecoreField]
        string Note { get; set; }

        [SitecoreField]
        string GeneralErrorMessage { get; set; }

        [SitecoreField]
        string Sign { get; set; }

        [SitecoreField]
        string Close { get; set; }
    }
}
