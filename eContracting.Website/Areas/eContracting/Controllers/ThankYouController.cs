﻿using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class ThankYouController : GlassController<EContractingThankYouTemplate>
    {
        public ActionResult ThankYou()
        {
            try
            {
                RweUtils utils = new RweUtils();
                RweClient client = new RweClient();

                if (!utils.IsUserInSession())
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                ViewData["MainText"] = SystemHelpers.GenerateMainText(Context.MainText);

                return View("/Areas/eContracting/Views/ThankYou.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying thank you page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}