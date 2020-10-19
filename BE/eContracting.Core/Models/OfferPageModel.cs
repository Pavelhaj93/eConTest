using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    /// <summary>
    /// Represents model for offer page.
    /// </summary>
    /// <seealso cref="BaseSitecoreModel" />
    [SitecoreType(TemplateId = "{456D5421-A2DE-42B4-97E6-A42FC243BF10}", AutoMap = true)]
    public class OfferPageModel : BaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        [SitecoreField]
        public virtual string PageTitle { get; set; }

        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        [SitecoreField]
        public virtual ProcessStepSitecoreModel Step { get; set; }

        /// <summary>
        /// Gets or sets the GDPR URL.
        /// </summary>
        [SitecoreField]
        public virtual string GDPRUrl { get; set; }

        /// <summary>
        /// Gets or sets the aes encrypt key.
        /// </summary>
        [SitecoreField]
        public virtual string AesEncryptKey { get; set; }

        /// <summary>
        /// Gets or sets the aes encrypt vector.
        /// </summary>
        [SitecoreField]
        public virtual string AesEncryptVector { get; set; }

        /// <summary>
        /// Gets or sets title for benefits.
        /// </summary>
        [SitecoreField]
        public virtual string BenefitsTitle { get; set; }

        /// <summary>
        /// Gets or sets title for documents for acceptance.
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptTitle { get; set; }

        /// <summary>
        /// Gets or sets label for address in documents for acceptance.
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptAddressLabel { get; set; }

        /// <summary>
        /// Gets or sets label for EAN in documents for acceptance..
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptEANLabel { get; set; }

        /// <summary>
        /// Gets or sets title for supplement and associated documents.
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptSupplementTitle { get; set; }

        /// <summary>
        /// Gets or sets subtitle for supplement and associated documents.
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptSupplementSubtitle { get; set; }

        /// <summary>
        /// Gets or sets title for power of attorney.
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptPowerTitle { get; set; }

        /// <summary>
        /// Gets or sets subtitle for power of attorney.
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptPowerSubtitle { get; set; }

        /// <summary>
        /// Gets or sets note for power of attorney.
        /// </summary>
        [SitecoreField]
        public virtual string DocumentsToAcceptPowerNote { get; set; }

        /// <summary>
        /// Gets or sets title for obligatory documents.
        /// </summary>
        [SitecoreField]
        public virtual string ObligatoryDocumentsTitle { get; set; }

        /// <summary>
        /// Gets or sets title for extra documents to upload.
        /// </summary>
        [SitecoreField]
        public virtual string ObligatoryDocumentsExtraAttachmentsTitle { get; set; }

        /// <summary>
        /// Gets or sets help text for extra documents to upload.
        /// </summary>
        [SitecoreField]
        public virtual string ObligatoryDocumentsExtraAttachmentsHelp { get; set; }

        /// <summary>
        /// Gets or sets note for extra documents to upload.
        /// </summary>
        [SitecoreField]
        public virtual string ObligatoryDocumentsLimitsNote { get; set; }

        /// <summary>
        /// Gets or sets title for additional services.
        /// </summary>
        [SitecoreField]
        public virtual string AdditionalServicesTitle { get; set; }

        /// <summary>
        /// Gets or sets subtitle for additional services.
        /// </summary>
        [SitecoreField]
        public virtual string AdditionalServicesSubtitle { get; set; }

        /// <summary>
        /// Gets or sets title for other services.
        /// </summary>
        [SitecoreField]
        public virtual string OtherServicesTitle { get; set; }

        /// <summary>
        /// Gets or sets subtitle for other services.
        /// </summary>
        [SitecoreField]
        public virtual string OtherServicesSubtitle { get; set; }

        /// <summary>
        /// Gets or sets title for other services and associated documents.
        /// </summary>
        [SitecoreField]
        public virtual string OtherServicesAssociatedDocumentsTitle { get; set; }

        /// <summary>
        /// Gets or sets subtitle for other services and associated documents.
        /// </summary>
        [SitecoreField]
        public virtual string OtherServicesAssociatedDocumentsSubtitle { get; set; }

        /// <summary>
        /// Gets or sets note for other services and associated documents.
        /// </summary>
        [SitecoreField]
        public virtual string OtherServicesAssociatedDocumentsNote { get; set; }

        /// <summary>
        /// Gets or sets title for accept offer.
        /// </summary>
        [SitecoreField]
        public virtual string AcceptOfferTitle { get; set; }

        /// <summary>
        /// Gets or sets subtitle for accept offer.
        /// </summary>
        [SitecoreField]
        public virtual string AcceptOfferSubtitle { get; set; }

        /// <summary>
        /// Gets or sets submit button label to accept offer.
        /// </summary>
        [SitecoreField]
        public virtual string AcceptOfferButtonLabel { get; set; }

        /// <summary>
        /// Gets or sets title for confirm modal window.
        /// </summary>
        [SitecoreField]
        public virtual string ConfirmModalWindowTitle { get; set; }

        /// <summary>
        /// Gets or sets text for confirm modal window.
        /// </summary>
        [SitecoreField]
        public virtual string ConfirmModalWindowText { get; set; }

        /// <summary>
        /// Gets or sets label for accept button in confirm modal window.
        /// </summary>
        [SitecoreField]
        public virtual string ConfirmModalWindowButtonAcceptLabel { get; set; }

        /// <summary>
        /// Gets or sets label for cancel button in confirm modal window.
        /// </summary>
        [SitecoreField]
        public virtual string ConfirmModalWindowButtonCancelLabel { get; set; }
    }
}
