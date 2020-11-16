using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.SessionState;
using eContracting.Website.Areas.eContracting2.Models;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2ApiController : ApiController
    {
        protected readonly ILogger Logger;
        protected readonly IApiService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ISettingsReaderService SettingsReaderService;

        [ExcludeFromCodeCoverage]
        public eContracting2ApiController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
        }

        internal eContracting2ApiController(
            ILogger logger,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
        }

        [HttpGet]
        public IHttpActionResult Index()
        {
            return this.Ok();
        }

        [HttpGet]
        [Route("files")]
        public async Task<IHttpActionResult> Files()
        {
            // needs to check like this because info about it is only as custom session property :-(
            if (!this.AuthService.IsLoggedIn())
            {
                return this.StatusCode(HttpStatusCode.Unauthorized);
            }

            var user = this.AuthService.GetCurrentUser();
            var offer = await this.ApiService.GetOfferAsync(user.Guid);

            if (offer == null)
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }

            var attachments = await this.ApiService.GetAttachmentsAsync(offer);

            if ((attachments?.Length ?? 0) == 0)
            {
                return this.NotFound();
            }

            return this.Json(attachments);
            //return this.Json(attachments.Select(x => new FileAttachmentViewModel(x)));
        }

        [HttpGet]
        [Route("file/{id}")]
        public async Task<IHttpActionResult> File([FromUri]string id)
        {
            // needs to check like this because info about it is only as custom session property :-(
            if (!this.AuthService.IsLoggedIn())
            {
                return this.StatusCode(HttpStatusCode.Unauthorized);
            }

            var user = this.AuthService.GetCurrentUser();
            var offer = await this.ApiService.GetOfferAsync(user.Guid);

            if (offer == null)
            {
                return this.StatusCode(HttpStatusCode.NoContent);
            }

            var attachments = await this.ApiService.GetAttachmentsAsync(user.Guid);
            var file = attachments.FirstOrDefault(x => x.UniqueKey == id);

            if (file == null)
            {
                return this.NotFound();
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(file.FileContent);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.MimeType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = file.FileName;
            return this.ResponseMessage(response);
        }
    }
}
