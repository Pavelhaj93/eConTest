using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    [ExcludeFromCodeCoverage]
    public abstract class EcontractingCodeException : ApplicationException
    {
        /// <summary>
        /// The error.
        /// </summary>
        public readonly ErrorModel Error;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingCodeException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public EcontractingCodeException(ErrorModel error) : base(error.ToString())
        {
            this.Error = error;
        }
    }
}
