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

        public static ErrorModel FileNotSigned(string errorMessage) => new ErrorModel("FSS-NS", $"File not signed: {errorMessage}");

        public static ErrorModel EmptySignedFile() => new ErrorModel("FSS-EF", $"Signed file is empty");

        public static ErrorModel UploadedFilesTotalSizeExceeded() => new ErrorModel("UPLOAD-TOTALSIZE", $"Uploaded files total size limit exceeded");

        public static ErrorModel UploadedFilesGroupSizeExceeded() => new ErrorModel("UPLOAD-GROUPSIZE", $"Uploaded file group size limit exceeded");
        public static ErrorModel UploadedFileUnrecognizedFormat() => new ErrorModel("UPLOAD-FORMAT", $"Unsupported file format");

        public static ErrorModel UploadFileError() => new ErrorModel("UPLOAD-ERROR", $"Error uploading file");
    }
}
