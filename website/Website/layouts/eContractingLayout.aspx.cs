using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class website_Website_layouts_eContractingLayout : System.Web.UI.Page
{
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
                this.imageLink.Attributes.Add("href", GetPath(linkField1));
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
                this.disclaimerLinkUrl.Attributes.Add("href", GetPath(disclaimerLink));
                this.disclaimerText.Text = disclaimerLink.Text;
            }
        }
    }

    private string GetPath(Sitecore.Data.Fields.LinkField lf)
    {
        switch (lf.LinkType.ToLower())
        {
            case "internal":
                // Use LinkMananger for internal links, if link is not empty
                return lf.TargetItem != null ? Sitecore.Links.LinkManager.GetItemUrl(lf.TargetItem).Replace("/en/", "/") : string.Empty;
            case "media":
                // Use MediaManager for media links, if link is not empty
                return lf.TargetItem != null ? Sitecore.Resources.Media.MediaManager.GetMediaUrl(lf.TargetItem) : string.Empty;
            case "external":
                return lf.Url;
            case "anchor":
                return !string.IsNullOrEmpty(lf.Anchor) ? "#" + lf.Anchor : string.Empty;
            case "mailto":
                return lf.Url;
            case "javascript":
                return lf.Url;
            default:
                return lf.Url;
        }
    }
}