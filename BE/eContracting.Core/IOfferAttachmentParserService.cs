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
        OfferAttachmentModel[] Parse(OfferModel offer, OfferFileXmlModel[] files);

        /// <summary>
        /// Determinates if <paramref name="template"/> is relevant for this <paramref name="file"/>.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="file">The file.</param>
        bool Equals(OfferAttachmentXmlModel template, OfferFileXmlModel file);

        /// <summary>
        /// Find a file which matches to its <paramref name="template"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="template">The template.</param>
        /// <param name="files">The files.</param>
        /// <returns>File or null.</returns>
        OfferFileXmlModel GetFileByTemplate(OfferModel offer, OfferAttachmentXmlModel template, OfferFileXmlModel[] files);

        /// <summary>
        /// Makes compatible offer templates and offer files.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="files">The files.</param>
        void MakeCompatible(OfferModel offer, OfferFileXmlModel[] files);
    }
}
