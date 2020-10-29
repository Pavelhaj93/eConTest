using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting.Models
{
    [ExcludeFromCodeCoverage]
    public class OfferHeaderModel
    {
        public readonly string CCHTYPE;
        public readonly string CCHKEY;
        public readonly string CCHSTAT;
        public readonly string CCHVALTO;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferHeaderModel"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        public OfferHeaderModel(ZCCH_ST_HEADER header)
        {
            this.CCHTYPE = header.CCHTYPE;
            this.CCHKEY = header.CCHKEY;
            this.CCHSTAT = header.CCHSTAT;
            this.CCHVALTO = header.CCHVALTO;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferHeaderModel"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="stat">The stat.</param>
        /// <param name="valto">The valto.</param>
        public OfferHeaderModel(string type, string key, string stat, string valto)
        {
            this.CCHTYPE = type;
            this.CCHKEY = key;
            this.CCHSTAT = stat;
            this.CCHVALTO = valto;
        }
    }
}
