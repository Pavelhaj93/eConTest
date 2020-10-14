using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public enum AuthResultState
    {
        SUCCEEDED,
        INVALID_BIRTHDATE,
        INVALID_PARTNER,
        INVALID_ZIP1,
        INVALID_ZIP2,
        KEY_VALUE_MISMATCH,
        KEY_MISMATCH,
    }
}
