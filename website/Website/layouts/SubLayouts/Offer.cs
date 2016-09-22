using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;

public partial class website_Website_WebControls_Offer : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Sitecore.Context.PageMode.IsNormal)
        {
            RweUtils utils = new RweUtils();
            utils.IsUserInSession();

            AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
            var data = authenticationDataSessionStorage.GetData();

            RweClient client = new RweClient();
            var files = client.GeneratePDFFiles(data.Identifier);

            var mainTextField = Sitecore.Context.Item.Fields["MainText"];

            if (mainTextField != null && !String.IsNullOrEmpty(mainTextField.Value))
            {
                this.mainText.Text = mainTextField.Value.Replace("{0}", data.LastName);
            }
        }
        else
        {
            var mainTextField = Sitecore.Context.Item.Fields["MainText"];
            this.mainText.Text = mainTextField.Value;
        }

        this.DataBind();
    }
}