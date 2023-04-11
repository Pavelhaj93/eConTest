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
    public class OffersContainerModel : List<OfferModel>
    {
        /// <inheritdoc/>
        public OffersContainerModel()
        {
        }

        /// <inheritdoc/>
        public OffersContainerModel(IEnumerable<OfferModel> collection) : base(collection)
        {
        }
    }
}
