using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class website_Website_WebControls_CookieLaw : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Sitecore.Data.Database context = Sitecore.Context.Database;
        Sitecore.Data.Items.Item item = context.GetItem("/sitecore/content/CookieLaw");

        this.mainText.Text = item["MainText"] ?? String.Empty;

        Sitecore.Data.Fields.LinkField linkField1 = item.Fields["Link"];

        if (linkField1 != null)
        {
            this.PersonalDataLink.Attributes.Add("href", linkField1.Url);
            this.PersonalDataText.Text = linkField1.Text;
        }

        this.AgreeButton.Text = item["ButtonText"] ?? String.Empty;
    }
}