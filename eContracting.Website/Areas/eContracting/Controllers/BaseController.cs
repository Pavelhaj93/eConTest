using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using Glass.Mapper.Sc.Web.Mvc;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Base class for all controllers.
    /// </summary>
    /// <typeparam name="T">Type of GlassMapper entity used as data model.</typeparam>
    public class BaseController<T> : GlassController<T> where T : class
    {
        /// <summary>
        /// Checks whether user is not blocked.
        /// </summary>
        /// <param name="guid">GUID of offer we are checking.</param>
        /// <returns>A value indicating whether user is blocked or not.</returns>
        protected bool CheckWhetherUserIsBlocked(string guid)
        {
            var siteSettings = ConfigHelpers.GetSiteSettings();
            var loginsCheckerClient = new LoginsCheckerClient(siteSettings.MaxFailedAttempts, siteSettings.DelayAfterFailedAttemptsTimeSpan);

            return !loginsCheckerClient.CanLogin(guid);
        }
    }
}
