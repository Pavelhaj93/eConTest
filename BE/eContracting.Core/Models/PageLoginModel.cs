using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    /// <summary>
    /// Represents login page.
    /// </summary>
    /// <seealso cref="eContracting.Models.BasePageWithStepsModel" />
    [SitecoreType(TemplateId = "{C8C58D58-C5D9-47C2-AEF3-F4DEFCA62A2C}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class PageLoginModel : BasePageWithStepsModel
    {
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

        #region Settings

        [SitecoreField]
        public int MaxFailedAttempts { get; set; }

        [SitecoreField]
        public string DelayAfterFailedAttempts { get; set; }

        /// <summary>
        /// Gets the delay after failed attempts as <see cref="TimeSpan"/>
        /// </summary>
        /// <value>
        /// Parsed value from <see cref="DelayAfterFailedAttempts"/>. If parsing failed, return default value '00:15:00'
        /// </value>
        [SitecoreIgnore]
        public TimeSpan DelayAfterFailedAttemptsTimeSpan
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.DelayAfterFailedAttempts))
                {
                    TimeSpan value;

                    if (TimeSpan.TryParse(this.DelayAfterFailedAttempts, out value))
                    {
                        return value;
                    }
                }

                return new TimeSpan(0, 15, 0);
            }
        }

        #endregion

        #region 	AB Testing Settings

        /// <summary>
        /// Gets switch if placeholders of every matrix combination (like /eContracting2Main/eContracting2-login_02_K) for A/B testing headertexts should be added automatically when this page is opened in Edit mode.
        /// </summary>
        [SitecoreField]
        public bool AutoGenerateTestableCombinationPlaceholders { get; set; }


        #endregion
    }
}
