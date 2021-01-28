using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Identifier for an offer.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class OfferCacheDataModel
    {
        /// <summary>
        /// Gets offer GUID identifier.
        /// </summary>
        public readonly string Guid;

        /// <summary>
        /// Gets offer's process.
        /// </summary>
        public readonly string Process;

        /// <summary>
        /// Gets offer's process type.
        /// </summary>
        public readonly string ProcessType;

        /// <summary>
        /// Determinates if offer is accepted or not.
        /// </summary>
        public readonly bool IsAccepted;

        /// <summary>
        /// Determinaes if offer is expired or not.
        /// </summary>
        public readonly bool IsExpired;

        /// <summary>
        /// The text parameters from offer.
        /// </summary>
        public readonly IDictionary<string, string> TextParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferCacheDataModel"/> class.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public OfferCacheDataModel(OfferModel offer)
        {
            this.Guid = offer.Guid;
            this.Process = offer.Process;
            this.ProcessType = offer.ProcessType;
            this.IsAccepted = offer.IsAccepted;
            this.IsExpired = offer.IsExpired;
            this.TextParameters = offer.TextParameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferCacheDataModel"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        public OfferCacheDataModel(string process, string processType)
        {
            this.Process = process;
            this.ProcessType = processType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferCacheDataModel"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        /// <param name="isAccepted">if set to <c>true</c> [is accepted].</param>
        /// <param name="textParameters">The text parameters.</param>
        public OfferCacheDataModel(string guid, string process, string processType, bool isAccepted, IDictionary<string, string> textParameters)
        {
            this.Guid = guid;
            this.Process = process;
            this.ProcessType = processType;
            this.IsAccepted = isAccepted;
            this.TextParameters = textParameters;
        }
    }
}
