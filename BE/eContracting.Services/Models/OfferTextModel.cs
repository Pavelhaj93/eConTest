using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Services.Models
{
    [Serializable]
    public class OfferTextModel
    {
        /// <summary>
        /// Gets or sets index.
        /// </summary>
        public readonly string Index;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public readonly string Name;

        /// <summary>
        /// Gets or sets text.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferTextModel"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        public OfferTextModel(string index, string name, string text)
        {
            this.Index = index;
            this.Name = name;
            this.Text = text;
        }
    }
}
