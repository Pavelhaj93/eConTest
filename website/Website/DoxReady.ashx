<%@ WebHandler Language="C#" Class="DoxReady" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
 
using rweClient;

public class FileItem
{
    [Newtonsoft.Json.JsonProperty("title")]
    public String Title { get; set; }

    [Newtonsoft.Json.JsonProperty("url")]
    public String Url { get; set; }
}

public class DoxReady : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest (HttpContext context) {
        var clientId = context.Request.QueryString["id"];

        if(clientId != null)
        {
            RweClient client = new RweClient();
            var files = client.GeneratePDFFiles(clientId);
            context.Session["UserFiles"] = files;
            context.Response.ContentType = "application/json";
            if(files != null)
            {
                List<FileItem> filesList = new System.Collections.Generic.List<FileItem>();

                foreach (var f in files)
                {
                    FileItem fi = new FileItem();
                    fi.Title = f.FileName;
                    filesList.Add(fi);
                }
                
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(filesList));
            }
            else
            {
                throw new Exception("No files found");
            }
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}