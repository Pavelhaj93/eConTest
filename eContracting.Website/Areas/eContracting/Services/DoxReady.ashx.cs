using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Helpers;
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

                client.StoreFilesInDb(files);
                context.Response.ContentType = "application/json";

                var generalSettings = ConfigHelpers.GetGeneralSettings();

                if (files != null)
                {
                    List<FileItem> filesList = new List<FileItem>();

                    bool alreadyHaveFirst = false;

                    foreach (var f in files)
                    {
                        FileItem fi = new FileItem();
                        fi.Title = f.FileName;
                        fi.Label = alreadyHaveFirst ? generalSettings.IAmInformed : generalSettings.IAgree;
                        filesList.Add(fi);
                        alreadyHaveFirst = true;
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