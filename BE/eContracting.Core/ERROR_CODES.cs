using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    public static class ERROR_CODES
    {
        /// <summary>
        /// Unknown
        /// </summary>
        public static ErrorModel UNKNOWN { get; } = new ErrorModel("UNKNOWN", "Unknown error");

        /// <summary>
        /// Missing file in an offer.
        /// </summary>
        public static ErrorModel MissingFile(string idAttach) => new ErrorModel("OAPS-MF", $"File template with {Constants.FileAttributes.TYPE} '{idAttach}' doesn't match to any file");
    }
}
