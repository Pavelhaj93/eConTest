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

public partial class website_Website_layouts_DocumentsPanel : System.Web.UI.UserControl
{
    public String ClientId { get; set; }
    public Boolean IsButtonVisible { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Sitecore.Context.PageMode.IsNormal)
        {
            AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
            var data = authenticationDataSessionStorage.GetData();
            this.ClientId = data.Identifier;

            this.mainBtn.Visible = IsButtonVisible;
            this.DataBind();
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
        var data = authenticationDataSessionStorage.GetData();

        RweClient client = new RweClient();
        var offerSent = client.AcceptOffer(data.Identifier);

        Sitecore.Data.Fields.LinkField nextPageUrl = Sitecore.Context.Item.Fields["NextPageUrl"];

        if (nextPageUrl != null)
        {
            Response.Redirect(rweHelpers.GetPath(nextPageUrl), true);
        }
    }
}