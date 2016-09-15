<%@ WebHandler Language="C#" Class="DoxReady" %>

using System;
using System.Web;

public class DoxReady : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";        
        if(context.Session["docsReady"] == "1")
        {
            context.Response.Write("1");
        } 
        else
        {
            context.Response.Write("0");
        }

        //context.Response.Write("1");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}