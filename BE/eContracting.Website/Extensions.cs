using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Sitecore.Mvc.Helpers;

namespace eContracting.Website
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static HtmlString eContractingPlaceholder(this SitecoreHelper helper, string placeholderName)
        {
            return helper.Placeholder(placeholderName);
        }

        public static eContractingHtmlHelper eContracting(this HtmlHelper htmlHelper)
        {
            return new eContractingHtmlHelper(htmlHelper);
        }

        public static NegotiatedContentResult<HttpError> InternalServerError(this ApiController controller, string message)
        {
            var errorMessage = new HttpError(message);
            return new NegotiatedContentResult<HttpError>(System.Net.HttpStatusCode.InternalServerError, errorMessage, controller);
        }

        public static NegotiatedContentResult<HttpError> InternalServerError(this ApiController controller, string message, Exception exception)
        {
            var errorMessage = new HttpError(exception, true);
            errorMessage.Message = message;
            return new NegotiatedContentResult<HttpError>(System.Net.HttpStatusCode.InternalServerError, errorMessage, controller);
        }

        public static NegotiatedContentResult<HttpError> ServiceUnavailable(this ApiController controller, string message)
        {
            var errorMessage = new HttpError(message);
            return new NegotiatedContentResult<HttpError>(System.Net.HttpStatusCode.ServiceUnavailable, errorMessage, controller);
        }

        public static NegotiatedContentResult<HttpError> ServiceUnavailable(this ApiController controller, string message, Exception exception)
        {
            var errorMessage = new HttpError(exception, true);
            errorMessage.Message = message;
            return new NegotiatedContentResult<HttpError>(System.Net.HttpStatusCode.ServiceUnavailable, errorMessage, controller);
        }
    }
}
