using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Services
{
    /// <summary>
    /// Summary description for SignFile
    /// </summary>
    public class SignFile : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
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
                    var signingResult = signingClient.SendDocumentsForMerge(pdfFile, signFile);
                    this.AddOrReplaceSignedFile(context, signingResult);

                    context.Response.ContentType = "application/json";

                    if (signingResult == null)
                    {
                        context.Response.TrySkipIisCustomErrors = true;
                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        context.Response.StatusCode = 200;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error occured during signing.", ex, this);
                context.Response.TrySkipIisCustomErrors = true;
                context.Response.StatusCode = 404;
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

        private void AddOrReplaceSignedFile(HttpContext context, FileToBeDownloaded signingResult)
        {
            var pdfFileId = context.Request["file"];

            var files = context.Session["UserFiles"] as List<FileToBeDownloaded>;

            var existingSignedFile = files.FirstOrDefault(xx => xx.Index == pdfFileId && xx.SignedVersion == true);
            if (existingSignedFile != null)
            {
                files.Remove(existingSignedFile);
            }
            files.Add(signingResult);
        }

        private FileToBeDownloaded GetPDfFile(HttpContext context)
        {
            var pdfFileId = context.Request["file"];

            var files = context.Session["UserFiles"] as List<FileToBeDownloaded>;
            if (files == null)
            {
                return null;
            }

            var file = null as FileToBeDownloaded;
            var availableFiles = files.Where(xx => xx.Index == pdfFileId);

            if (availableFiles.Count() > 1)
            {
                file = availableFiles.FirstOrDefault(xx => xx.SignedVersion == true);
                if (file == null)
                {
                    file = availableFiles.FirstOrDefault();
                }
            }
            else
            {
                file = availableFiles.FirstOrDefault();
            }

            if (file == null)
            {
                return null;
            }

            return file;
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
