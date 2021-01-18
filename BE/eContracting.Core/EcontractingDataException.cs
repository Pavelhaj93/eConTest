using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    [ExcludeFromCodeCoverage]
    public class EcontractingDataException : EcontractingCodeException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingDataException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public EcontractingDataException(ErrorModel error) : base(error)
        {
        }
    }
}
