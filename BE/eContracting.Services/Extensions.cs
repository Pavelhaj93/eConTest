using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.SAP;

namespace eContracting.Services
{
    public static class Extensions
    {
        public static void TimeSpent(this ILogger logger, ZCCH_CACHE_STATUS_SET model, TimeSpan time)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"[{model.IV_CCHKEY}] {nameof(ZCCH_CACHE_STATUS_SET)} finished in " + time.ToString("hh\\:mm\\:ss\\:fff"));
            logger.Debug(stringBuilder.ToString());
        }

        public static void TimeSpent(this ILogger logger, ZCCH_CACHE_GET model, TimeSpan time)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"[{model.IV_CCHKEY}] {nameof(ZCCH_CACHE_GET)} finished in " + time.ToString("hh\\:mm\\:ss\\:fff"));
            logger.Debug(stringBuilder.ToString());
        }

        public static void LogFiles(this ILogger logger, IEnumerable<AttachmentModel> files, string guid, bool IsAccepted)
        {
            var template = "[{0}] Offer:[{1}]{2} Generated PDF files(Documents to download):{3}{4}";
            var accept = IsAccepted ? "Accepted" : "Not Accepted";

            if (files == null || !files.Any())
            {
                var noFiles = string.Format(template, guid, accept, Environment.NewLine, "[No files generated]", Environment.NewLine);
                logger.Debug(noFiles);
                return;
            }

            var builder = new StringBuilder();
            var counter = 1;

            foreach (var file in files)
            {
                var line = string.Format("[{0}] Sign Required:[{1}] {2} {3}", counter++, file.SignRequired ? "Yes" : "No", file.FileName, Environment.NewLine);
                builder.Append(line);
            }

            var tmp = string.Format(template, guid, accept, Environment.NewLine, Environment.NewLine, builder.ToString());
            logger.Debug(tmp);
        }

        public static void LogFiles(this ILogger logger, IEnumerable<ZCCH_ST_FILE> files, string guid, bool IsAccepted, string responseType)
        {
            var template = "[{0}] Offer:[{1}] Response Type:[{2}]{3} Received files:{4}{5}";
            var accept = IsAccepted ? "Accepted" : "Not Accepted";

            if (files == null || !files.Any())
            {
                var noFiles = string.Format(template, guid, accept, responseType, Environment.NewLine, "No files received", Environment.NewLine);
                logger.Debug(noFiles);
                return;
            }

            var builder = new StringBuilder();
            var counter = 1;

            foreach (var file in files)
            {
                var line = string.Format("[{0}] {1}{2}", counter++, file.FILENAME, Environment.NewLine);
                builder.Append(line);
            }

            var res = string.Format(template, guid, accept, responseType, Environment.NewLine, Environment.NewLine, builder.ToString());
            logger.Debug(res);
        }
    }
}
