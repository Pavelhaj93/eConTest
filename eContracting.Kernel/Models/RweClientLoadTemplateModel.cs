using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Kernel.Models
{
    /// <summary>
    /// Represents model for loading xml sources with attribute "TEMPLATE" in <see cref="Kernel.Services.RweClient"/>.
    /// </summary>
    public class RweClientLoadTemplateModel
    {
        /// <summary>
        /// Gets or sets the identifier, for example "EED", "EPD", "EPS" or "EES".
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Identifier"/> was found inside iteration when processing templates.
        /// </summary>
        public bool StopWhenFound { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RweClientLoadTemplateModel"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public RweClientLoadTemplateModel(string identifier)
        {
            this.Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RweClientLoadTemplateModel"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="stopWhenFound">if set to <c>true</c> [stop when found].</param>
        public RweClientLoadTemplateModel(string identifier, bool stopWhenFound) : this(identifier)
        {
            this.StopWhenFound = stopWhenFound;
        }
    }
}
