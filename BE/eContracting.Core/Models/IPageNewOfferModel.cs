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
    /// Represents model for offer page.
    /// </summary>
    /// <seealso cref="IBasePageWithStepsModel" />
    [SitecoreType(TemplateId = "{456D5421-A2DE-42B4-97E6-A42FC243BF10}", AutoMap = true)]
    public interface IPageNewOfferModel : IBasePageWithStepsModel
    {
        /// <summary>
        /// Gets or sets the GDPR URL.
        /// </summary>
        [SitecoreField]
        string GDPRUrl { get; set; }

        /// <summary>
        /// Gets or sets the aes encrypt key.
        /// </summary>
        [SitecoreField]
        string AesEncryptKey { get; set; }

        /// <summary>
        /// Gets or sets the aes encrypt vector.
        /// </summary>
        [SitecoreField]
        string AesEncryptVector { get; set; }

        #region Confirm Modal Window

        /// <summary>
        /// Gets or sets title for confirm modal window.
        /// </summary>
        [SitecoreField]
        string ConfirmModalWindowTitle { get; set; }

        /// <summary>
        /// Gets or sets text for confirm modal window.
        /// </summary>
        [SitecoreField]
        string ConfirmModalWindowText { get; set; }

        /// <summary>
        /// Gets or sets label for accept button in confirm modal window.
        /// </summary>
        [SitecoreField]
        string ConfirmModalWindowButtonAcceptLabel { get; set; }

        /// <summary>
        /// Gets or sets label for cancel button in confirm modal window.
        /// </summary>
        [SitecoreField]
        string ConfirmModalWindowButtonCancelLabel { get; set; }

        [SitecoreField]
        string ConfirmModalWindowGeneralErrorMessage { get; set; }

        #endregion

        #region Sign Modal Window

        [SitecoreField]
        string SignModalWindowTitle { get; set; }

        [SitecoreField]
        string SignModalWindowText { get; set; }

        [SitecoreField]
        string SignModalWindowThumbnailText { get; set; }

        [SitecoreField]
        string SignModalWindowPenArea { get; set; }

        [SitecoreField]
        string SignModalWindowNote { get; set; }

        [SitecoreField]
        string SignModalWindowConfirmButtonLabel { get; set; }

        [SitecoreField]
        string SignModalWindowClearButtonLabel { get; set; }

        [SitecoreField]
        string SignModalWindowGeneralErrorMessage { get; set; }

        #endregion
    }
}
