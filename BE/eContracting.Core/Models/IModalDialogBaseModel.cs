using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{BB623970-E484-41CC-AFF8-D7F94A0B9DDD}", AutoMap = true)]
    public interface IModalDialogBaseModel
    {
        [SitecoreField]
        string Title { get; set; }

        [SitecoreField]
        string ConfirmButtonLabel { get; set; }

        [SitecoreField]
        string CancelButtonLabel { get; set; }
    }
}
