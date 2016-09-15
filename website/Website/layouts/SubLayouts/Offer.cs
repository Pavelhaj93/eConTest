using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;

public partial class website_Website_WebControls_Offer : System.Web.UI.UserControl
{
    public String ClientId { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Sitecore.Context.PageMode.IsNormal)
        {
            RweUtils utils = new RweUtils();
            utils.IsUserInSession();
        }

        AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
        var data = authenticationDataSessionStorage.GetData();

        var mainTextField = Sitecore.Context.Item.Fields["MainText"];

        if (mainTextField != null && !String.IsNullOrEmpty(mainTextField.Value))
        {
            this.mainText.Text = mainTextField.Value.Replace("{0}", data.LastName);
        }

        this.PanelDox.ClientId = data.Identifier;
        this.DataBind();
    }
}