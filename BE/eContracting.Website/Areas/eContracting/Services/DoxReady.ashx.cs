using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;

namespace eContracting.Website
{
    /// <summary>
    /// Summary description for DoxReady
    /// </summary>
    public class DoxReady : IHttpHandler, IRequiresSessionState
    {
        protected readonly IRweClient Client;
        protected readonly ISettingsReaderService SettingsReaderService;
        public DoxReady()
        {
            this.Client = ServiceLocator.ServiceProvider.GetRequiredService<IRweClient>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
        }

        /// <summary>
        /// Request processing.
        /// </summary>
        /// <param name="context">Request context.</param>
        public void ProcessRequest(HttpContext context)
        {
            var clientId = context.Request.QueryString["id"];

            if (clientId != null)
            {
                var files = this.Client.GeneratePDFFiles(clientId);
                context.Session["UserFiles"] = files;
                context.Response.ContentType = "application/json";
                var generalSettings = this.SettingsReaderService.GetGeneralSettings();

                if (files != null)
                {
                    List<FileItem> filesList = new List<FileItem>();

                    bool alreadyHaveFirst = false;

                    foreach (FileToBeDownloaded f in files)
                    {
                        FileItem fi = new FileItem();
                        fi.Number = f.FileNumber;
                        fi.Title = f.FileName;
                        fi.SignRequired = f.SignRequired;
                        fi.Label = alreadyHaveFirst ? generalSettings.IAmInformed : generalSettings.IAgree;
                        filesList.Add(fi);
                        alreadyHaveFirst = true;
                    }

                    context.Response.Write(JsonConvert.SerializeObject(filesList));
                }
                else
                {
                    Log.Fatal($"[{clientId}] No files found", this);
                    throw new Exception("No files found");
                }
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
