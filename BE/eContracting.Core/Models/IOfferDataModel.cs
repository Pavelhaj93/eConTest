using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public interface IOfferDataModel
    {
        /// <summary>
        /// The unique offer identifier.
        /// </summary>
        string Guid { get; }

        /// <summary>
        /// The process.
        /// </summary>
        string Process { get; }

        /// <summary>
        /// The process type.
        /// </summary>
        string ProcessType { get; }

        /// <summary>
        /// The text parameters from offer.
        /// </summary>
        IDictionary<string, string> TextParameters { get; }

        /// <summary>
        /// Determinates if offer is accepted or not.
        /// </summary>
        bool IsAccepted { get; }

        /// <summary>
        /// Determinaes if offer is expired or not.
        /// </summary>
        bool IsExpired { get; }

        /// <summary>
        /// Gets or sets state of the offer - <see cref="OfferModel.State"/>.
        /// </summary>
        string State { get; }

        /// <summary>
        /// Gets or sets birthday of offer owner - <see cref="OfferModel.Birthday"/>.
        /// </summary>
        string Birthday { get; }

        /// <summary>
        /// Gets partner number.
        /// </summary>
        /// <value>Contains only digits, or is empty.</value>
        string PartnerNumber { get; }

        /// <summary>
        /// Gets post number where living.
        /// </summary>
        string PostNumber { get; }

        /// <summary>
        /// Gets post number when metering.
        /// </summary>
        string PostNumberConsumption { get; }
    }
}
