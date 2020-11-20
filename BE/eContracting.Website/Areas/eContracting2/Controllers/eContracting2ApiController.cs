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

        [ExcludeFromCodeCoverage]
        public eContracting2ApiController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.Context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
        }

        internal eContracting2ApiController(
            ILogger logger,
            ISitecoreContext context,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
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

            return this.StatusCode(HttpStatusCode.NotImplemented);
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

            var model = await this.GetAcceptedViewModel(offer);
            return this.Json(model);
        }

        /// <summary>
        /// Uploads file and returns actual group size.
        /// </summary>
        /// <param name="id">Group identifier.</param>
        [HttpPost]
        [Route("upload/{id}")]
        public async Task<IHttpActionResult> Upload([FromUri] string id)
        {
            return await Task.FromResult(this.Ok("Will be uploaded in the future"));
        }

        /// <summary>
        /// Deletes uploaded file from group and returns actual group size.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fileId">The file identifier.</param>
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

        protected internal async Task<ComplexOfferAcceptedViewModel> GetAcceptedViewModel(OfferModel offer)
        {
            var groups = new List<FilesSectionViewModel>();
            var version = offer.Version;
            var documents = offer.Documents;
            var files = await this.ApiService.GetAttachmentsAsync(offer);
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var page = this.GetAcceptedPageModel();
            var textParameters = offer.TextParameters;

            if (version == 1)
            {
                var fileGroups = files.GroupBy(x => x.Group);

                foreach (IGrouping<string, OfferAttachmentModel> fileGroup in fileGroups)
                {
                    var g = this.GetSection(fileGroup.Key, fileGroup, page, offer);
                    groups.AddRange(g);
                }
            }

            return new ComplexOfferAcceptedViewModel(groups);
        }

        protected internal IEnumerable<FilesSectionViewModel> GetSection(string groupName, IEnumerable<OfferAttachmentModel> attachments, AcceptedOfferPageModel definition, OfferModel offer)
        {
            var list = new List<FilesSectionViewModel>();

            if (groupName == "COMMODITY")
            {
                var acceptFiles = attachments.Where(x => x.Group == "COMMODITY" && !x.IsSignReq);
                var signFiles = attachments.Where(x => x.Group == "COMMODITY" && x.IsSignReq);

                if (acceptFiles.Any())
                {
                    var title = definition.AcceptedDocumentsTitle;
                    list.Add(new FilesSectionViewModel(acceptFiles.Select(x => new FileViewModel(x)), title));
                }

                if (signFiles.Any())
                {
                    var title = definition.SignedDocumentsTitle;
                    list.Add(new FilesSectionViewModel(signFiles.Select(x => new FileViewModel(x)), title));
                }
            }
            else if (groupName == "DLS")
            {
                var files = attachments.Where(x => x.Group == "DLS");
                var title = definition.AdditionalServicesTitle;
                list.Add(new FilesSectionViewModel(files.Select(x => new FileViewModel(x)), title));
            }
            else if (groupName == "NONCOMMODITY")
            {
                var files = attachments.Where(x => x.Group == "NONCOMMODITY");
                var title = definition.OthersTitle;
                list.Add(new FilesSectionViewModel(files.Select(x => new FileViewModel(x)), title));
            }
            else
            {
                this.Logger.Fatal(offer.Guid, $"Unknown group '{groupName}' found");
            }

            return list;
        }

        protected internal string GetGroupTitle(DefinitionCombinationModel definition, OfferAttachmentModel attachment, string groupName)
        {
            if (groupName == "COMMODITY")
            {
                return definition.OfferDocumentsForAcceptanceSection1.Text;
            }

            return null;
        }

        protected internal AcceptedOfferPageModel GetAcceptedPageModel()
        {
            Guid guid = this.SettingsReaderService.GetSiteSettings().AcceptedOffer?.TargetId ?? Guid.Empty;

            if (guid == Guid.Empty)
            {
                return new AcceptedOfferPageModel();
            }

            return this.Context.GetItem<AcceptedOfferPageModel>(guid) ?? new AcceptedOfferPageModel();
        }
    }
}
