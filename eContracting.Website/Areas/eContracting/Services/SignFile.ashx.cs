using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;

namespace eContracting.Website.Areas.eContracting.Services
{
    /// <summary>
    /// Summary description for SignFile
    /// </summary>
    public class SignFile : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["UserFiles"] != null)
            {
                var pdfFile = this.GetPDfFile(context);
                var signFile = this.GetSignFile(context);

                if (pdfFile == null || signFile == null)
                {
                    return;
                }

                var signingClient = new SigningClient();
                var signingResult = signingClient.SendDocumentsForMerge(pdfFile.Content, signFile, pdfFile.FileType);
            }
        }

        private byte[] GetSignFile(HttpContext context)
        {
            var postedFiles = context.Request.Files;
            if (postedFiles == null)
            {
                return null;
            }

            var postedSignFile = postedFiles.OfType<HttpPostedFile>().FirstOrDefault();
            if (postedSignFile == null)
            {
                return null;
            }

            var signFile = null as byte[];
            using (var signFileReader = new BinaryReader(postedSignFile.InputStream))
            {
                signFile = signFileReader.ReadBytes(postedSignFile.ContentLength);
            }

            return signFile;
        }

        private PdfFile GetPDfFile(HttpContext context)
        {
            var pdfFileId = context.Request["pdfFileId"];

            var files = context.Session["UserFiles"] as List<FileToBeDownloaded>;
            if (files == null)
            {
                return null;
            }

            var pdfFile = files.FirstOrDefault(file => file.Index == pdfFileId);
            if (pdfFile == null)
            {
                return null;
            }

            return new PdfFile() { Content = pdfFile.FileContent.ToArray(), FileType = pdfFile.FileType };
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
