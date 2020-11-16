using eContracting.Models;
using eContracting.Services;

namespace eContracting
{
    /// <summary>
    /// Represents parser from <see cref="ZCCH_ST_FILE"/> to <see cref="OfferAttachmentModel"/> object.
    /// </summary>
    public interface IOfferAttachmentParserService
    {
        /// <summary>
        /// Parses raw <paramref name="files"/> into serializable format.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="files">The files.</param>
        /// <returns>Array of files.</returns>
        OfferAttachmentModel[] Parse(OfferModel offer, ZCCH_ST_FILE[] files);
    }
}
