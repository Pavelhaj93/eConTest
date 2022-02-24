using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// List of available authentication methods.
    /// </summary>
    public enum AUTH_METHODS
    {
        /// <summary>
        /// Not authorized yet.
        /// </summary>
        NONE,
        /// <summary>
        /// Classic login type with birth date and secret value.
        /// </summary>
        TWO_SECRETS,
        /// <summary>
        /// Cognite authentication method.
        /// </summary>
        COGNITO
    }
}
