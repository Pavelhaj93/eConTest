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

        #region Modal windows

        [SitecoreField]
        IModalDialogSignOfferModel ModalWindowSignOffer { get; set; }

        [SitecoreField]
        IModalDialogAcceptOfferModel ModalWindowAcceptOffer { get; set; }

        [SitecoreField]
        IModalDialogSimpleModel ModalWindowUnfinishedOffer { get; set; }

        [SitecoreField]
        IModalDialogSimpleModel ModalWindowCancelOffer { get; set; }

        #endregion

        [SitecoreField]
        string BackToSummaryLinkLabel { get; set; }

        [SitecoreField]
        string StartAgainLinkLabel { get; set; }
    }
}
