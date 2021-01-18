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
    public class EcontractingSignException : EcontractingCodeException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingSignException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public EcontractingSignException(ErrorModel error) : base(error)
        {
        }
    }
}
