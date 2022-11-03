using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{3DCA57C1-AACA-496F-9368-5F42DA5FB244}", AutoMap = true)]
    public interface IModalDialogAcceptOfferModel : IModalDialogBaseModel
    {
        [SitecoreField]
        string Text { get; set; }

        [SitecoreField]
        string CancelOfferLabel { get; set; }

        [SitecoreField]
        string GeneralErrorMessage { get; set; }
    }
}
