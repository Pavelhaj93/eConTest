using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class website_Website_WebControls_SessionExpired : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.headerText.Text = Sitecore.Context.Item["Header"];
        this.mainText.Text = Sitecore.Context.Item["MainText"];
    }
}