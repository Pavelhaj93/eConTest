using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Authentication
/// </summary>
public partial class website_Website_WebControls_Authentication: System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var item = Sitecore.Context.Item;

        if (item != null)
        {
            this.firstTxt.Text = item["DateOfBirth"] ?? String.Empty;
            this.secondTxt.Text = item["ContractData"] ?? String.Empty;
            this.buttonText.Text = item["ButtonText"] ?? String.Empty;
        }
    }
}