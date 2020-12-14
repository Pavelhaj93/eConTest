using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using eContracting.Website.Areas.eContracting2.Controllers;
using Sitecore.Pipelines;

namespace eContracting.Website.Pipelines.Initialize
{
    public class RegisterApiProcessor
    {
        public void Process(PipelineArgs args)
        {
            this.RegisterApiRoutes();
        }

        public void RegisterApiRoutes()
        {
            var controllerName = typeof(eContracting2ApiController).Name.Replace("Controller", "");
            Route route = RouteTable.Routes.MapHttpRoute(
                name: "Actum." + controllerName,
                routeTemplate: "api/econ/{action}/{id}",
                defaults: new { controller = controllerName, action = "Index", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("GET", "POST", "DELETE", "OPTIONS") });
            //route.RouteHandler = new SessionRouteHandler();
        }
    }

    class SessionRouteHandler : IRouteHandler
    {
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            return new SessionControllerHandler(requestContext.RouteData);
        }
    }

    class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public SessionControllerHandler(RouteData routeData) : base(routeData)
        {
        }
    }
}
