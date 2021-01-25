using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    /// <inheritdoc/>
    /// <seealso cref="eContracting.ILoginFailedAttemptBlockerStore" />
    public class DbLoginFailedAttemptBlockerStore : ILoginFailedAttemptBlockerStore
    {
        /// <inheritdoc/>
        public void Add(LoginFailureModel loginAttemptModel)
        {
            //TODO: TBI
            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsAllowed(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts)
        {
            //TODO: TBI
            return true;
            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Clear(string guid)
        {
            //TODO: TBI
            //throw new NotImplementedException();
        }
    }
}
