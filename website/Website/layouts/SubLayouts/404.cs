﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;

public partial class website_Website_WebControls_404 : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var item = Sitecore.Context.Item;

        if (item != null)
        {
            this.MainText.Text = item["HeaderText"] ?? String.Empty;
            this.Text.Text = item["Text"] ?? String.Empty;
        }
    }
}