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
                        var thisFile = files.FirstOrDefault(xx => xx.Index == file);
                        if (thisFile != null)
                        {
                            context.Response.Clear();
                            using (var ms = new MemoryStream(thisFile.FileContent.ToArray()))
                            {
                                context.Response.ContentType = "application/pdf";
                                context.Response.AddHeader("content-disposition", "attachment;filename=" + thisFile.FileName);
                                context.Response.AddHeader("Content-Length", thisFile.FileContent.Count.ToString());
                                context.Response.Buffer = true;
                                ms.WriteTo(context.Response.OutputStream);
                                context.Response.End();
                            }
                        }
                    }
                }
            }
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