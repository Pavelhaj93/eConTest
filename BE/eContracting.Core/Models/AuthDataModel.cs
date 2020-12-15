using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Authentication data model to log-in a visitor.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class AuthDataModel
    {
        /// <summary>
        /// The unique offer identifier.
        /// </summary>
        public readonly string Guid;

        /// <summary>
        /// The process.
        /// </summary>
        public readonly string Process;

        /// <summary>
        /// The process type.
        /// </summary>
        public readonly string ProcessType;

        /// <summary>
        /// Prevents a default instance of the <see cref="AuthDataModel"/> class from being created.
        /// </summary>
        private AuthDataModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthDataModel"/> class.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public AuthDataModel(OfferModel offer)
        {
            this.Guid = offer.Guid;
            this.Process = offer.Process;
            this.ProcessType = offer.ProcessType;
        }
    }
}
