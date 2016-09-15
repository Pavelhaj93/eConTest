using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using rweClient;

public partial class website_Website_layouts_DocumentsPanel : System.Web.UI.UserControl
{
    public String ClientId { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        HttpContext.Current.Session["docsReady"] = "0";

        AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
        var data = authenticationDataSessionStorage.GetData();

        RweClient client = new RweClient();
        var files = client.GeneratePDFFiles(data.Identifier);
        HttpContext.Current.Session["docsReady"] = "1";
        HttpContext.Current.Session["UserFiles"] = files;
    }
    protected void justLink_Click(object sender, EventArgs e)
    {
        LinkButton butt = sender as LinkButton;
        var b = butt.CommandArgument;

        var files = HttpContext.Current.Session["UserFiles"] as List<FileToBeDownloaded>;

        var thisFile = files.FirstOrDefault(xx => xx.FileNumber == b);

        if (thisFile != null)
        {
            Response.Clear();
            MemoryStream ms = new MemoryStream(thisFile.FileContent.ToArray());
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=labtest.pdf");
            Response.AddHeader("Content-Length", thisFile.FileContent.Count.ToString());
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }

        //AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
        //var data = authenticationDataSessionStorage.GetData();

        //RweClient client = new RweClient();
        //var files = client.GeneratePDFFiles(data.Identifier);

        //Response.Clear();
        //MemoryStream ms = new MemoryStream(files.ElementAt(1).FileContent.ToArray());
        //Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", "attachment;filename=labtest.pdf");
        //Response.AddHeader("Content-Length", files.ElementAt(1).FileContent.ToArray().Length.ToString());
        //Response.Buffer = true;
        //ms.WriteTo(Response.OutputStream);
        //Response.End();
    }
}