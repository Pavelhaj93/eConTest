﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;

namespace eContracting.Services
{
    public static class Extensions
    {
        public static void TimeSpent(this ILogger logger, ZCCH_CACHE_PUT model, ZCCH_CACHE_PUTResponse1 response, TimeSpan time)
        {
            logger.Info(model.IV_CCHKEY, $"{nameof(ZCCH_CACHE_PUT)} finished in " + time.ToString("hh\\:mm\\:ss\\:fff"));
        }

        public static void TimeSpent(this ILogger logger, ZCCH_CACHE_STATUS_SET model, TimeSpan time)
        {
            logger.Info(model.IV_CCHKEY, $"{nameof(ZCCH_CACHE_STATUS_SET)} finished in " + time.ToString("hh\\:mm\\:ss\\:fff"));
        }

        public static void TimeSpent(this ILogger logger, ZCCH_CACHE_GET model, TimeSpan time)
        {
            logger.Info(model.IV_CCHKEY, $"{nameof(ZCCH_CACHE_GET)} ({model.IV_CCHTYPE}) finished in " + time.ToString("hh\\:mm\\:ss\\:fff"));
        }

        public static void LogFiles(this ILogger logger, IEnumerable<OfferAttachmentModel> files, string guid, bool IsAccepted)
        {
            var accept = IsAccepted ? "accepted" : "not accepted";

            var builder = new StringBuilder();
            builder.Append($"Offer is {accept}");
            builder.AppendLine();

            if (files == null || !files.Any())
            {
                builder.AppendLine("0 files received");
                logger.Debug(guid, builder.ToString());
                return;
            }

            builder.AppendLine($"{files.Count()} files received:");
            var counter = 1;
            int totalSize = 0;

            foreach (var file in files)
            {
                builder.AppendLine($" - [{counter}] {file.OriginalFileName} {file.FileName} ({file.SizeLabel}) => group: {file.Group}, sign: {file.IsSignReq}, printed: {file.IsPrinted}");
                counter++;
                totalSize += file.Size;
            }

            builder.AppendLine(" - Total files size: " + Utils.GetReadableFileSize(totalSize));
            logger.Debug(guid, builder.ToString());
        }

        public static void LogFiles(this ILogger logger, IEnumerable<OfferFileXmlModel> files, string guid, bool IsAccepted, OFFER_TYPES type)
        {
            var accept = IsAccepted ? "accepted" : "not accepted";

            var builder = new StringBuilder();
            builder.Append($"[{Enum.GetName(typeof(OFFER_TYPES), type)}] ");
            builder.Append($"Offer is {accept}");
            builder.AppendLine();

            if (files == null || !files.Any())
            {
                builder.AppendLine("0 files received");
                logger.Debug(guid, builder.ToString());
                return;
            }

            builder.AppendLine($"{files.Count()} files received:");
            var counter = 1;

            foreach (var file in files)
            {
                builder.AppendLine($"[{counter}] {file.File.FILENAME}");
                counter++;
            }

            logger.Debug(guid, builder.ToString());
        }
    }
}
