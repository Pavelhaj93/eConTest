using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Shell.Applications.ContentEditor;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{B9B48C83-1724-43B5-8200-9CDE955D6835}", AutoMap = true)]
    public class DefinitionCombinationModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual ProcessModel Process { get; set; }

        [SitecoreField]
        public virtual ProcessModel ProcessType { get; set; }

        [SitecoreField]
        public virtual IEnumerable<LoginTypeModel> LoginTypes { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextLogin { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextLoginAccepted { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextOffer { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextOfferAccepted { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextOfferExpired { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextThankYou { get; set; }
    }
}
