using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{F6909B18-2F78-459B-866B-3CC82F308BA7}", AutoMap = true)]
    public interface IPageSummaryOfferModel : IBasePageWithStepsModel
    {
        [SitecoreField]
        string TextUnderPageTitle { get; set; }

        [SitecoreField]
        string DistributorChange_Title { get; set; }

        [SitecoreField]
        string DistributorChange_Text { get; set; }

        [SitecoreField]
        IModalDialogSimpleModel ModalWindowUnfinishedOffer { get; set; }

        [SitecoreField]
        string ButtonContinueLabel { get; set; }
    }
}
