using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using rweClient;
using System.IO;

public partial class website_Website_WebControls_Offer : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Sitecore.Context.PageMode.IsNormal)
        {
            RweUtils utils = new RweUtils();
            utils.IsUserInSession();

            AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
            var data = authenticationDataSessionStorage.GetData();

            RweClient client = new RweClient();
            var text = client.GetTextsXml(data.Identifier);
            if (text != null && text.Any())
            {
                TextReader tr = new StringReader(text.First().Text);
                XDocument doc = XDocument.Load(tr);
                var textNode = doc.Descendants("BODY").FirstOrDefault();

                if (textNode != null)
                {
                    var els = textNode.FirstNode;
                    if (els != null)
                    {
                        var offerSubText = els as XElement;
                        var mainOfferText = offerSubText.Elements().FirstOrDefault();

                        if (mainOfferText != null)
                        {
                            this.mainText.Text = mainOfferText.ToString();
                        }
                    }
                }
            }
        }
        this.DataBind();
    }
}