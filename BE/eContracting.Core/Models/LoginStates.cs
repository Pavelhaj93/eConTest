using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public enum LoginStates
    {
        OFFER_NOT_FOUND,
        INVALID_GUID,
        USER_BLOCKED,
        OFFER_STATE_1,
        MISSING_BIRTHDAY,
        OK
    }
}
