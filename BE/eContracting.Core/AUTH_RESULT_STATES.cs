using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    public enum AUTH_RESULT_STATES
    {
        SUCCEEDED,
        INVALID_BIRTHDATE,
        INVALID_BIRTHDATE_AND_VALUE,
        INVALID_BIRTHDATE_DEFINITION,
        INVALID_VALUE,
        INVALID_VALUE_FORMAT,
        INVALID_VALUE_DEFINITION,
        KEY_VALUE_MISMATCH,
        MISSING_LOGIN_TYPE,
        MISSING_VALUE,
        KEY_MISMATCH,
        NOT_PREAUTHENTICATED,
        CANNOT_READ_OFFER
    }
}
