using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{C8C58D58-C5D9-47C2-AEF3-F4DEFCA62A2C}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class LoginPageModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string PageTitle { get; set; }

        [SitecoreField]
        public virtual ProcessStepModel Step { get; set; }

        [SitecoreField]
        public virtual string BirthDateLabel { get; set; }

        [SitecoreField]
        public virtual string BirthDatePlaceholder { get; set; }

        [SitecoreField]
        public virtual string BirthDateValidationMessage { get; set; }

        [SitecoreField]
        public virtual string VerificationMethodLabel { get; set; }

        [SitecoreField]
        public virtual string CalendarOpen { get; set; }

        [SitecoreField]
        public virtual string CalendarNextMonth { get; set; }

        [SitecoreField]
        public virtual string CalendarPreviousMonth { get; set; }

        [SitecoreField]
        public virtual string CalendarSelectDay { get; set; }

        [SitecoreField]
        public virtual string ButtonText { get; set; }

        [SitecoreField]
        public virtual string AcceptedOfferText { get; set; }

        [SitecoreField]
        public virtual string NotAcceptedOfferText { get; set; }

        [SitecoreField]
        public virtual string ValidationMessage { get; set; }

        [SitecoreField]
        public virtual string RequiredFields { get; set; }
    }
}
