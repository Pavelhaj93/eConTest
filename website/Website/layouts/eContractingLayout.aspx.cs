using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using rweClient;

public partial class website_Website_layouts_eContractingLayout : System.Web.UI.Page
{
    public static string SessionExpiredLink { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        Sitecore.Data.Database context = Sitecore.Context.Database;
        Sitecore.Data.Items.Item item = context.GetItem("/sitecore/content/MasterPage");

        if (item != null)
        {
            //header
            Sitecore.Data.Fields.LinkField linkField1 = item.Fields["ImageLink"];

            if (linkField1 != null)
            {
                this.imageLink.Attributes.Add("href", rweHelpers.GetPath(linkField1));
            }

            this.phoneLink.Attributes.Add("href", "tel:" + item["PhoneNumberLink"] ?? String.Empty);
            this.phoneLinkText.Text = item["PhoneNumber"] ?? String.Empty;

            //footer
            this.copyRightText.Text = item["CopyrightText"] ?? String.Empty;

            this.emailLink.Attributes.Add("href", "mailto:" + item["EmailLink"] ?? String.Empty);
            this.emailText.Text = item["EmailText"] ?? String.Empty;

            this.phoneLinkLower.Attributes.Add("href", "tel:" + item["PhoneNumberLinkFooter"] ?? String.Empty);
            this.phoneLinkTextLower.Text = item["PhoneNumberFooter"] ?? String.Empty;

            Sitecore.Data.Fields.LinkField disclaimerLink = item.Fields["DisclaimerLink"];

            if (disclaimerLink != null)
            {
                this.disclaimerLinkUrl.Attributes.Add("href", rweHelpers.GetPath(disclaimerLink));
                this.disclaimerText.Text = disclaimerLink.Text;
            }

            if (String.IsNullOrEmpty(RweUtils.RedirectSessionExpired))
            {
                //session expired
                Sitecore.Data.Fields.LinkField sessionExpiredLink = item.Fields["SessionExpired"];

                if (sessionExpiredLink != null)
                {
                    RweUtils.RedirectSessionExpired = rweHelpers.GetPath(sessionExpiredLink);
                }
            }
        }
    }
}