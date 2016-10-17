﻿using System.Collections.Generic;

public static class ZCCH_CACHE_GETResponseExtensions
{
    public static bool ThereAreFiles(this ZCCH_CACHE_GETResponse res)
    {
        return res != null && res.ET_FILES.IsNotNullOrEmpty();
    }
}

