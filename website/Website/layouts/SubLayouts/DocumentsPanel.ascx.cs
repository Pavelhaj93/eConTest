using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Services;
using rweClient;

public class FileItem
{
    [Newtonsoft.Json.JsonProperty("title")]
    public String Title { get; set; }

    [Newtonsoft.Json.JsonProperty("url")]
    public String Url { get; set; }
}

public partial class website_Website_layouts_DocumentsPanel : System.Web.UI.UserControl
{
    public String ClientId { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Sitecore.Context.PageMode.IsNormal)
        {
            AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
            var data = authenticationDataSessionStorage.GetData();
            this.ClientId = data.Identifier;
            this.DataBind();
        }
    }
}