using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using eContracting.Website.Areas.eContracting2.Models;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Mvc;
using Sitecore.Web.UI.HtmlControls;

namespace eContracting.Website
{
    public class eContractingHtmlHelper
    {
        protected readonly HtmlHelper HtmlHelper;

        public eContractingHtmlHelper(HtmlHelper htmlHelper)
        {
            this.HtmlHelper = htmlHelper;
        }

        public HtmlString ProcessTypeSwitcher()
        {
            if (!Sitecore.Context.PageMode.IsNormal)
            {
                var settings = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
                var allProcesses = settings.GetAllProcesses();
                var allProcessTypes = settings.GetAllProcessTypes();
                var dic = new NameValueCollection(HttpContext.Current.Request.QueryString);

                var process = dic[Constants.QueryKeys.PROCESS];
                var processType = dic[Constants.QueryKeys.PROCESS_TYPE];

                dic.Remove(Constants.QueryKeys.PROCESS);
                dic.Remove(Constants.QueryKeys.PROCESS_TYPE);

                var viewModel = new ProcessTypesSwitcherViewModel(process, processType, dic, allProcesses.ToArray(), allProcessTypes.ToArray());
                return this.HtmlHelper.Partial("/Areas/eContracting2/Views/Preview/ProcessTypesSwitcher.cshtml", viewModel);
            }

            return new HtmlString(string.Empty);
        }
    }
}
