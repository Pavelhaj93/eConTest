using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{F0DDBD64-C980-4A16-AF12-4A5BA63DCDAB}", AutoMap = true)]
    public interface IModalDialogSimpleModel : IModalDialogBaseModel
    {
        [SitecoreField]
        string Text { get; set; }
    }
}
