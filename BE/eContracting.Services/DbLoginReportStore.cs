using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Services
{
    public class DbLoginReportStore : ILoginReportStore
    {
        public void AddFailedAttempt(string guid, string sessionId, string browserInfo)
        {
            //throw new NotImplementedException();
        }

        public void AddLoginAttempt(string sessionId, string timestamp, string guid, string type, bool birthdayDate = false, bool wrongPostalCode = false, bool WrongResidencePostalCode = false, bool WrongIdentityCardNumber = false, bool generalError = false)
        {
            //throw new NotImplementedException();
        }

        public bool CanLogin(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts)
        {
            return true;
            //throw new NotImplementedException();
        }

        public bool CanLogin(string guid)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}
