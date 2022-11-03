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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.DataProviders.Sql.FastQuery;
using Sitecore.DependencyInjection;
using Sitecore.IO;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2ApiController : ApiController, IRequiresSessionState
    {
        protected readonly ILogger Logger;
        protected readonly ISessionProvider SessionProvider;
        protected readonly IOfferService OfferService;
        protected readonly IUserService UserService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IOfferJsonDescriptor OfferJsonDescriptor;
        protected readonly IFileOptimizer FileOptimizer;
        protected readonly ISignService SignService;
        protected readonly IDataSessionCacheService UserDataCacheService;
        protected readonly IUserFileCacheService UserFileCacheService;
        protected readonly IEventLogger EventLogger;
        protected readonly ITextService TextService;
        protected readonly ILoginFailedAttemptBlockerStore LoginReportService;

        private static Random KeepAliveRandomizer = new Random();

        [ExcludeFromCodeCoverage]
        public eContracting2ApiController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.SessionProvider = ServiceLocator.ServiceProvider.GetRequiredService<ISessionProvider>();
            this.OfferService = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
            this.UserService = ServiceLocator.ServiceProvider.GetRequiredService<IUserService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.OfferJsonDescriptor = ServiceLocator.ServiceProvider.GetRequiredService<IOfferJsonDescriptor>();
            this.SignService = ServiceLocator.ServiceProvider.GetRequiredService<ISignService>();
            this.UserDataCacheService = ServiceLocator.ServiceProvider.GetRequiredService<IDataSessionCacheService>();
            this.UserFileCacheService = ServiceLocator.ServiceProvider.GetRequiredService<IUserFileCacheService>();
            this.FileOptimizer = ServiceLocator.ServiceProvider.GetRequiredService<IFileOptimizer>();
            this.EventLogger = ServiceLocator.ServiceProvider.GetRequiredService<IEventLogger>();
            this.TextService = ServiceLocator.ServiceProvider.GetRequiredService<ITextService>();
            this.LoginReportService = ServiceLocator.ServiceProvider.GetRequiredService<ILoginFailedAttemptBlockerStore>();
        }

        [ExcludeFromCodeCoverage]
        public eContracting2ApiController(
            ILogger logger,
            ISessionProvider sessionProvider,
            IOfferService offerService,
            IUserService userService,
            ISettingsReaderService settingsReaderService,
            IOfferJsonDescriptor offerJsonDescriptor,
            ISignService signService,
            IDataSessionCacheService userDataCache,
            IUserFileCacheService userFileCache,
            IFileOptimizer fileOptimizer,
            IEventLogger eventLogger,
            ITextService textService,
            ILoginFailedAttemptBlockerStore loginReportService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.SessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
            this.OfferService = offerService ?? throw new ArgumentNullException(nameof(offerService));
            this.UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.SignService = signService ?? throw new ArgumentNullException(nameof(signService));
            this.OfferJsonDescriptor = offerJsonDescriptor ?? throw new ArgumentNullException(nameof(offerJsonDescriptor));
            this.UserDataCacheService = userDataCache ?? throw new ArgumentNullException(nameof(userDataCache));
            this.UserFileCacheService = userFileCache ?? throw new ArgumentNullException(nameof(userFileCache));
            this.FileOptimizer = fileOptimizer ?? throw new ArgumentNullException(nameof(fileOptimizer));
            this.EventLogger = eventLogger ?? throw new ArgumentNullException(nameof(eventLogger));
            this.TextService = textService ?? throw new ArgumentNullException(nameof(textService));
            this.LoginReportService = loginReportService ?? throw new ArgumentNullException(nameof(loginReportService));
            // this.FileStorageRoot = HttpContext.Current.Server.MapPath("~/App_Data");
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
        public async Task<IHttpActionResult> Files()
        {
            string guid = this.GetGuid();

            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                var attachments = this.OfferService.GetAttachments(offer, user);

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

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
        }

        /// <summary>
        /// Download requested file from an offer.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpGet]
        public async Task<IHttpActionResult> File([FromUri] string id)
        {
            var guid = this.GetGuid();

            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                var attachments = this.OfferService.GetAttachments(offer, user);
                var file = attachments.FirstOrDefault(x => x.UniqueKey == id);

                if (file == null)
                {
                    return this.NotFound();
                }

                byte[] fileContent = file.FileContent;

                if (file.IsSignReq)
                {
                    var dbFile = this.UserFileCacheService.FindSignedFile(new DbSearchParameters(id, guid, this.SessionProvider.GetId()));

                    if (dbFile != null)
                    {
                        fileContent = dbFile.File.Content;
                    }
                }
                
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(fileContent);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.MimeType);
                response.Content.Headers.ContentLength = (long)fileContent.Length;
                response.Content.Headers.Add("Content-Disposition", $"attachment; filename*=UTF-8''{ HttpUtility.UrlPathEncode(file.FileNameExtension).Replace(",", "%2C")}");

                return this.ResponseMessage(response);
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> Thumbnail([FromUri] string id)
        {
            string guid = this.GetGuid();

            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                var attachments = this.OfferService.GetAttachments(offer, user);
                var file = attachments.FirstOrDefault(x => x.UniqueKey == id);

                if (file == null)
                {
                    return this.NotFound();
                }

                if (!file.IsPrinted)
                {
                    return this.BadRequest("File is not printed");
                }

                byte[] fileContent = file.FileContent;

                if (file.IsSignReq)
                {
                    var dbFile = this.UserFileCacheService.FindSignedFile(new DbSearchParameters(id, guid, this.SessionProvider.GetId()));

                    if (dbFile != null)
                    {
                        fileContent = dbFile.File.Content;
                    }
                }

                using (var imageStream = new MemoryStream())
                {
                    using (var pdfStream = new MemoryStream(fileContent))
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

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Sign([FromUri] string id)
        {
            string guid = this.GetGuid();

            try
            {
                // needs to check like this because info about it is only as custom session property :-(
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                var attachments = this.OfferService.GetAttachments(offer, user);
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

                var formData = await this.Request.Content.ReadAsAsync<SingDataViewModel>();
                var postedSignature = formData.Signature;

                if (string.IsNullOrWhiteSpace(postedSignature))
                {
                    return this.BadRequest("Empty signature");
                }

                var base64 = postedSignature.Substring(postedSignature.IndexOf(",", StringComparison.Ordinal) + 1, postedSignature.Length - postedSignature.IndexOf(",", StringComparison.Ordinal) - 1);
                var signature = Convert.FromBase64String(base64);
                var signedFile = this.SignService.Sign(offer, file, signature);

                if (signedFile == null)
                {
                    throw new EcontractingSignException(ERROR_CODES.EmptySignedFile());
                }

                var dbSignedFile = new DbSignedFileModel(id, guid, this.SessionProvider.GetId(), signedFile);
                this.UserFileCacheService.Set(dbSignedFile);
                this.EventLogger.Add(this.SessionProvider.GetId(), guid, EVENT_NAMES.SIGN_DOCUMENT, file.OriginalFileName);

                return this.Ok();
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
            catch (EcontractingSignException ex)
            {
                this.Logger.Fatal(guid, ex);
                var message = this.TextService.Error(ex.Error);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(message, ex);
                }
                else
                {
                    return this.InternalServerError(message);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                var error = ERROR_CODES.UNKNOWN;
                var message = this.TextService.Error(error);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(message, ex);
                }
                else
                {
                    return this.InternalServerError(message);
                }
            }
        }

        /// <summary>
        /// Get JSON model for non accepted offer.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<IHttpActionResult> Offer()
        {
            string guid = this.GetGuid();

            try
            {
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                if (this.Request.Method == HttpMethod.Post)
                {
                    return await this.Submit();
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                if (offer.IsAccepted)
                {
                    return this.BadRequest("Offer is already accepted");
                }

                var model = this.OfferJsonDescriptor.GetNew(offer, user);
                return this.Json(model);
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
            catch (EcontractingDataException ex)
            {
                this.Logger.Fatal(guid, ex);
                var message = this.TextService.Error(ex.Error);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(message, ex);
                }
                else
                {
                    return this.InternalServerError(message);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                var error = ERROR_CODES.UNKNOWN;

                var message = this.TextService.Error(error);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(message, ex);
                }
                else
                {
                    return this.InternalServerError(message);
                }
            }
        }

        /// <summary>
        /// Get JSON model for accepted offer.
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> Accepted()
        {
            string guid = this.GetGuid();

            try
            {
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                if (!offer.IsAccepted)
                {
                    return this.BadRequest();
                }

                var model = this.OfferJsonDescriptor.GetAccepted(offer, user);
                return this.Json(model);
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> Summary()
        {
            string guid = this.GetGuid();

            try
            {
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                var model = this.OfferJsonDescriptor.GetSummary(offer, user);
                return this.Json(model);
            }
            catch (EndpointNotFoundException ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
            catch (EcontractingDataException ex)
            {
                this.Logger.Fatal(guid, ex);
                var message = this.TextService.Error(ex.Error);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(message, ex);
                }
                else
                {
                    return this.InternalServerError(message);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                var error = ERROR_CODES.UNKNOWN;

                var message = this.TextService.Error(error);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(message, ex);
                }
                else
                {
                    return this.InternalServerError(message);
                }
            }
        }

        [HttpGet]
        [HttpPost]
        [HttpDelete]
        [HttpOptions]
        public async Task<IHttpActionResult> Upload([FromUri] string id)
        {
            var guid = this.GetGuid();

            if (!this.CanRead(guid))
            {
                return this.StatusCode(HttpStatusCode.Unauthorized);
            }

            if (this.Request.Method == HttpMethod.Post)
            {
                return await this.AddToUpload(id);
            }

            if (this.Request.Method == HttpMethod.Delete)
            {
                return await this.DeleteFromUpload(id);
            }

            if (this.Request.Method == HttpMethod.Get)
            {
                return await this.GetUpload(id);
            }

            return this.Ok();
        }

        /// <summary>
        /// Submit an offer.
        /// </summary>
        [HttpPost]
        public async Task<IHttpActionResult> Submit()
        {
            string guid = this.GetGuid();

            try
            {
                if (this.Request.Method.Method == "OPTIONS")
                {
                    return this.Ok();
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.UserService.GetUser();

                var submitModel = await this.Request.Content.ReadAsAsync<OfferSubmitDataModel>();

                if (submitModel == null)
                {
                    return this.BadRequest("Invalid submit data");
                }

                var offer = this.OfferService.GetOffer(guid, user, false);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                if (offer.IsAccepted)
                {
                    return this.BadRequest("Offer is already accepted");
                }

                this.OfferService.AcceptOffer(offer, submitModel, user, this.SessionProvider.GetId());
                this.UserService.SaveUser(guid, user);
                this.EventLogger.Add(this.SessionProvider.GetId(), guid, EVENT_NAMES.SUBMIT_OFFER);

                if (this.SettingsReaderService.SubmitOfferDelay > 0)
                {
                    Thread.Sleep(this.SettingsReaderService.SubmitOfferDelay * 1000);
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
        }

        /// <summary>
        /// Cancels an offer
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        [HttpGet]
        public async Task<IHttpActionResult> Cancel()
        {
            string guid = this.GetGuid();

            try
            {
                if (this.Request.Method.Method == "OPTIONS")
                {
                    return this.Ok();
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user, false);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                if (offer.IsAccepted)
                {
                    return this.BadRequest("Offer is already accepted");
                }

                if (!offer.CanBeCanceled)
                {
                    return this.BadRequest("Offer cannot be cancelled.");
                }

                this.OfferService.CancelOffer(guid);
                this.EventLogger.Add(this.SessionProvider.GetId(), guid, EVENT_NAMES.CANCEL_OFFER);

                if (this.SettingsReaderService.CancelOfferDelay > 0)
                {
                    Thread.Sleep(this.SettingsReaderService.CancelOfferDelay * 1000);
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
        }

        [HttpOptions]
        [HttpGet]
        public async Task<IHttpActionResult> Cmb()
        {
            string guid = this.GetGuid();

            try
            {
                if (this.Request.Method.Method == "OPTIONS")
                {
                    return this.Ok();
                }

                var offer = this.OfferService.GetOffer(guid);
                var definition = this.SettingsReaderService.GetDefinition(offer);
                
                if (definition.SummaryCallMeBack == null)
                {
                    return this.NotFound();
                }

                var model = new CallMeBackResponseViewModel();
                model.Title = definition.SummaryCallMeBack.Title;
                model.Phone = offer.Xml.Content.Body.PHONE;
                model.MaxFiles = 0;
                model.MaxFileSize = 0;
                model.AllowedFileTypes = new string[] { };

                if (definition.SummaryCallMeBack.Image != null)
                {
                    model.Image = new ImageViewModel() { Url = definition.SummaryCallMeBack.Image.Src };
                }

                this.UpdateWorkingHours(model, definition.SummaryCallMeBack);
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);
                return this.InternalServerError();
            }

            throw new NotImplementedException();
        }

        [HttpGet]
        [HttpOptions]
        public IHttpActionResult KeepAlive()
        {
            #region Catch phrases
            var catchphrases = new List<string>()
            {
                "Jestlipak si myslíte, soudruzi, že to přežil? No přežil, protože mu to politicky myslelo, vy šmejdi!",
                "Čo bolo, to bolo, v pätnáctém století jsem též nebyl majorom a terazky hej!",
                "...samopal má kadenci: ta ta ta ta ta, a někdy i mnohem rychlejší...",
                "Ahój - hoj, slyšíš to jo?",
                "Šak je to sladké jak hov.. z nutrie",
                "Děláte machry a hajzl máte na chodbě!",
                "Vašu pi.. jsem taky neviděl a věřím, že ju máte",
                "Jak se to ty kur.. šikmooké dozvěděli, že mám cihelňu?",
                "Nová doba, host vyhazuje vrchního!",
                "A přece jdu do Budějovic!",
                "Co blbnete, dyť jsou tady lidi!",
                "Jestli si ten fotbal neprosadíš, tak si u mě mrtvej muž.",
                "Já jsem si ty prsa musel holit, dokud si mě nevzala, ovšem pak jsem se na to vyfláknul...",
                "Vy povídejte, vy jste z Prahy!",
                "Až budeme nahoře, to teprv budou panoramata!",
                "On se tam stejně dívá. No nedivte se, dyť mu hoří barák...",
                "Kdo si nenakrad, musí to brát, jako že nevyhrál",
                "Nemá někdo zájem o dobrou brazilskou kávu?",
                "To je ale pěkná půdička... ta se vám povedla pane Knotek, to jsem sama ráda. Tady se mi to bude věšet",
                "To mělo všechno čistě umělecký platonický ráz. Anička, ta měla na zadečku takovou velkou roztomilou pihu",
                "Máňa říkala, že to není směroplatný",
                "Nejradši mám takový, co nenosej prsenky",
                "Doporučuji studenou sprchu a tření končetin a celého těla froté ručníky",
                "Neber úplatky, neber úplatky nebo se z toho zblázníš",
                "Tak kdepak je ten prďola, co tady čepuje pivo?",
                "Neřeš, nepřepínej a hlavně po ničem nepátrej",
                "Nikam neuběgáj a normálně zůstaň v metró... konec hlášení v ruském jazyce",
                "Já znám široko daleko všechny krávy, ale vás jsem tady ještě neviděl",
                "Kakaová skvrna velikosti Mexického dolaru!",
                "Já se vrátím a nebudu sám Dougu Badmane, se mnou přijde zákon",
                "Hliník se vodstěhoval do Humpolce",
                "Metelesku blesku",
                "Nepotěšil jste mě, ani já vás nepotěším",
                "Sem si dovolil... na ukázku ze své zahrádky pár švestiček",
                "To jsou blechy psí, ty na člověka nejdou",
                "Děda je hodný, ale překáží nám",
                "Slovan jsem a Slovan budu! Až jindy Lakatoši",
                "A jiný kruhy nemáte? Kreténi!",
                "Je-li ruka nastřelena, tak v žádném případě",
                "Kde udělali soudruzi z NDR chybu?",
                "Jsou tady Rusové! Tak jim řekni, že hned přijdeme. To je vůl, ten Boris",
                "Nudíte se? Kupte si medvídka mývala",
                "Tak z toho nedělejme aféru, onanoval každý...",
                "Repráky here, dráty here, kazeťák here... I am going, I am playing.. a to je zakázaný, jo",
                "Tři vojáky těžce zranil, dva zhmoždil a jednoho zesměšnil",
                "Prdí taky hadi?",
                "Co to je za muziku? To je státní hymna, ty vole",
                "Tys nás zapálil? Nezapálil. Dyť hoříme!",
                "To zas bude v álejích nablito",
                "Inženýrka, to je něco, auto má z Tuzexu, peněz jako šlupek...",
                "Tu přední nohu budeš jíst, že se budeš divit",
                "Bitvu jsem presrál, ale dobro sa vyspál!",
                "Jeď do Pelhřimova a prohlédni si krematorium, ať víš, do čeho jdeš",
                "Na funkci to vliv mít nebude. To jako na funkci rostlináře?",
                "Stěrače stírají, klakson troubí...",
                "Já jsem inženýr Králík... Ježiš to je hrozný, takhle se střískat za bílého dne",
                "Dobrý den, půjde Jindra ven? Nikdy, nebo navždy...",
                "Dávám Bolševikovi rok, maximálně dva!",
                "Proletáří všech zemí světa, vyližte si prdel.",
                "Pane učiteli, už je čas...",
                "Máš v hlavně místo mozku z piva kostku.",
                ".... Vy nekouříte? Ne, já když kouřím, tak zvracím",
                "Chčije a chčije",
                "Ty kuřata v tý bedně se mi zdáli nějaký divný, měly takový zahnutý zobáky",
                "Nečum na mně, nebo ti useknu ruku",
                "Ráno ho našli na pláži, ležel tam jak vorvaň. Už se do něho pouštěli racci",
                "Do obrněného transportéru se vejde 20 - 24 západoněmeckých soudruhů",
                "Tati, a bila tě taky tvoje maminka? Ne, jenom ta tvoje",
                "Spadlo ti to, asi vítr",
                "Táto, ty si se zul. Ale jenom trošku",
                "Rodiče, kteří mají zájem vidět Idiota, nechť se dostaví do ředitelny",
                "To musim sežrat, i kdybych se měl poblejt",
                "My nejsme žádní ti, unterwassermani",
                "Pepíno! Už toho máme dost! - My taky! Matýsek se posral!",
                "Kačenka jde celou dobu v bačkůrkách – to má smůlu a v čem bude lyžovat?",
                "Kdo to byl? Omyl...A co chtěl?",
                "Já sem synovec...Hmm, a chcete to reklamovat, kdo vám to dělal?",
                "A zme v pérdéli, pane hrábě!",
                "Chlapi, nelijte to pivo z vokna...",
                "Vona taky dáma může být pěkná svině...",
                "Já ti dám posraný hasiči, ty chuligáne!",
                "Prosimvás, netykejte mi, ja to vopravdu nemám rád. Kdo ti tyká?",
                "Pojedeme zkratkou, je to sice delší, ale zato horší cesta.",
                "Vydrž, prďka vydrž!!",
                "Kdo sem pověsil tu máničku? Myslíš Gagarinova bratra?",
                "A komu tim prospějete?!",
                "Hledáš někoho soudruhu? Co že ?!",
                "Co to je za čaj? - Normální houbovej čaj...",
                "Doktor říkal, že je momentálně zaostalej",
                "Odvolávám, co jsem odvolal a slibuji, co jsem slíbil",
                "Vy jste se zase kochal, že jo, pane doktore?",
                "Když vy kachličky, tak my břízolit",
                "Drž to pořádně nebo ti jednu fláknu! Se pobleju! Aby ses neposral!",
                "Složenky, vy krávy zelený, nenažraný",
                "Dědo, kde se to splachuje? To nech ležet…",
                "Chlape, odkud jste přišel? STS Chvojkovice Brod. Hmmm, zřejmě nějakej slušnej oddíl!",
                "Já měl na frontě aj 27 stupňů a šol som na steč",
                "Čéská foják, dobrá foják",
                "Sejdeme se za 15 minut - já nemám hodinky - tak za 20",
                "Poslušně hlásím, že jsem opět zde!"
            };
            #endregion

            try
            {
                this.SessionProvider.Set("D2DKeepAlive", DateTime.Now);
                var index = KeepAliveRandomizer.Next(0, catchphrases.Count);
                var phrase = catchphrases[index];

                var user = this.UserService.GetUser();
                this.UserService.RefreshAuthorizationIfNeeded(null, user);

                return this.Ok(phrase);
            }
            catch { }

            return this.Ok();
        }

        /// <summary>
        /// Gets current state of group with <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><see cref="GroupUploadViewModel"/> with current status of the group.</returns>
        protected async Task<IHttpActionResult> GetUpload([FromUri] string id)
        {
            string guid = this.GetGuid();

            try
            {
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                if (string.IsNullOrWhiteSpace(id))
                {
                    return this.BadRequest("Invalid id");
                }

                var user = this.UserService.GetUser();

                var searchGroupParams = new DbSearchParameters(id, guid, this.SessionProvider.GetId());
                var result = this.UserFileCacheService.FindGroup(searchGroupParams);

                if (result == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                return this.Json(new GroupUploadViewModel(result));
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
        }

        /// <summary>
        /// Uploads file and returns actual group state.
        /// </summary>
        /// <param name="id">Group identifier.</param>
        /// <returns><see cref="GroupUploadViewModel"/> with current status of uploaded files. Failed uploaded file is not presented.</returns>
        protected async Task<IHttpActionResult> AddToUpload([FromUri] string id)
        {
            UploadGroupFileOperationResultModel internalOperationResult= new UploadGroupFileOperationResultModel();
            string guid = this.GetGuid();

            try
            {
                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                if (string.IsNullOrWhiteSpace(id))
                {
                    return this.BadRequest("Invalid id");
                }

                var user = this.UserService.GetUser();

                if (!this.Request.Content.IsMimeMultipartContent())
                {
                    return this.BadRequest($"Invalid content type");
                }

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                var multipartData = await this.Request.Content.ReadAsMultipartAsync(provider);
                var fileId = GetSafeUploadedFileName(multipartData.FormData["key"]);

                if (string.IsNullOrWhiteSpace(fileId))
                {
                    return this.BadRequest("File key cannot be empty");
                }
                
                //alphanumeric keys only
                if (!Regex.IsMatch(fileId, @"^[a-zA-Z0-9]*$"))
                {
                    return this.BadRequest("File key can contain alphanumeric chracters only.");
                }

                if (multipartData.FileData.Count < 1)
                {
                    return this.BadRequest("No file received");
                }

                var groupSearchParams = new DbSearchParameters(id, guid, this.SessionProvider.GetId());
                var group = this.UserFileCacheService.FindGroup(groupSearchParams);

                // everytime there "should" be only one file
                for (int i = 0; i < multipartData.FileData.Count; i++)
                {
                    var file = multipartData.FileData[i];
                    var localFile = new FileInfo(file.LocalFileName);
                    var originalFileName = this.GetSafeUploadedFileName(file.Headers.ContentDisposition.FileName.Trim('"'));

                    using (var stream = localFile.OpenRead())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            var fileBytes = memoryStream.ToArray();
                            internalOperationResult = await this.FileOptimizer.AddAsync(group, id, fileId, originalFileName, fileBytes, this.SessionProvider.GetId(), guid);
                            group = internalOperationResult?.DbUploadGroupFileModel;
                        }
                    }

                    if (internalOperationResult.IsSuccess)
                    {
                        if (group == null)
                        {
                            return this.InternalServerError();
                        }

                        // ukladam tu, se kterou se pracovalo
                        this.SaveToDebug(group);
                        this.UserFileCacheService.Set(group);

                        // nactu ostatni groups teto nabidky a overim celkovou velikost vyslednych souboru oproti limitu
                        var allOfferGroupsSearchParams = new DbSearchParameters(null, guid, this.SessionProvider.GetId());
                        int allOfferGroupsOutFilesSize = this.UserFileCacheService.GetTotalOutputFileSize(allOfferGroupsSearchParams);

                        // celkova velikost vystupnich souboru prekracuje limit
                        if (!(await this.FileOptimizer.IsOfferTotalFilesSizeInLimitAsync(allOfferGroupsOutFilesSize)))
                        {
                            var allOfferGroups = this.UserFileCacheService.FindGroups(allOfferGroupsSearchParams);

                            // provedu pokus o kompresi
                            var checkOperationResult = await this.FileOptimizer.EnforceOfferTotalFilesSizeAsync(allOfferGroups, group, fileId);

                            if (!checkOperationResult.IsSuccess)
                            {
                                // nepodarilo se dodrzet celkove kriterium, tak ten posledni soubor zase odmaz
                                var updatedGroup = await this.FileOptimizer.RemoveFileAsync(group, fileId);
                                this.UserFileCacheService.Set(updatedGroup);
                                return this.BadRequest(this.TextService.Error(checkOperationResult.ErrorModel));
                            }
                            else
                            {
                                if (checkOperationResult.MadeChanges)
                                {
                                    foreach (var modifiedGroup in checkOperationResult.DbUploadGroupFileModels)
                                    {
                                        //this.SaveToDebug(modifiedGroup);
                                        this.UserFileCacheService.Set(modifiedGroup);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return this.BadRequest(this.TextService.Error(internalOperationResult.ErrorModel));
                    }

                }

                this.EventLogger.Add(this.SessionProvider.GetId(), guid, EVENT_NAMES.UPLOAD_ATTACHMENT, group.OutputFile.FileName);

                return this.Json(new GroupUploadViewModel(group));
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, "An error occured when trying to add new upload into a group", ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError(this.TextService.Error(ERROR_CODES.UploadFileError()));
                }
            }
        }

        /// <summary>
        /// Deletes uploaded file from group and returns actual group state.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns><see cref="GroupUploadViewModel"/> with current status of related files.</returns>
        protected async Task<IHttpActionResult> DeleteFromUpload([FromUri] string id)
        {
            string guid = this.GetGuid();

            try
            {

                if (!this.CanRead(guid))
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                if (!this.IsValidGuid(guid))
                {
                    return this.InvalidGuid(guid);
                }

                string groupId = id;

                if (string.IsNullOrWhiteSpace(groupId))
                {
                    return this.BadRequest("Invalid id");
                }

                var fileId = this.Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "f").Value;

                if (string.IsNullOrWhiteSpace(fileId))
                {
                    return this.BadRequest("Invalid file id");
                }

                var user = this.UserService.GetUser();
                var groupSearchParams = new DbSearchParameters(groupId, guid, this.SessionProvider.GetId());
                var group = this.UserFileCacheService.FindGroup(groupSearchParams);

                if (group == null)
                {
                    return this.BadRequest("Targe upload doesn't exists");
                }

                var updatedGroup = await this.FileOptimizer.RemoveFileAsync(group, fileId);

                if (updatedGroup == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    this.UserFileCacheService.Set(updatedGroup);
                    return this.Json(new GroupUploadViewModel(updatedGroup));
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, "An error occured when trying to remove a file from a group", ex);

                if (this.SettingsReaderService.ShowDebugMessages)
                {
                    return this.InternalServerError(ex);
                }
                else
                {
                    return this.InternalServerError();
                }
            }
        }

        /// <summary>
        /// Delete obsolete files from database that have not been disposed correctly at the end of a session
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> DeleteOldFiles(string mode)
        {
            bool previewOnly = mode != "delete";
            List<string> outputMsgs = new List<string>();

            if (!this.SettingsReaderService.GetSiteSettings().EnableCleanupApiTrigger)
                return this.BadRequest("Cleanup not enabled through api endpont. Check EnableCleanupApiTrigger field.");

            var cleanupFilesOlderThanDays = SettingsReaderService.GetSiteSettings().CleanupFilesOlderThanDays;
            if (cleanupFilesOlderThanDays == null || cleanupFilesOlderThanDays <= 0)
            {
                return this.BadRequest("Invalid value of CleanupFilesOlderThanDays field. Positive number expected, 1 or bigger recommended.");
            }

            var filesSearchParams = new DbSearchParameters(DateTime.UtcNow.AddDays(-1 * cleanupFilesOlderThanDays));
            var groupsToDelete = this.UserFileCacheService.FindGroups(filesSearchParams);
            if (groupsToDelete != null && groupsToDelete.Any())
            {
                foreach (var group in groupsToDelete)
                {
                    string msg = $"{(previewOnly ? "Would delete" : "Deleted")} obsolete UploadGroup {group.Key} for offer {group.Guid}, session { group.SessionId}, created at {group.CreateDate}";
                    outputMsgs.Add(msg);
                    Sitecore.Diagnostics.Log.Info(msg, this);
                    if (!previewOnly)
                    {
                        this.UserFileCacheService.RemoveGroup(new DbSearchParameters(group.Key, group.Guid, group.SessionId));
                    }
                }
            }
            var signedFilesToDelete = this.UserFileCacheService.FindSignedFiles(filesSearchParams);
            if (signedFilesToDelete != null && signedFilesToDelete.Any())
            {
                foreach (var signedFile in signedFilesToDelete)
                {
                    string msg = $"{(previewOnly ? "Would delete" : "Deleted")} obsolete SignedFile {signedFile.Key} for offer {signedFile.Guid}, session { signedFile.SessionId}, created at {signedFile.CreateDate}";
                    outputMsgs.Add(msg);
                    Sitecore.Diagnostics.Log.Info(msg, this);
                    if (!previewOnly)
                    {
                        this.UserFileCacheService.RemoveSignedFile(new DbSearchParameters(signedFile.Key, signedFile.Guid, signedFile.SessionId));
                    }
                }
            }
            return this.Ok(outputMsgs);
        }

        /// <summary>
        /// Delete obsolete files from database that have not been disposed correctly at the end of a session
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> DeleteOldLogs(string mode)
        {
            bool previewOnly = mode != "delete";
            List<string> outputMsgs = new List<string>();

            if (!this.SettingsReaderService.GetSiteSettings().EnableCleanupApiTrigger)
                return this.BadRequest("Cleanup not enabled through api endpont. Check EnableCleanupApiTrigger field.");

            var cleanupLogsOlderThanDays = SettingsReaderService.GetSiteSettings().CleanupLogsOlderThanDays;
            if (cleanupLogsOlderThanDays == null || cleanupLogsOlderThanDays <= 0)
            {
                return this.BadRequest("Invalid value of CleanupLogsOlderThanDays field. Positive number expected, 1 or bigger recommended.");
            }

            DateTime removeLogsOlderThanDate =  DateTime.UtcNow.AddDays(-1 * cleanupLogsOlderThanDays);
            int loginAttemptsCount = this.LoginReportService.DeleteAllOlderThan(removeLogsOlderThanDate, previewOnly);
            outputMsgs.Add((previewOnly ? "Would delete " : "Deleted ") + loginAttemptsCount + " login attempts records.");

            return this.Ok(outputMsgs);
        }

        protected internal bool CanRead(string guid)
        {
            return this.UserService.IsAuthorizedFor(guid);
        }

        /// <summary>
        /// Converts PDF file to the PNG stream
        /// </summary>
        /// <param name="memoryStream"></param>
        protected internal void PrintPdfToImage(MemoryStream pdfStream, MemoryStream imageStream)
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
        protected internal Bitmap CombineBitmap(IEnumerable<Image> files)
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

        protected internal void SaveToDebug(DbUploadGroupFileModel group)
        {
            try
            {
                if (this.SettingsReaderService.SaveFilesToDebugFolder)
                {
                    var fileName = new StringBuilder();
                    fileName.Append("upload_group");
                    fileName.Append("_");
                    fileName.Append(group.Guid);
                    fileName.Append("_");
                    fileName.Append(group.Id);
                    fileName.Append(".pdf");
                    var filePath = Sitecore.Configuration.Settings.DebugFolder + "/" + fileName.ToString();
                    System.IO.File.WriteAllBytes(filePath, group.OutputFile.Content);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(null, "Cannot save files to debug folder", ex);
            }
        }

        protected IHttpActionResult Error(string guid, Exception ex)
        {
            this.Logger.Fatal(guid, ex);
            string code = null;
            string message = null;

            if (ex is EcontractingCodeException)
            {
                var error = (ex as EcontractingCodeException).Error;
                code = error.Code;
                message = this.TextService.Error(error);
            }
            else if (ex is EndpointNotFoundException)
            {
                
            }
            else
            {
                code = ERROR_CODES.UNKNOWN.Code;
            }

            if (this.SettingsReaderService.ShowDebugMessages)
            {
                return this.InternalServerError(message, ex);
            }
            else
            {
                return this.InternalServerError(message);
            }
        }

        protected IHttpActionResult InvalidGuid(string guid)
        {
            return this.BadRequest("Invalid guid");
        }

        protected internal string GetSafeUploadedFileName(string fileIdFromRequest)
        {
            if (string.IsNullOrEmpty(fileIdFromRequest))
                return fileIdFromRequest;

            string fileName = System.IO.Path.GetFileName(fileIdFromRequest);
            return fileName;
        }

        /// <summary>
        /// Gets guid value from query string.
        /// </summary>
        protected string GetGuid()
        {
            return HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];
        }

        protected bool IsValidGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            if (guid == Constants.FakeOfferGuid)
            {
                return false;
            }

            return true;
        }

        protected SelectionViewModel UpdateWorkingHours(CallMeBackResponseViewModel model, ICallMeBackModalWindow cmb)
        {
            var currentDay = (DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
            IWorkingDay workingDay = null;

            if (currentDay == cmb.AvailabilityMonday.DayInWeek)
            {
                workingDay = cmb.AvailabilityMonday;
            }
            else if (currentDay == cmb.AvailabilityThuesday.DayInWeek)
            {
                workingDay = cmb.AvailabilityThuesday;
            }
            else if (currentDay == cmb.AvailabilityWednesday.DayInWeek)
            {
                workingDay = cmb.AvailabilityWednesday;
            }
            else if (currentDay == cmb.AvailabilityThursday.DayInWeek)
            {
                workingDay = cmb.AvailabilityThursday;
            }
            else if (currentDay == cmb.AvailabilityFriday.DayInWeek)
            {
                workingDay = cmb.AvailabilityFriday;
            }
            else
            {
                return null;
            }

            return null;
        }

        protected SelectionViewModel GetWorkingHours(IWorkingDay workingDay)
        {
            if (workingDay == null)
            {
                return null;
            }

            var model = new SelectionViewModel();

            if (string.IsNullOrEmpty(workingDay.OperatorHoursFrom) && string.IsNullOrEmpty(workingDay.OperatorHoursTo))
            {
                model.Label = workingDay.OperatorHoursFrom + " - " + workingDay.OperatorHoursTo;
                model.Value = model.Label;
                return model;
            }

            return null;
        }
    }
}
