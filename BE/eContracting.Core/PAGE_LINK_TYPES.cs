namespace eContracting
{
    /// <summary>
    /// Enumeration of page link types defined on <c>/sitecore/content/eContracting2</c>.
    /// </summary>
    public enum PAGE_LINK_TYPES
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
        /// The summary of the offer.
        /// </summary>
        Summary,

        /// <summary>
        /// The offer.
        /// </summary>
        Offer,

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
        SystemError,

        /// <summary>
        /// Login page.
        /// </summary>
        Login,

        /// <summary>
        /// Logout page.
        /// </summary>
        Logout,

        /// <summary>
        /// Upload page.
        /// </summary>
        Upload
    }
}
