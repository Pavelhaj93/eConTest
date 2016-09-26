using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;

public partial class website_Website_WebControls_AcceptedOffer : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Sitecore.Context.PageMode.IsNormal)
        {
            RweUtils utils = new RweUtils();
            utils.IsUserInSession();
        }

        var item = Sitecore.Context.Item;

        if (item != null)
        {
            this.MainText.Text = item["MainText"] ?? String.Empty;

            if (Sitecore.Context.PageMode.IsNormal)
            {
                AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var data = authenticationDataSessionStorage.GetData();
                this.MainText.Text = this.MainText.Text.Replace("{0}", data.LastName);
            }
        }
    }
}