using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{E97736D1-7DCC-4040-8C54-9BE97E4F9D0F}", AutoMap = true)]
    public class SimpleTextModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string Text { get; set; }
    }
}
