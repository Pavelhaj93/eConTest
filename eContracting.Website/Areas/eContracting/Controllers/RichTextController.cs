using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Content;
using eContracting.Kernel.Models;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Handles RichText component.
    /// </summary>
    public class RichTextController : BaseController<EContractingRichTextDatasource>
    {
        /// <summary>
        /// Base action of RichText component.
        /// </summary>
        /// <returns>Instance result.</returns>
        public ActionResult RichText()
        {
            var dataSource = this.DataSource;

            var richTextModel = new RichTextModel() { Datasource = dataSource };

            return View("/Areas/eContracting/Views/Content/RichText.cshtml", richTextModel);
        }
    }
}
