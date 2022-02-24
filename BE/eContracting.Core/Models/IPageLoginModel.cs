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
    /// <seealso cref="eContracting.Models.IBasePageWithStepsModel" />
    [SitecoreType(TemplateId = "{C8C58D58-C5D9-47C2-AEF3-F4DEFCA62A2C}", AutoMap = true)]
    public interface IPageLoginModel : IBasePageWithStepsModel
    {
        #region Form

        [SitecoreField]
        string BirthDateLabel { get; set; }

        [SitecoreField]
        string BirthDatePlaceholder { get; set; }

        [SitecoreField]
        string BirthDateValidationMessage { get; set; }

        [SitecoreField]
        string BirthDateHelpMessage { get; set; }       

        [SitecoreField]
        string VerificationMethodLabel { get; set; }

        [SitecoreField]
        string CalendarOpen { get; set; }

        [SitecoreField]
        string CalendarNextMonth { get; set; }

        [SitecoreField]
        string CalendarPreviousMonth { get; set; }

        [SitecoreField]
        string CalendarSelectDay { get; set; }

        [SitecoreField]
        string ButtonText { get; set; }

        [SitecoreField]
        string ValidationMessage { get; set; }

        [SitecoreField]
        string RequiredFields { get; set; }

        #endregion

        #region Extra Box

        [SitecoreField]
        IInfoBoxModel InfoLoginBox { get; set; }

        [SitecoreField]
        string WarningDontHaveAccess { get; set; }

        #endregion

        #region Settings

        [SitecoreField]
        int MaxFailedAttempts { get; set; }

        [SitecoreField]
        string DelayAfterFailedAttempts { get; set; }

        #endregion

        #region 	AB Testing Settings

        /// <summary>
        /// Gets switch if placeholders of every matrix combination (like /eContracting2Main/eContracting2-login_02_K) for A/B testing headertexts should be added automatically when this page is opened in Edit mode.
        /// </summary>
        [SitecoreField]
        bool AutoGenerateTestableCombinationPlaceholders { get; set; }


        #endregion

        #region Google Tag Manager Data

        string CampaignLabel { get; set; }

        string IndividualLabel { get; set; }

        string ElectricityLabel { get; set; }

        string GasLabel { get; set; }

        string LoginView_eCat { get; set; }

        string LoginView_eAct { get; set; }

        string LoginView_eLab { get; set; }

        string LoginClick_eCat { get; set; }

        string LoginClick_eAct { get; set; }

        string LoginClick_eLab { get; set; }

        #endregion
    }
}
