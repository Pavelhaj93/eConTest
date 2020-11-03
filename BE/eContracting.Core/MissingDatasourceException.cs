﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    [ExcludeFromCodeCoverage]
    public class MissingDatasourceException : InvalidOperationException
    {
        public MissingDatasourceException(string message) : base(message)
        {
        }

        public MissingDatasourceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
