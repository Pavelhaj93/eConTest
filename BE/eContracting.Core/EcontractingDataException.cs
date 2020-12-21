using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    public class EcontractingDataException : ApplicationException
    {
        /// <summary>
        /// The error.
        /// </summary>
        public readonly ErrorModel Error;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingDataException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="description">The description.</param>
        public EcontractingDataException(ERROR_CODES code, string description) : this(new ErrorModel(code, description))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcontractingDataException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public EcontractingDataException(ErrorModel error) : base(error.ToString())
        {
            this.Error = error;
        }
    }
}
