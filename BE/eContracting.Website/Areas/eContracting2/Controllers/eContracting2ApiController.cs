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
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2ApiController : ApiController, IRequiresSessionState
    {
        protected readonly ILogger Logger;
        protected readonly ISitecoreContext Context;
        protected readonly IApiService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IOfferJsonDescriptor OfferJsonDescriptor;

        [ExcludeFromCodeCoverage]
        public eContracting2ApiController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.Context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.OfferJsonDescriptor = ServiceLocator.ServiceProvider.GetRequiredService<IOfferJsonDescriptor>();
        }

        internal eContracting2ApiController(
            ILogger logger,
            ISitecoreContext context,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService,
            IOfferJsonDescriptor offerJsonDescriptor)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.OfferJsonDescriptor = offerJsonDescriptor ?? throw new ArgumentNullException(nameof(offerJsonDescriptor));
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        [HttpDelete]
        public async Task<IHttpActionResult> Index()
        {
            return await Task.FromResult(this.Ok("Hello " + this.Request.Method.Method));
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

        /// <summary>
        /// Download requested file from an offer.
        /// </summary>
        /// <param name="id">The identifier.</param>
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

            var attachments = await this.ApiService.GetAttachmentsAsync(offer);
            var file = attachments.FirstOrDefault(x => x.UniqueKey == id);

            if (file == null)
            {
                return this.NotFound();
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(file.FileContent);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.MimeType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = file.FileNameExtension;
            return this.ResponseMessage(response);
        }

        /// <summary>
        /// Get JSON model for non accepted offer.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("offer")]
        public async Task<IHttpActionResult> Offer()
        {
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

            if (offer.IsAccepted)
            {
                return this.BadRequest();
            }

            var model = await this.OfferJsonDescriptor.GetNewAsync(offer);
            return this.Json(model);
        }

        /// <summary>
        /// Get JSON model for accepted offer.
        /// </summary>
        [HttpGet]
        [Route("accepted")]
        public async Task<IHttpActionResult> Accepted()
        {
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

            if (!offer.IsAccepted)
            {
                return this.BadRequest();
            }

            var model = await this.OfferJsonDescriptor.GetAcceptedAsync(offer);
            return this.Json(model);
        }

        /// <summary>
        /// Uploads file and returns actual group state.
        /// </summary>
        /// <param name="id">Group identifier.</param>
        /// <returns><see cref="GroupUploadViewModel"/> with current status of uploaded files. Failed uploaded file is not presented.</returns>
        [HttpPost]
        [Route("upload/{id}")]
        public async Task<IHttpActionResult> Upload([FromUri] string id)
        {
            return await Task.FromResult(this.Ok(new GroupUploadViewModel()));
        }

        /// <summary>
        /// Deletes uploaded file from group and returns actual group state.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns><see cref="GroupUploadViewModel"/> with current status of related files.</returns>
        [HttpDelete]
        [HttpOptions]
        [Route("upload/{id}")]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            if (this.Request.Method.Method == "OPTIONS")
            {
                return this.Ok();
            }

            return await Task.FromResult(this.Ok("Will be deleted in the future"));
        }

        /// <summary>
        /// Submit an offer (model not defined yet).
        /// </summary>
        [HttpPost]
        [Route("offer")]
        public async Task<IHttpActionResult> Submit()
        {
            return this.StatusCode(HttpStatusCode.NotImplemented);
        }
    }
}
