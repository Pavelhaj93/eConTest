using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class website_Website_WebControls_AcceptedOffer : BaseRweControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        base.IsUserInSession();

        var item = Sitecore.Context.Item;

        if (item != null)
        {
            this.MainText.Text = item["MainText"] ?? String.Empty;
        }
    }
}