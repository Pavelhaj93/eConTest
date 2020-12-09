using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// List of events for <see cref="IEventLogger"/>.
    /// </summary>
    public enum EVENT_NAMES
    {
        /// <summary>
        /// When user was logged in.
        /// </summary>
        LOGIN,
        /// <summary>
        /// When user signed a document.
        /// </summary>
        SIGN_DOCUMENT,
        /// <summary>
        /// When user uploaded an attachment.
        /// </summary>
        UPLOAD_ATTACHMENT,
        /// <summary>
        /// When user submitted an offer.
        /// </summary>
        SUBMIT_OFFER
    }
}
