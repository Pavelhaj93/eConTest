using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;
using rweClient.SerializationClasses;
using System.Globalization;

public partial class website_Website_WebControls_Authentication : System.Web.UI.UserControl
{
    public String DateOfBirth { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        // base.IsUserInSession();  ??

        RweClient client = new RweClient();

        var offer = client.GenerateXml("00145EE9475D1ED59CCDD59CA4BF0EB0");

        if ((offer == null) || (offer.Body == null) || String.IsNullOrEmpty(offer.Body.BIRTHDT))
        {
            throw new OfferIsNullException("Offer not found");
        }

        DateTime outputDateTimeValue;

        if (DateTime.TryParseExact(offer.Body.BIRTHDT, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTimeValue))
        {
            this.DateOfBirth = outputDateTimeValue.ToString("dd.MM.yyy");
        }
        else
        {
            throw new DateOfBirthWrongFormatException(String.Format("Wrong format: {0}", offer.Body.BIRTHDT));
        }

        AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage(offer);
        var authenticationData = authenticationDataSessionStorage.GetData();
        
        this.additional.Attributes["placeholder"] = "Vložte Vaše " + authenticationData.ItemFriendlyName;
        this.birth.Attributes["placeholder"] = "napr. 26. 12. 1966";
        
        var item = Sitecore.Context.Item;

        if (item != null)
        {
            this.firstTxt.Text = item["DateOfBirth"] ?? String.Empty;
            this.secondTxt.Text = item["ContractData"] ?? String.Empty;
            this.buttonText.Text = item["ButtonText"] ?? String.Empty;
        }

        this.DataBind();
    }
    protected void mainBtn_ServerClick(object sender, EventArgs e)
    {

    }
}
