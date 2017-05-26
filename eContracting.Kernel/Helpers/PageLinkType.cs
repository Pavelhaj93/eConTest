// <copyright file="PageLinkType.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Helpers
{
    /// <summary>
    /// Enumeration of page link types.
    /// </summary>
    public enum PageLinkType
    {
        /// <summary>
        /// Expired session.
        /// </summary>
        SessionExpired,

        /// <summary>
        /// User is blocked.
        /// </summary>
        UserBlocked,

        /// <summary>
        /// Offer is accepted.
        /// </summary>
        AcceptedOffer,

        /// <summary>
        /// Wrong url.
        /// </summary>
        WrongUrl,

        /// <summary>
        /// Offer expired.
        /// </summary>
        OfferExpired,

        /// <summary>
        /// Thank you link.
        /// </summary>
        ThankYou,

        /// <summary>
        /// System error link.
        /// </summary>
        SystemError
    }
}
