using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Services;

namespace eContracting.Website
{
    /// <summary>
    /// Summary description for GetFile
    /// </summary>
    public class GetFile : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Request processing.
        /// </summary>
        /// <param name="context">Request context.</param>
        public void ProcessRequest(HttpContext context)
        {
            // DateTime functionbeginTime = DateTime.UtcNow;
            if (context.Session["UserFiles"] != null)
            {
                // DateTime sessionBeginTime = DateTime.UtcNow;
                var files = context.Session["UserFiles"] as List<FileToBeDownloaded>;
                // DateTime sessionEndTime = DateTime.UtcNow;
                if (files != null)
                {
                    var file = context.Request.QueryString["file"];
                    if (file != null)
                    {
                        var thisFile = files.FirstOrDefault(xx => xx.Index == file);
                        if (thisFile != null)
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
                    }
                }
                DateTime functionEndTime = DateTime.UtcNow;
                // var totalDownload = functionEndTime.Subtract(functionbeginTime).Seconds;
                // if (totalDownload > 3)
                // {
                // StringBuilder builder = new StringBuilder();
                // builder.AppendFormat("File download takes longer than 3 seconds");
                // builder.AppendLine();
                // builder.AppendFormat("total download : {0} seconds", totalDownload);
                // builder.AppendLine();
                // builder.AppendFormat("Get data from session took : {0} seconds", sessionEndTime.Subtract(sessionBeginTime).Seconds);
                // builder.AppendLine();
                // builder.AppendFormat("Write file to response took : {0} seconds", functionEndTime.Subtract(sessionEndTime).Seconds);

                // Log.Warn(builder.ToString(), this);
                // }

            }
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