using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class website_Website_WebControls_Offer : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var mainTextField = Sitecore.Context.Item.Fields["MainText"];

        if (mainTextField != null && !String.IsNullOrEmpty(mainTextField.Value))
        {
            this.mainText.Text = mainTextField.Value;
        }
    }
}