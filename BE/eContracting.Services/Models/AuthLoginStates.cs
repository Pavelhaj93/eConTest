using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Services.Models
{
    public enum AuthLoginStates
    {
        INVALID_GUID,
        USER_BLOCKED,
        NO_OFFER,
        EMPTY_OFFER,
        OFFER_STATE_1,
        MISSING_BIRTHDAY,
        OK
    }
}
