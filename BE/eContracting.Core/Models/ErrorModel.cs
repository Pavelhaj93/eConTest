using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Error description with it's unique code.
    /// </summary>
    public struct ErrorModel
    {
        /// <summary>
        /// The code.
        /// </summary>
        public readonly string Code;

        /// <summary>
        /// The description.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorModel"/> struct.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="description">The description.</param>
        public ErrorModel(ERROR_CODES code, string description)
        {
            this.Code = Enum.GetName(typeof(ERROR_CODES), code);
            this.Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorModel"/> struct.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="description">The description.</param>
        public ErrorModel(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        /// <summary>
        /// Converts to string in format '[<see cref="Code"/>] <see cref="Description"/>'.
        /// </summary>
        public override string ToString()
        {
            var str = new StringBuilder();

            str.Append($"[{this.Code}]");

            if (!string.IsNullOrEmpty(this.Description))
            {
                str.Append(" " + this.Description);
            }

            return str.ToString();
        }
    }
}
