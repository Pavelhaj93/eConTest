using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop.Abstractions;
using eContracting.Models;
using eContracting.Services;

namespace eContracting.ConsoleClient
{
    public static class Utils
    {
        public static void CompareIdAttach(IConsole console, OfferModel offer, ZCCH_ST_FILE[] files)
        {
            for (int i = 0; i < offer.Documents.Length; i++)
            {
                var attachment = offer.Documents[i];

                if (!attachment.IsPrinted())
                {
                    console.WriteLineSuccess($"Attachment {attachment.IdAttach} not printed (upload)");
                    continue;
                }

                bool fileFound = false;

                for (int y = 0; y < files.Length; y++)
                {
                    var file = files[y];
                    var fileIdAttach = file.GetIdAttach();

                    if (fileIdAttach == attachment.IdAttach)
                    {
                        fileFound = true;
                    }
                }

                if (fileFound)
                {
                    console.WriteLineSuccess($"Attachment {attachment.IdAttach} found");
                }
                else
                {
                    console.WriteLineError($"Attachment {attachment.IdAttach} ({attachment.Description}) not found in files");
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var idattach = file.GetIdAttach();

                if (!offer.Documents.Any(x => x.IdAttach == idattach))
                {
                    console.WriteLineError($"File {file.FILENAME} doesn't exist in attachments (IDATTACH = {idattach})");
                }
            }
        }
    }
}
