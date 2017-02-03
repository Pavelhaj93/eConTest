using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;

namespace eContracting.Kernel.PipeLines
{
    public class ItemNotFoundPipeline : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            //Check whether Site is Econtracting Site or not
            if (args.StartPath != "/sitecore/content/eContracting/Site")
                return;
            // Do nothing if the item is actually found
            if (Sitecore.Context.Item != null || Sitecore.Context.Database == null)
                return;

            // all the icons and media library items 
            // for the sitecore client need to be ignored
            if (args.Url.FilePath.StartsWith("-"))
                return;

            // Get the 404 not found item in Sitecore.
            ID SiteId = new ID("{11685D5C-7EA3-4350-AB6B-DB0482D6764C}");
            Item notFoundPage = Sitecore.Context.Database.GetItem(SiteId);
            if (notFoundPage == null)
                return;

            // Switch to the 404 item
            Sitecore.Context.Item = notFoundPage;
        }

    }
}
