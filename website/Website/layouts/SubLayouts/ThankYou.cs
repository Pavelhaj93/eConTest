using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using rweClient;

/// <summary>
/// Summary description for ThankYou
/// </summary>
public partial class website_Website_WebControls_ThankYou : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RweClient client = new RweClient();
        client.TryCall();

        Sitecore.Data.Fields.LinkField linkField1 = Sitecore.Context.Item.Fields["Link1"];

        if (linkField1 != null)
        {
            this.firstLinkA.Attributes.Add("href", linkField1.Url);
            this.firstLinkText.Text = linkField1.Text;
        }

        Sitecore.Data.Fields.LinkField linkField2 = Sitecore.Context.Item.Fields["Link2"];

        if (linkField2 != null)
        {
            this.secondLinkA.Attributes.Add("href", linkField2.Url);
            this.secondLinkText.Text = linkField2.Text;
        }

        Sitecore.Data.Fields.LinkField linkField3 = Sitecore.Context.Item.Fields["Link3"];

        if (linkField3 != null)
        {
            this.thirdLinkA.Attributes.Add("href", linkField3.Url);
            this.thirdLinkText.Text = linkField3.Text;
        }
    }
}