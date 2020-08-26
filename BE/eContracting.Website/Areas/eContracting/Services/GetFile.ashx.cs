using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace eContracting.Website
{
    /// <summary>
    /// Summary description for GetFile
    /// </summary>
    public class GetFile : IHttpHandler, IRequiresSessionState
    {
        protected readonly ISettingsReaderService SettingsReaderService;

        public GetFile()
        {
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
        }

        /// <summary>
        /// Request processing.
        /// </summary>
        /// <param name="context">Request context.</param>
        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["UserFiles"] != null)
            {
                var files = context.Session["UserFiles"] as List<FileToBeDownloaded>;

                if (files != null)
                {
                    var file = context.Request.QueryString["file"];

                    if (file != null)
                    {
                        var thisFile = null as FileToBeDownloaded;
                        var availableFiles = files.Where(xx => xx.Index == file);

                        if (availableFiles.Count() > 1)
                        {
                            thisFile = availableFiles.FirstOrDefault(xx => xx.SignedVersion == true);

                            if (thisFile == null)
                            {
                                thisFile = availableFiles.FirstOrDefault();
                            }
                        }
                        else
                        {
                            thisFile = availableFiles.FirstOrDefault();
                        }

                        if (thisFile != null || thisFile.FileContent.Count > 0)
                        {
                            context.Response.Clear();

                            using (var ms = new MemoryStream(thisFile.FileContent.ToArray()))
                            {
                                context.Response.ContentType = "application/pdf";
                                context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename*=UTF-8''{0}", HttpUtility.UrlPathEncode(thisFile.FileName).Replace(",", "%2C")));
                                context.Response.AddHeader("Content-Length", thisFile.FileContent.Count.ToString());
                                context.Response.Buffer = true;
                                ms.WriteTo(context.Response.OutputStream);
                            }
                        }
                        else
                        {
                            this.SetNotFoundResponse(context);
                        }
                    }
                    else
                    {
                        this.SetNotFoundResponse(context);
                    }
                }
                else
                {
                    this.SetNotFoundResponse(context);
                }
            }
            else
            {
                this.SetNotFoundResponse(context);
            }
        }

        private void SetNotFoundResponse(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Status = "404 Not Found";
            context.Response.StatusCode = 404;
            context.Response.Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.SessionExpired).Url);
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the System.Web.IHttpHandler instance.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
