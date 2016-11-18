﻿using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Analytics;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class AuthenticationController : GlassController<EContractingAuthenticationTemplate>
    {
        [HttpGet]
        public ActionResult Authentication()
        {
            try
            {
                FillViewData();

                var dataModel = new AuthenticationModel();

                RweClient client = new RweClient();
                var guid = Request.QueryString["guid"];

                if (string.IsNullOrEmpty(guid))
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                var offer = client.GenerateXml(guid);
                if ((offer == null) || (offer.OfferInternal.Body == null) || string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage(offer);
                var authenticationData = authenticationDataSessionStorage.GetData();

                var text = client.GetTextsXml(authenticationData.Identifier);
                var letterXml = client.GetLetterXml(text);
                var salutation = client.GetAttributeText("CUSTTITLELET", letterXml);
                var email = client.GetAttributeText("CUSTEMAIL", letterXml);

                //if (!string.IsNullOrEmpty(email))
                //{
                //    Tracker.Current.Session.Identify(email);
                //}

                ViewData["MainText"] = this.Context.MainText.Replace("{SALUTATION}", salutation);
                ViewData["AdditionalPlaceholder"] = string.Format(this.Context.ContractDataPlaceholder, authenticationData.ItemFriendlyName);

                return View("/Areas/eContracting/Views/Authentication.cshtml", dataModel);
            }
            catch (Exception ex)
            {
                Log.Error("Error when authenticating user", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        [HttpPost]
        public ActionResult Authentication(AuthenticationModel authenticationModel)
        {
            try
            {
                FillViewData();

                if (!ModelState.IsValid)
                {
                    // fuj :(
                    return View("/Areas/eContracting/Views/Authentication.cshtml", authenticationModel);
                }

                var dobValue = authenticationModel.BirthDate.Trim().Replace(" ", string.Empty).ToLower();
                var additionalValue = authenticationModel.Additional.Trim().Replace(" ", string.Empty).ToLower();

                AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var authenticationData = authenticationDataSessionStorage.GetData();

                if (Session["NumberOfLogons"] == null)
                {
                    Session["NumberOfLogons"] = 0;
                }

                var numberOfLogonsBefore = (int)Session["NumberOfLogons"];

                if ((numberOfLogonsBefore < 3) && ((dobValue != authenticationData.DateOfBirth.Trim().Replace(" ", String.Empty).ToLower()) || (additionalValue != authenticationData.ItemValue.Trim().Replace(" ", String.Empty).ToLower())))
                {
                    var numberOfLogons = (int)Session["NumberOfLogons"];
                    Session["NumberOfLogons"] = ++numberOfLogons;

                    string url = Request.RawUrl;

                    if (Request.RawUrl.Contains("&error=validationError"))
                    {
                        url = Request.RawUrl;
                    }
                    else
                    {
                        url = Request.RawUrl + "&error=validationError";
                    }

                    return Redirect(url);
                }
                else if (numberOfLogonsBefore >= 3)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);
                }
                else
                {
                    if (authenticationData.IsAccountNumber)
                    {
                        if (!SystemHelpers.IsAccountNumberValid(authenticationData.ItemValue))
                        {
                            string url = Request.RawUrl;

                            if (Request.RawUrl.Contains("&error=validationError"))
                            {
                                url = Request.RawUrl;
                            }
                            else
                            {
                                url = Request.RawUrl + "&error=validationError";
                            }

                            return Redirect(url);
                        }
                    }

                    Session["NumberOfLogons"] = 0;

                    RweClient client = new RweClient();
                    var offer = client.GenerateXml(authenticationData.Identifier);

                    if (offer.OfferInternal.IsAccepted)
                    {
                        var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                        return Redirect(redirectUrl);
                    }

                    if (offer.OfferInternal.Body.OfferIsExpired)
                    {
                        var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.OfferExpired).Url;
                        return Redirect(redirectUrl);
                    }

                    return Redirect(Context.NextPageLink.Url);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error when authenticating user", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        private void FillViewData()
        {
            ViewData["FirstText"] = this.Context.DateOfBirth;
            ViewData["SecondText"] = this.Context.ContractData;
            ViewData["ButtonText"] = this.Context.ButtonText;
            ViewData["BirthDatePlaceholder"] = this.Context.DateOfBirthPlaceholder;

            ViewData["RequiredFields"] = this.Context.RequiredFields;
            ViewData["ValidationMessage"] = this.Context.ValidationMessage;
        }
    }
}