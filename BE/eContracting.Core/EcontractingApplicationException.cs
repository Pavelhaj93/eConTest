using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Represents application error (invalid / incomplete data, another uncatched error).
    /// </summary>
    /// <seealso cref="eContracting.EcontractingCodeException" />
    [ExcludeFromCodeCoverage]
    public class EcontractingApplicationException : EcontractingCodeException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingApplicationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public EcontractingApplicationException(string message) : base(new ErrorModel("EAP", message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingApplicationException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public EcontractingApplicationException(ErrorModel error) : base(error)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingApplicationException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="innerException">The inner exception.</param>
        public EcontractingApplicationException(ErrorModel error, Exception innerException) : base(error, innerException)
        {
        }
    }
}
