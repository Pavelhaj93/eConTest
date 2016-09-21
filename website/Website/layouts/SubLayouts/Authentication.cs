using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;
using rweClient.SerializationClasses;
using System.Globalization;

public partial class website_Website_WebControls_Authentication : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // base.IsUserInSession();  ??
        if (!Page.IsPostBack || !AuthenticationDataSessionStorage.IsDataActiveStatic())
        {
            RweClient client = new RweClient();
            var offer = client.GenerateXml("005056BF704B1EE69C893F8B337F3D61");

            if ((offer == null) || (offer.Body == null) || String.IsNullOrEmpty(offer.Body.BIRTHDT))
            {
                throw new OfferIsNullException("Offer not found");
            }

            this.nameLit.Text = offer.Body.NAME_LAST;

            AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage(offer);
            var authenticationData = authenticationDataSessionStorage.GetData();

            this.additional.Attributes["placeholder"] = "Vložte Vaše " + authenticationData.ItemFriendlyName;
            this.birth.Attributes["placeholder"] = "napr. 26. 12. 1966";

            var item = Sitecore.Context.Item;

            if (item != null)
            {
                this.firstTxt.Text = item["DateOfBirth"] ?? String.Empty;
                this.secondTxt.Text = item["ContractData"] ?? String.Empty;
                this.mainBtn.Text = item["ButtonText"] ?? String.Empty;
            }

            this.DataBind();
        }
    }

    protected void mainBtn_ServerClick(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(this.birth.Text) || String.IsNullOrEmpty(additional.Text))
        {
            return;
        }

        var dobValue = this.birth.Text.Trim().Replace(" ", String.Empty).ToLower();
        var additionalValue = additional.Text.Trim().Replace(" ", String.Empty).ToLower();

        AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
        var authenticationData = authenticationDataSessionStorage.GetData();

        if (HttpContext.Current.Session["NumberOfLogons"] == null)
        {
            HttpContext.Current.Session["NumberOfLogons"] = 0;
        }

        var numberOfLogonsBefore = (int)HttpContext.Current.Session["NumberOfLogons"];

        if ((numberOfLogonsBefore < 3) && ((dobValue != authenticationData.DateOfBirth.Trim().Replace(" ", String.Empty).ToLower()) ||
            (additionalValue != authenticationData.ItemValue.Trim().Replace(" ", String.Empty).ToLower())))
        {
            var numberOfLogons = (int)HttpContext.Current.Session["NumberOfLogons"];
            HttpContext.Current.Session["NumberOfLogons"] = ++numberOfLogons;

            string url = Request.RawUrl;

            if (Request.RawUrl.Contains("?error=validationError"))
            {
                url = Request.RawUrl;
            }
            else
            {
                url = Request.RawUrl + "?error=validationError";
            }

            Response.Redirect(url);  //error same page
        }
        else if (numberOfLogonsBefore >= 3)
        {
            Response.Redirect(RweUtils.RedirectUserHasBeenBlocked); //block for 30 min page
        }
        else
        {
            HttpContext.Current.Session["NumberOfLogons"] = 0;
            
            var item = Sitecore.Context.Item;
            if (item != null)
            {
                Sitecore.Data.Fields.LinkField nextPageLink = item.Fields["NextPageLink"];

                if (nextPageLink != null)
                {
                    Response.Redirect(rweHelpers.GetPath(nextPageLink));
                }
            }
        }
    }
}
