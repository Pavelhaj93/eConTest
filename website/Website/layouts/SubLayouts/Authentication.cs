using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using rweClient;
using rweClient.SerializationClasses;
using System.Globalization;

public partial class website_Website_WebControls_Authentication : BaseRweControl
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

        var item = Sitecore.Context.Item;

        if (item != null)
        {
            this.firstTxt.Text = item["DateOfBirth"] ?? String.Empty;
            this.secondTxt.Text = item["ContractData"] ?? String.Empty;
            this.buttonText.Text = item["ButtonText"] ?? String.Empty;
        }

        this.DataBind();
    }
}

public class AuthenticationDataItem
{
    public String ItemType { get; set; }
    public String ItemValue { get; set; }
}

public class AuthenticationDataSessionStorage
{
    private readonly String SessionKey = "AuthDataSession";

    public AuthenticationDataSessionStorage(rweClient.SerializationClasses.Offer offer)
    {
        if ((offer == null) || (offer.Body == null))
        {
            throw new OfferIsNullException("Offer is null by Session init");
        }

        if (!this.IsDataActive())
        {
            Random rnd = new Random();
            int value = rnd.Next(1, 4);

            AuthenticationDataItem authenticationDataItem = new AuthenticationDataItem();

            switch (value)
            {
                case 1:
                    authenticationDataItem.ItemType = "PARTNER";
                    authenticationDataItem.ItemValue = offer.Body.PARTNER;
                    break;

                case 2:
                    authenticationDataItem.ItemType = "PSC_MS";
                    authenticationDataItem.ItemValue = offer.Body.PscMistaSpotreby;
                    break;

                case 3:
                    authenticationDataItem.ItemType = "PSC_ADDR";
                    authenticationDataItem.ItemValue = offer.Body.PscTrvaleBydliste;
                    break;

                case 4:
                    authenticationDataItem.ItemType = "ACCOUNT_NUMBER";
                    authenticationDataItem.ItemValue = offer.Body.ACCOUNT_NUMBER;
                    break;
                default:
                    break;
            }
            HttpContext.Current.Session[SessionKey] = authenticationDataItem;
        }
    }

    public AuthenticationDataSessionStorage() { }

    public AuthenticationDataItem GetData()
    {
        if (HttpContext.Current.Session[SessionKey] != null)
        {
            var data = HttpContext.Current.Session[SessionKey] as AuthenticationDataItem;
            return data;
        }

        return null;
    }

    public Boolean IsDataActive()
    {
        return HttpContext.Current.Session[SessionKey] != null;
    }

    public void ClearSession()
    {
        HttpContext.Current.Session[SessionKey] = null;
    }
}

public abstract class BaseRweControl : System.Web.UI.UserControl
{
    protected AuthenticationDataSessionStorage authenticationDataSessionStorage { get; set; }

    protected void IsUserInSession()
    {
        if (authenticationDataSessionStorage == null)
        {
            authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
        }

        if (authenticationDataSessionStorage.IsDataActive())
        {
            return;
        }

        Response.Redirect("http://www.microsoft.com");
    }
}