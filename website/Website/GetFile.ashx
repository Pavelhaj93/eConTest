<%@ WebHandler Language="C#" Class="GetFile" %>

using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using rweClient;

public class GetFile : IHttpHandler, System.Web.SessionState.IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {    
        if(context.Session["UserFiles"] != null)
        {
            var files = context.Session["UserFiles"] as List<FileToBeDownloaded>;
            if(files != null)
            {
                 var file = context.Request.QueryString["file"];                
                 if(file != null)
                 {
                     var thisFile = files.FirstOrDefault(xx => xx.Index == file);
                     if(thisFile != null)
                     {
                         context.Response.Clear();
                         MemoryStream ms = new MemoryStream(files.ElementAt(1).FileContent.ToArray());
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

        //context.Response.ContentType = "text/plain";
        //context.Response.Write("Hello World");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}