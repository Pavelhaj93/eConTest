using eContracting.Models;
using eContracting.Services;

namespace eContracting.Services
{
    /// <summary>
    /// Represents parser from <see cref="ZCCH_ST_FILE"/> to <see cref="OfferAttachmentXmlModel"/> object.
    /// </summary>
    public interface IOfferAttachmentParserService
    {
        /// <summary>
        /// Creates <see cref="OfferAttachmentXmlModel"/> from given <paramref name="file"/>. <paramref name="offer"/> is needed to define some parameters.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="index">The index position from list of all files.</param>
        /// <param name="offer">The offer.</param>
        /// <returns>Instance of <see cref="OfferAttachmentXmlModel"/>.</returns>
        OfferAttachmentXmlModel Parse(ZCCH_ST_FILE file, int index, OfferModel offer);
    }
}
