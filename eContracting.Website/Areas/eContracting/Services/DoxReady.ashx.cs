using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Services;
using Newtonsoft.Json;

namespace eContracting.Website
{
    /// <summary>
    /// Summary description for DoxReady
    /// </summary>
    public class DoxReady : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var clientId = context.Request.QueryString["id"];

            if (clientId != null)
            {
                RweClient client = new RweClient();
                var files = client.GeneratePDFFiles(clientId);
                context.Session["UserFiles"] = files;
                context.Response.ContentType = "application/json";
                if (files != null)
                {
                    List<FileItem> filesList = new List<FileItem>();

                    foreach (var f in files)
                    {
                        FileItem fi = new FileItem();
                        fi.Title = f.FileName;
                        filesList.Add(fi);
                    }

                    context.Response.Write(JsonConvert.SerializeObject(filesList));
                }
                else
                {
                    throw new Exception("No files found");
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