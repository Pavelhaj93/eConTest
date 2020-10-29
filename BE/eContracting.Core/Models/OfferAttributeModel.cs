using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting.Models
{
    /// <summary>
    /// Represent an attribute anywhere in an offer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OfferAttributeModel
    {
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <seealso cref="ZCCH_ST_ATTRIB.ATTRINDX"/>
        public readonly int Index;

        /// <summary>
        /// The key.
        /// </summary>
        /// <seealso cref="ZCCH_ST_ATTRIB.ATTRID"/>
        public readonly string Key;

        /// <summary>
        /// The value.
        /// </summary>
        /// <seealso cref="ZCCH_ST_ATTRIB.ATTRVAL"/>
        public readonly string Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttributeModel"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public OfferAttributeModel(int index, string key, string value)
        {
            this.Index = index;
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttributeModel"/> class.
        /// </summary>
        /// <param name="attrib">The attribute.</param>
        public OfferAttributeModel(ZCCH_ST_ATTRIB attrib)
        {
            this.Index = Convert.ToInt32(attrib.ATTRINDX);
            this.Key = attrib.ATTRID;
            this.Value = attrib.ATTRVAL;
        }
    }
}
