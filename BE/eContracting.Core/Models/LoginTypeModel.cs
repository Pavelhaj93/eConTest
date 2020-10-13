using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{1CF3E1F2-7C49-4ED6-9CB1-FA9EF95A73DC}", AutoMap = true)]
    public class LoginTypeModel : BaseSitecoreModel
    {
        /// <summary>
        /// Gets computed unique id base on <see cref="BaseSitecoreModel.ID"/>. This is used only for login page.
        /// </summary>
        [SitecoreIgnore]
        public string ComputeId
        {
            get
            {
                return Utils.GetMd5(this.ID.ToString());
            }
        }

        [SitecoreField]
        public virtual string Label { get; set; }

        [SitecoreIgnore]
        public virtual string Key { get; set; }

        [SitecoreField]
        public virtual string HelpText { get; set; }

        [SitecoreField]
        public virtual string Placeholder { get; set; }

        [SitecoreField]
        public virtual string ValidationRegex { get; set; }

        [SitecoreField]
        public virtual string ValidationMessage { get; set; }
    }
}
