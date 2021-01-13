using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{220B964C-EA21-4672-92F1-58CCE932BD33}", AutoMap = true)]
    public class CookieLawSettingsModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string Text { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link { get; set; }

        [SitecoreField]
        public virtual string ButtonLabel { get; set; }
    }
}
