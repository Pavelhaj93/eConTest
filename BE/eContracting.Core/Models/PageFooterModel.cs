using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{1C38E7D9-42D9-4C95-9800-89E0069037BA}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class PageFooterModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string CopyrightText { get; set; }

        [SitecoreField]
        public virtual string EmailText { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual string EmailLink { get; set; }

        [SitecoreField]
        public virtual string PhoneNumberFooter { get; set; }

        [SitecoreField]
        public virtual string PhoneNumberLinkFooter { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link DisclaimerLink { get; set; }

        [SitecoreIgnore]
        public string DisclaimerLinkUrl
        {
            get
            {
                return this.DisclaimerLink?.Url;
            }
        }
    }
}
