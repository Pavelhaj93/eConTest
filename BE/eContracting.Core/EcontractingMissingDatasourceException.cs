using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// Represents missing datasouce.
    /// </summary>
    /// <seealso cref="System.InvalidOperationException" />
    [ExcludeFromCodeCoverage]
    public class EcontractingMissingDatasourceException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingMissingDatasourceException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EcontractingMissingDatasourceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingMissingDatasourceException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
        public EcontractingMissingDatasourceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
