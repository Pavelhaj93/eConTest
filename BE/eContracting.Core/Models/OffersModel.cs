using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Represents collection of <see cref="OfferModel"/>.
    /// </summary>
    public class OffersModel : List<OfferModel>, IOfferDataModel
    {
        /// <summary>
        /// Gets guid which were used as first in sequence.
        /// </summary>
        public readonly string InitialGuid;

        /// <summary>
        /// Gets version of the first offer in the collection.
        /// </summary>
        /// <value>Version of first offer in collection. If collection is empty, returns 0.</value>
        public int Version => this.FirstOrDefault()?.Version ?? 0;

        /// <summary>
        /// Determinates if any offers has attribute <see cref="Constants.OfferAttributes.MCFU_REG_STAT"/>.
        /// </summary>
        /// <see cref="OfferModel.HasMcfu"/>
        public bool HasMcfu => this.Any(x => x.HasMcfu);

        /// <summary>
        /// Gets first guid from the collection.
        /// </summary>
        public string Guid
        {
            get
            {
                return this.FirstOrDefault()?.Guid;
            }
        }

        public string Process => this.FirstOrDefault()?.Process;
        public string ProcessType => this.FirstOrDefault()?.ProcessType;
        public IDictionary<string, string> TextParameters { get; }
        public bool IsAccepted => this.All(x => x.IsAccepted);
        public bool IsExpired => this.All(x => x.IsExpired);
        public string State => this.FirstOrDefault()?.State;
        public string Birthday => this.FirstOrDefault()?.Birthday;
        public string PartnerNumber => this.FirstOrDefault()?.PartnerNumber;
        public string PostNumber => this.FirstOrDefault()?.PostNumber;
        public string PostNumberConsumption => this.FirstOrDefault()?.PostNumberConsumption;

        /// <inheritdoc/>
        protected OffersModel() : base()
        {
        }

        /// <inheritdoc/>
        public OffersModel(string initialGuid, IEnumerable<OfferModel> collection) : base(collection)
        {
            this.InitialGuid = initialGuid;
        }

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> from the collection.
        /// </summary>
        /// <param name="guid">The guid.</param>
        /// <returns>The offer or null.</returns>
        public OfferModel GetOffer(string guid)
        {
            return this.FirstOrDefault(x => x.Guid == guid);
        }

        public string GetState(string guid)
        {
            return this.FirstOrDefault(x => x.Guid == guid)?.State ?? string.Empty;
        }
    }
}
