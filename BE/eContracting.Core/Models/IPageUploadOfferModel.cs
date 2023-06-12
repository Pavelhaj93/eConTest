using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{2F0D37F8-91A3-4686-9DB3-DBF67D010FB5}", AutoMap = true)]
    public interface IPageUploadOfferModel : IBasePageWithStepsModel
    {
        [SitecoreField]
        string TextUnderPageTitle { get; set; }

        [SitecoreField]
        IModalDialogSimpleModel ModalWindowUnfinishedOffer { get; set; }

        [SitecoreField]
        string ButtonContinueLabel { get; set; }
    }
}
