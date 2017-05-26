// <copyright file="ZCCH_CACHE_GETResponseExtensions.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

using System.Collections.Generic;

/// <summary>
/// Extensions for SAP proxy.
/// </summary>
public static class ZCCH_CACHE_GETResponseExtensions
{
    /// <summary>
    /// Checks if the response contains files.
    /// </summary>
    /// <param name="res">Response from SAP.</param>
    /// <returns>Retruns tru if response contains files.</returns>
    public static bool ThereAreFiles(this ZCCH_CACHE_GETResponse res)
    {
        return res != null && res.ET_FILES.IsNotNullOrEmpty();
    }
}

