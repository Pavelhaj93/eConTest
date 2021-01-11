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
    /// <seealso cref="BasePageWithStepsModel" />
    [SitecoreType(TemplateId = "{456D5421-A2DE-42B4-97E6-A42FC243BF10}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class PageNewOfferModel : BasePageWithStepsModel
    {
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

        [SitecoreField]
        public virtual string ConfirmModalWindowGeneralErrorMessage { get; set; }

        [SitecoreField]
        public virtual string SignModalWindowTitle { get; set; }

        [SitecoreField]
        public virtual string SignModalWindowText { get; set; }

        [SitecoreField]
        public virtual string SignModalWindowThumbnailText { get; set; }

        [SitecoreField]
        public virtual string SignModalWindowPenArea { get; set; }

        [SitecoreField]
        public virtual string SignModalWindowConfirmButtonLabel { get; set; }

        [SitecoreField]
        public virtual string SignModalWindowClearButtonLabel { get; set; }

        [SitecoreField]
        public virtual string SignModalWindowGeneralErrorMessage { get; set; }
    }
}
