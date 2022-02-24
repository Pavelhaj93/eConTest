using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class OfferTokenModel
    {
        public string Guid { get; }

        public string Process { get; }

        public string ProcessType { get; }

        public bool IsAccepted { get; }

        public bool IsExpired { get; }

        public string State { get; }
    }
}
