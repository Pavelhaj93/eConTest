using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;

public partial class website_Website_WebControls_ThankYou : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Sitecore.Context.PageMode.IsNormal)
        {
            RweUtils utils = new RweUtils();
            utils.IsUserInSession();
        }

        Sitecore.Data.Fields.LinkField linkField1 = Sitecore.Context.Item.Fields["Link1"];

        if (linkField1 != null)
        {
            this.firstLinkA.Attributes.Add("href", rweHelpers.GetPath(linkField1));
            this.firstLinkText.Text = linkField1.Text;
        }

        Sitecore.Data.Fields.LinkField linkField2 = Sitecore.Context.Item.Fields["Link2"];

        if (linkField2 != null)
        {
            this.secondLinkA.Attributes.Add("href", rweHelpers.GetPath(linkField2));
            this.secondLinkText.Text = linkField2.Text;
        }

        Sitecore.Data.Fields.LinkField linkField3 = Sitecore.Context.Item.Fields["Link3"];

        if (linkField3 != null)
        {
            this.thirdLinkA.Attributes.Add("href", rweHelpers.GetPath(linkField3));
            this.thirdLinkText.Text = linkField3.Text;
        }
    }
}