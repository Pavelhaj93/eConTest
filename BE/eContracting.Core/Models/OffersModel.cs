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
        /// <see cref="OfferModel.Version"/>
        public int Version => this.FirstOrDefault()?.Version ?? 0;

        /// <summary>
        /// Determinates if any offers has attribute <see cref="Constants.OfferAttributes.MCFU_REG_STAT"/>.
        /// </summary>
        /// <see cref="OfferModel.HasMcfu"/>
        public bool HasMcfu => this.Any(x => x.HasMcfu);

        /// <summary>
        /// Gets first guid from the collection.
        /// </summary>
        public string Guid => this.FirstOrDefault()?.Guid;

        /// <summary>
        /// Gets all guids from <see cref="OfferModel"/>s.
        /// </summary>
        public string[] Guids => this.Select(x => x.Guid).ToArray();

        /// <summary>
        /// Gets first <see cref="OfferModel.Process"/> from the collection.
        /// </summary>
        public string Process => this.FirstOrDefault()?.Process;

        /// <summary>
        /// Gets first <see cref="OfferModel.ProcessType"/> from the collection.
        /// </summary>
        public string ProcessType => this.FirstOrDefault()?.ProcessType;

        /// <summary>
        /// Gets merged text parameters from all <see cref="OfferModel.TextParameters"/>.
        /// </summary>
        public IDictionary<string, string> TextParameters { get { return Utils.Merge(this.Select(x => x.TextParameters).ToArray()); } }

        /// <summary>
        /// Gets <c>true</c> if all <see cref="OfferModel.IsAccepted"/> equals <c>true</c>, otherwise <c>false</c>.
        /// </summary>
        public bool IsAccepted => this.All(x => x.IsAccepted);

        /// <summary>
        /// Gets <c>true</c> if all <see cref="OfferModel.IsExpired"/> equals <c>true</c>, otherwise <c>false</c>.
        /// </summary>
        public bool IsExpired => this.All(x => x.IsExpired);

        /// <summary>
        /// Gets first <see cref="OfferModel.State"/> from the collection.
        /// </summary>
        public string State => this.FirstOrDefault()?.State;

        /// <summary>
        /// Gets first <see cref="OfferModel.Birthday"/> from the collection.
        /// </summary>
        public string Birthday => this.FirstOrDefault()?.Birthday;

        /// <summary>
        /// Gets first <see cref="OfferModel.PartnerNumber"/> from the collection.
        /// </summary>
        public string PartnerNumber => this.FirstOrDefault()?.PartnerNumber;

        /// <summary>
        /// Gets first <see cref="OfferModel.PostNumber"/> from the collection.
        /// </summary>
        public string PostNumber => this.FirstOrDefault()?.PostNumber;

        /// <summary>
        /// Gets first <see cref="OfferModel.PostNumberConsumption"/> from the collection.
        /// </summary>
        public string PostNumberConsumption => this.FirstOrDefault()?.PostNumberConsumption;

        /// <summary>
        /// Gets first EanOrAndEic from the collection.
        /// </summary>
        /// <see cref="OfferBodyXmlModel.EanOrAndEic"/>
        public string EanOrAndEic => this.FirstOrDefault()?.Xml.Content.Body.EanOrAndEic;

        /// <summary>
        /// Gets first PHONE from the collection.
        /// </summary>
        /// <see cref="OfferBodyXmlModel.PHONE"/>
        public string Phone => this.FirstOrDefault()?.Xml.Content.Body.PHONE;

        /// <summary>
        /// Gets <c>true</c> if any of <see cref="OfferModel.CanBeCanceled"/> equals <c>true</c>, otherwise <c>false</c>.
        /// </summary>
        public bool CanBeCanceled => this.Any(x => x.CanBeCanceled);

        /// <summary>
        /// Gets <c>true</c> if any of <see cref="OfferModel.IsCampaign"/> equals <c>true</c>, otherwise <c>false</c>.
        /// </summary>
        public bool IsCampaign => this.Any(x => x.IsCampaign);

        /// <summary>
        /// Gets first <see cref="OfferModel.Campaign"/> from the collection.
        /// </summary>
        public string Campaign => this.FirstOrDefault()?.Campaign;

        /// <summary>
        /// Gets first <see cref="OfferModel.CreatedAt"/> from the collection.
        /// </summary>
        public string CreatedAt => this.FirstOrDefault()?.CreatedAt;

        /// <summary>
        /// Gets first <see cref="OfferModel.GDPRKey"/> from the collection.
        /// </summary>
        public string GDPRKey => this.FirstOrDefault()?.GDPRKey;

        /// <summary>
        /// Gets <c>true</c> if any of <see cref="OfferModel.HasGDPR"/> equals <c>true</c>, otherwise <c>false</c>.
        /// </summary>
        public bool HasGDPR => this.Any(x => x.HasGDPR);

        /// <summary>
        /// Gets first <see cref="OfferModel.GdprIdentity"/> from the collection.
        /// </summary>
        public string GdprIdentity => this.FirstOrDefault()?.GdprIdentity;

        /// <summary>
        /// Gets first <see cref="OfferModel.ExpirationDate"/> from the collection.
        /// </summary>
        public DateTime ExpirationDate => this.FirstOrDefault().ExpirationDate;

        /// <summary>
        /// Gets <c>true</c> if all of <see cref="OfferModel.ShowPrices"/> equals <c>true</c>, otherwise <c>false</c>.
        /// </summary>
        public bool ShowPrices => this.All(x => x.ShowPrices);

        /// <summary>
        /// Gets all <see cref="OfferModel.Documents"/> from all offers.
        /// </summary>
        public OfferAttachmentXmlModel[] Documents => this.Where(x => x.Documents != null).SelectMany(x => x.Documents).ToArray();

        /// <inheritdoc/>
        public OffersModel() : base()
        {
        }

        /// <inheritdoc/>
        public OffersModel(OfferModel offer) : base(new[] { offer })
        {
            this.InitialGuid = offer.Guid;
        }

        /// <inheritdoc/>
        public OffersModel(IEnumerable<OfferModel> offers) : base(offers)
        {
            this.InitialGuid = offers?.FirstOrDefault()?.Guid;
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
    }
}
