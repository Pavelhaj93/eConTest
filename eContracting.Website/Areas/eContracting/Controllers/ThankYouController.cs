﻿using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
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
                if (Sitecore.Context.PageMode.IsExperienceEditor)
                {
                    return View("/Areas/eContracting/Views/ThankYou.cshtml", Context);
                }

                RweUtils utils = new RweUtils();
                utils.IsUserInSession();

                AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var data = authenticationDataSessionStorage.GetData();
                try
                {
                    Context.MainText = string.Format(Context.MainText, data.LastName);
                }
                catch (Exception ex)
                {
                    Log.Warn("Error when processing users name pattern in thank you page", this);
                }

                return View("/Areas/eContracting/Views/ThankYou.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying thank you page", ex, this);
                return View("/Areas/eContracting/Views/SystemError.cshtml");
            }
        }
    }
}