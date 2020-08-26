using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Content;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Handles RichText component.
    /// </summary>
    public class WelcomeRichTextController : BaseController<EContractingWelcomeRichTextDatasource>
    {
        /// <summary>
        /// Base action of RichText component.
        /// </summary>
        /// <returns>Instance result.</returns>
        [Obsolete("Use 'eContracting2Controller.RichText' instead")]
        public ActionResult RichText()
        {
            var dataSource = this.DataSource;

            if (Sitecore.Context.PageMode.IsNormal)
            {
                var processingParameters = this.HttpContext.Items["WelcomeData"] as IDictionary<string, string>;
                if (processingParameters == null)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                var replacedText = SystemHelpers.ReplaceParameters(dataSource.Text, processingParameters);

                var richTextModel = new WelcomeRichTextModel() { Datasource = dataSource, ReplacedText = replacedText };

                return View("/Areas/eContracting/Views/Content/WelcomeRichText.cshtml", richTextModel);
            }
            else
            {
                var richTextModel = new WelcomeRichTextModel() { Datasource = dataSource, ReplacedText = dataSource.Text };

                return View("/Areas/eContracting/Views/Content/WelcomeRichText.cshtml", richTextModel);
            }
        }
    }
}
