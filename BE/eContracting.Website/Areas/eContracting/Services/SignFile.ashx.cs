using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Services;
using Sitecore.Diagnostics;
using System.Text;
using System.Net;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace eContracting.Website.Areas.eContracting.Services
{
    /// <summary>
    /// Summary description for SignFile
    /// </summary>
    public class SignFile : IHttpHandler, IRequiresSessionState
    {
        protected readonly ISettingsReaderService SettingsReaderService;

        public SignFile()
        {
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
        }

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
                        context.Response.Write("No file found for signature");
                        context.Response.TrySkipIisCustomErrors = true;
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
                    }

                    var signingClient = new SigningClient(this.SettingsReaderService);
                    FileToBeDownloaded signingResult = signingClient.SendDocumentsForMerge(pdfFile, signFile);

                    if (signingResult == null)
                    {
                        context.Response.Write("Document not signed");
                        context.Response.TrySkipIisCustomErrors = true;
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return;
                    }
                    else
                    {
                        this.AddOrReplaceSignedFile(context, signingResult);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        return;
                    }
                }
                else
                {
                    context.Response.Write("No files found");
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error occured during signing.", ex, this);
                context.Response.Write(ex.Message);
                context.Response.TrySkipIisCustomErrors = true;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private byte[] GetSignFile(HttpContext context)
        {
            var postedSignature = context.Request.Form["signature"];
            if (string.IsNullOrEmpty(postedSignature))
            {
                return null;
            }

            var base64 = postedSignature.Substring(postedSignature.IndexOf(",", StringComparison.Ordinal) + 1, postedSignature.Length - postedSignature.IndexOf(",", StringComparison.Ordinal) - 1);
            return Convert.FromBase64String(base64);
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
                file = availableFiles.FirstOrDefault(xx => xx.SignedVersion == false);
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
