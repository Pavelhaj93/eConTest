using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Services.Models
{
    /// <summary>
    /// Authentication data model to log-in a visitor.
    /// </summary>
    [Serializable]
    public class AuthDataModel
    {
        /// <summary>
        /// The unique offer identifier.
        /// </summary>
        public readonly string Guid;

        /// <summary>
        /// Prevents a default instance of the <see cref="AuthDataModel"/> class from being created.
        /// </summary>
        private AuthDataModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthDataModel"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        public AuthDataModel(string guid)
        {
            this.Guid = guid;
        }
    }
}
