using Glass.Mapper.Sc.Web.Mvc;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Base class for all controllers.
    /// </summary>
    /// <typeparam name="T">Type of GlassMapper entity used as data model.</typeparam>
    public class BaseController<T> : GlassController<T> where T : class
    {
    }
}
