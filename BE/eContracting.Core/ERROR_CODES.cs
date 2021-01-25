using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;

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

        public static ErrorModel FromResponse(string code, ZCCH_CACHE_PUTResponse response)
        {
            var msg = new StringBuilder();
            msg.AppendLine("ZCCH_CACHE_PUT request failed");
            msg.AppendLine(" EV_RETCODE: " + response.EV_RETCODE);
            msg.AppendLine(GetDescription(response.ET_RETURN));
            return new ErrorModel(code, msg.ToString());
        }

        public static ErrorModel FromResponse(string code, ZCCH_CACHE_STATUS_SETResponse response)
        {
            var msg = new StringBuilder();
            msg.AppendLine("ZCCH_CACHE_STATUS_SET request failed");
            msg.AppendLine("EV_RETCODE: " + response.EV_RETCODE);
            msg.AppendLine(GetDescription(response.ET_RETURN));
            return new ErrorModel(code, msg.ToString());
        }

        private static string GetDescription(BAPIRET2[] descriptions)
        {
            var msg = new StringBuilder();

            if (descriptions?.Length > 0)
            {
                var et = descriptions.First();
                msg.AppendLine(" BAPIRET2:");
                msg.AppendLine("  ID: " + et.ID);
                msg.AppendLine("  MESSAGE: " + et.MESSAGE);
                msg.AppendLine("  MESSAGE_V1: " + et.MESSAGE_V1);
                msg.AppendLine("  MESSAGE_V2: " + et.MESSAGE_V2);
                msg.AppendLine("  MESSAGE_V3: " + et.MESSAGE_V3);
                msg.AppendLine("  MESSAGE_V4: " + et.MESSAGE_V4);
                msg.AppendLine("  FIELD: " + et.FIELD);
                msg.AppendLine("  NUMBER: " + et.NUMBER);
                msg.AppendLine("  PARAMETER: " + et.PARAMETER);
                msg.AppendLine("  LOG_NO: " + et.LOG_NO);
                msg.AppendLine("  LOG_MSG_NO: " + et.LOG_MSG_NO);
                msg.AppendLine("  ROW: " + et.ROW);
                msg.AppendLine("  SYSTEM: " + et.SYSTEM);
                msg.AppendLine("  TYPE: " + et.TYPE);
            }

            return msg.ToString();
        }
    }
}
