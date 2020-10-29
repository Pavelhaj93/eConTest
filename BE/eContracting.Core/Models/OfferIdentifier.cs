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
    public class OfferIdentifier
    {
        /// <summary>
        /// Gets offer GUID identifier.
        /// </summary>
        public string Guid { get; }

        /// <summary>
        /// Gets offer's process.
        /// </summary>
        public string Process { get; }

        /// <summary>
        /// Gets offer's process type.
        /// </summary>
        public string ProcessType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferIdentifier"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        public OfferIdentifier(string process, string processType)
        {
            this.Process = process;
            this.ProcessType = processType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferIdentifier"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        public OfferIdentifier(string guid, string process, string processType)
        {
            this.Guid = guid;
            this.Process = process;
            this.ProcessType = processType;
        }
    }
}
