using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Represents sign service for documents.
    /// </summary>
    public interface ISignService
    {
        /// <summary>
        /// Sign specifi <paramref name="file"/> with <paramref name="signature"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="file">The file.</param>
        /// <param name="signature">The signature.</param>
        /// <returns>New attachment model cloned from <paramref name="file"/> with new <see cref="OfferAttachmentModel.FileContent"/>.</returns>
        OfferAttachmentModel Sign(OfferModel offer, OfferAttachmentModel file, byte[] signature);
    }
}
