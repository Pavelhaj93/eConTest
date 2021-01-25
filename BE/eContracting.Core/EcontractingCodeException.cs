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
    /// Represent abstract base error with custom code.
    /// </summary>
    /// <seealso cref="System.ApplicationException" />
    /// <seealso cref="ErrorModel"/>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingCodeException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="innerException">The inner exception.</param>
        public EcontractingCodeException(ErrorModel error, Exception innerException) : base(error.ToString(), innerException)
        {
                
        }
    }
}
