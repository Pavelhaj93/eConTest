﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public enum AUTH_RESULT_STATES
    {
        SUCCEEDED,
        INVALID_BIRTHDATE,
        INVALID_PARTNER,
        INVALID_PARTNER_FORMAT,
        INVALID_ZIP1,
        INVALID_ZIP1_FORMAT,
        INVALID_ZIP2,
        INVALID_ZIP2_FORMAT,
        INVALID_VALUE,
        INVALID_VALUE_FORMAT,
        INVALID_VALUE_DEFINITION,
        KEY_VALUE_MISMATCH,
        KEY_MISMATCH,
    }
}
