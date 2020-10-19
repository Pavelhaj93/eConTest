using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class OfferFileAttribute
    {
        public int Index { get; }
        public string Key { get; }
        public string Value { get; }

        public OfferFileAttribute(int index, string key, string value)
        {
            this.Index = index;
            this.Key = key;
            this.Value = value;
        }
    }
}
