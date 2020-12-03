using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
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
        protected readonly IFileOptimizer FileOptimizer;
        protected readonly ISignService SignService;
        protected readonly IUserDataCacheService CacheService;

        [ExcludeFromCodeCoverage]
        public eContracting2ApiController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.Context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.OfferJsonDescriptor = ServiceLocator.ServiceProvider.GetRequiredService<IOfferJsonDescriptor>();
            this.SignService = ServiceLocator.ServiceProvider.GetRequiredService<ISignService>();
            this.CacheService = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
        }

        internal string FileStorageRoot { get; private set; }

        internal eContracting2ApiController(
            ILogger logger,
            ISitecoreContext context,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService,
            ISignService signService,
            IOfferJsonDescriptor offerJsonDescriptor,
            IUserDataCacheService cacheService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.SignService = signService ?? throw new ArgumentNullException(nameof(signService));
            this.OfferJsonDescriptor = offerJsonDescriptor ?? throw new ArgumentNullException(nameof(offerJsonDescriptor));
            this.CacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

            this.FileStorageRoot = HttpContext.Current.Server.MapPath("~/App_Data");
            this.FileOptimizer.FileStorageRoot = this.FileStorageRoot;
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
            string guid = null;

            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
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
                //return this.Json(attachments.Select(x => new FileAttachmentViewModel(x)));            }
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
        }

        /// <summary>
        /// Download requested file from an offer.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpGet]
        [Route("file/{id}")]
        public async Task<IHttpActionResult> File([FromUri]string id)
        {
            string guid = null;
            
            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
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
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
        }

        [HttpGet]
        [Route("thumbnail/{id}")]
        public async Task<IHttpActionResult> Thumbnail([FromUri] string id)
        {
            string guid = null;

            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
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

                if (!file.IsPrinted)
                {
                    return this.BadRequest("File is not printed");
                }

                using (var imageStream = new MemoryStream())
                {
                    using (var pdfStream = new MemoryStream(file.FileContent))
                    {
                        this.PrintPdfToImage(pdfStream, imageStream);

                        var imageArray = imageStream.ToArray();
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.CacheControl = new CacheControlHeaderValue();
                        response.Headers.CacheControl.NoCache = true;
                        response.Content = new ByteArrayContent(imageArray);
                        response.Content.Headers.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(1));
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                        response.Content.Headers.ContentDisposition.FileName = file.FileNameExtension;
                        return this.ResponseMessage(response);
                    }
                }
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
        }

        [HttpPost]
        [Route("sign/{id}")]
        public async Task<IHttpActionResult> Sign([FromUri] string id)
        {
            string guid = null;

            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.Request.Content.IsFormData())
                {
                    return this.BadRequest("Invalid request data");
                }

                var formData = await this.Request.Content.ReadAsFormDataAsync();
                var postedSignature = formData["signature"];

                if (string.IsNullOrWhiteSpace(postedSignature))
                {
                    return this.BadRequest("Empty signature");
                }

                var base64 = postedSignature.Substring(postedSignature.IndexOf(",", StringComparison.Ordinal) + 1, postedSignature.Length - postedSignature.IndexOf(",", StringComparison.Ordinal) - 1);
                var signature = Convert.FromBase64String(base64);
                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
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

                if (!file.IsPrinted)
                {
                    return this.BadRequest("File is not printed");
                }

                if (!file.IsSignReq)
                {
                    return this.BadRequest("File is not determined for sign");
                }

                var signedFile = await this.SignService.SignAsync(file, signature);

                if (signedFile == null)
                {
                    return this.InternalServerError();
                }

                return this.Ok();
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }

            return this.StatusCode(HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Get JSON model for non accepted offer.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("offer")]
        public async Task<IHttpActionResult> Offer()
        {
            string guid = null;

            try
            {
                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
                var offer = await this.ApiService.GetOfferAsync(user.Guid);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                if (offer.IsAccepted)
                {
                    return this.BadRequest("Offer is already accepted");
                }

                var model = await this.OfferJsonDescriptor.GetNewAsync(offer);
                return this.Json(model);
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
        }

        /// <summary>
        /// Get JSON model for accepted offer.
        /// </summary>
        [HttpGet]
        [Route("accepted")]
        public async Task<IHttpActionResult> Accepted()
        {
            string guid = null;

            try
            {
                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
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
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
        }

        /// <summary>
        /// Gets current state of group with <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><see cref="GroupUploadViewModel"/> with current status of the group.</returns>
        [HttpGet]
        [Route("upload/{id}")]
        public async Task<IHttpActionResult> Uploaded([FromUri] string id)
        {
            string guid = null;

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return this.BadRequest("Invalid id");
                }

                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
                var result = await this.FileOptimizer.GetAsync(id);

                if (result == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                return this.Json(new GroupUploadViewModel(result));
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
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
            string guid = null;

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return this.BadRequest("Invalid id");
                }

                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;

                if (!this.Request.Content.IsMimeMultipartContent())
                {
                    return this.BadRequest($"Invalid content type");
                }

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                var multipartData = await this.Request.Content.ReadAsMultipartAsync(provider);
                var key = multipartData.FormData["key"];

                if (string.IsNullOrWhiteSpace(key))
                {
                    return this.BadRequest("File key cannot be empty");
                }

                if (multipartData.FileData.Count == 1)
                {
                    return this.BadRequest("No file received");
                }
                
                OptimizedFileGroupModel result = null;

                // everytime there "should" be only one file
                for (int i = 0; i < multipartData.FileData.Count; i++)
                {
                    var file = multipartData.FileData[i];
                    var localFile = new FileInfo(file.LocalFileName);
                    var originalFileName = file.Headers.ContentDisposition.FileName.Trim('"');

                    using (var stream = localFile.OpenRead())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            var fileBytes = memoryStream.ToArray();
                            result = await this.FileOptimizer.AddAsync(id, key, originalFileName, fileBytes);
                        }
                    }
                }

                if (result == null)
                {
                    return this.InternalServerError();
                }

                return this.Json(new GroupUploadViewModel(result));
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
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
        public async Task<IHttpActionResult> Delete([FromUri] string id)
        {
            string guid = null;

            try
            {
                if (this.Request.Method.Method == "OPTIONS")
                {
                    return this.Ok();
                }

                if (string.IsNullOrWhiteSpace(id))
                {
                    return this.BadRequest("Invalid id");
                }

                var fileId = this.Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "fileId").Value;

                if (string.IsNullOrWhiteSpace(fileId))
                {
                    return this.BadRequest("Invalid file id");
                }

                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
                var result = await this.FileOptimizer.RemoveAsync(id, fileId);

                if (result)
                {
                    var data = await this.FileOptimizer.GetAsync(id);

                    if (data == null)
                    {
                        return this.StatusCode(HttpStatusCode.NoContent);
                    }

                    return this.Json(new GroupUploadViewModel(data));
                }

                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
        }

        /// <summary>
        /// Submit an offer (model not defined yet).
        /// </summary>
        [HttpPost]
        [Route("offer")]
        public async Task<IHttpActionResult> Submit()
        {
            string guid = null;
            
            try
            {
                if (this.Request.Method.Method == "OPTIONS")
                {
                    return this.Ok();
                }

                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;

                //TODO: Process submitted data

                return this.StatusCode(HttpStatusCode.NotImplemented);
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }
        }

        /// <summary>
        /// Converts PDF file to the PNG stream
        /// </summary>
        /// <param name="memoryStream"></param>
        private void PrintPdfToImage(MemoryStream pdfStream, MemoryStream imageStream)
        {
            var pdfImages = new List<Image>();

            using (var document = PdfiumViewer.PdfDocument.Load(pdfStream))
            {
                for (var i = 0; i < document.PageCount; i++)
                {
                    var image = document.Render(i, 600, 600, false);
                    pdfImages.Add(image);
                }
            }

            var bitmap = CombineBitmap(pdfImages);
            bitmap.Save(imageStream, ImageFormat.Png);
        }

        /// <summary>
        /// Mergne obrazky
        /// https://stackoverflow.com/questions/465172/merging-two-images-in-c-net
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private Bitmap CombineBitmap(IEnumerable<Image> files)
        {
            //read all images into memory
            var images = new List<Bitmap>();
            Bitmap finalImage = null;

            try
            {
                var width = 0;
                var height = 0;

                foreach (var image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    var bitmap = new Bitmap(image);

                    //update the size of the final bitmap
                    width = bitmap.Width > width ? bitmap.Width : width;
                    height += bitmap.Height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (var g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(Color.Black);

                    //go through each image and draw it on the final image
                    var offset = 0;
                    foreach (var image in images)
                    {
                        g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                }

                return finalImage;
            }
            catch (Exception)
            {
                finalImage?.Dispose();
                return null;
            }
            finally
            {
                //clean up memory
                foreach (var image in images)
                {
                    image.Dispose();
                }
            }
        }
    }
}
