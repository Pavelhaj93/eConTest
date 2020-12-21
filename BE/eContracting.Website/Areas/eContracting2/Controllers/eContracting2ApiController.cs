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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.IO;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2ApiController : ApiController, IRequiresSessionState
    {
        protected readonly ILogger Logger;
        protected readonly ISitecoreContext Context;
        protected readonly ISessionProvider SessionProvider;
        protected readonly IOfferService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IOfferJsonDescriptor OfferJsonDescriptor;
        protected readonly IFileOptimizer FileOptimizer;
        protected readonly ISignService SignService;
        protected readonly IUserDataCacheService UserDataCacheService;
        protected readonly IUserFileCacheService UserFileCacheService;

        private static Random KeepAliveRandomizer = new Random();

        [ExcludeFromCodeCoverage]
        public eContracting2ApiController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.Context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
            this.SessionProvider = ServiceLocator.ServiceProvider.GetRequiredService<ISessionProvider>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.OfferJsonDescriptor = ServiceLocator.ServiceProvider.GetRequiredService<IOfferJsonDescriptor>();
            this.SignService = ServiceLocator.ServiceProvider.GetRequiredService<ISignService>();
            this.UserDataCacheService = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
            this.UserFileCacheService = ServiceLocator.ServiceProvider.GetRequiredService<IUserFileCacheService>();
            this.FileOptimizer = ServiceLocator.ServiceProvider.GetRequiredService<IFileOptimizer>();
        }

        internal string FileStorageRoot { get; private set; }

        internal eContracting2ApiController(
            ILogger logger,
            ISitecoreContext context,
            ISessionProvider sessionProvider,
            IOfferService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService,
            ISignService signService,
            IOfferJsonDescriptor offerJsonDescriptor,
            IUserDataCacheService userDataCache,
            IUserFileCacheService userFileCache,
            IFileOptimizer fileOptimizer)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.SessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.SignService = signService ?? throw new ArgumentNullException(nameof(signService));
            this.OfferJsonDescriptor = offerJsonDescriptor ?? throw new ArgumentNullException(nameof(offerJsonDescriptor));
            this.UserDataCacheService = userDataCache ?? throw new ArgumentNullException(nameof(userDataCache));
            this.UserFileCacheService = userFileCache ?? throw new ArgumentNullException(nameof(userFileCache));
            this.FileOptimizer = fileOptimizer ?? throw new ArgumentNullException(nameof(fileOptimizer));

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
        public async Task<IHttpActionResult> File([FromUri] string id)
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

                var formData = await this.Request.Content.ReadAsAsync<SingDataViewModel>();
                var postedSignature = formData.Signature;

                if (string.IsNullOrWhiteSpace(postedSignature))
                {
                    return this.BadRequest("Empty signature");
                }

                var base64 = postedSignature.Substring(postedSignature.IndexOf(",", StringComparison.Ordinal) + 1, postedSignature.Length - postedSignature.IndexOf(",", StringComparison.Ordinal) - 1);
                var signature = Convert.FromBase64String(base64);
                var signedFile = await this.SignService.SignAsync(file, signature);

                if (signedFile == null)
                {
                    return this.InternalServerError();
                }

                var dbSignedFile = new DbSignedFileModel(id, guid, this.SessionProvider.GetId(), signedFile);
                await this.UserFileCacheService.SetAsync(dbSignedFile);

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

        [HttpGet]
        [HttpPost]
        [HttpDelete]
        [HttpOptions]
        public async Task<IHttpActionResult> Upload([FromUri] string id)
        {
            if (this.Request.Method.Method == "POST")
            {
                return await this.AddToUpload(id);
            }

            if (this.Request.Method.Method == "DELETE")
            {
                return await this.DeleteFromUpload(id);
            }

            if (this.Request.Method.Method == "GET")
            {
                return await this.GetUpload(id);
            }

            return this.Ok();
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

                var searchGroupParams = new DbSearchParameters(id, guid, this.SessionProvider.GetId());
                var result = await this.UserFileCacheService.FindGroupAsync(searchGroupParams);

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
        protected async Task<IHttpActionResult> AddToUpload([FromUri] string id)
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
                var fileId = multipartData.FormData["key"];

                if (string.IsNullOrWhiteSpace(fileId))
                {
                    return this.BadRequest("File key cannot be empty");
                }

                if (multipartData.FileData.Count < 1)
                {
                    return this.BadRequest("No file received");
                }

                var groupSearchParams = new DbSearchParameters(id, guid, this.SessionProvider.GetId());
                var group = await this.UserFileCacheService.FindGroupAsync(groupSearchParams);

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
                            group = await this.FileOptimizer.AddAsync(group, id, fileId, originalFileName, fileBytes, this.SessionProvider.GetId(), guid);
                        }
                    }
                }

                if (group == null)
                {
                    return this.InternalServerError();
                }

                this.SaveToDebug(group);

                await this.UserFileCacheService.SetAsync(group);

                return this.Json(new GroupUploadViewModel(group));
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, "An error occured when trying to add new upload into a group", ex);
                return this.InternalServerError(ex);
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
            string guid = null;

            try
            {
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

                if (!this.AuthService.IsLoggedIn())
                {
                    return this.StatusCode(HttpStatusCode.Unauthorized);
                }

                var user = this.AuthService.GetCurrentUser();
                guid = user.Guid;
                var groupSearchParams = new DbSearchParameters(groupId, guid, this.SessionProvider.GetId());
                var group = await this.UserFileCacheService.FindGroupAsync(groupSearchParams);

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
                    await this.UserFileCacheService.SetAsync(updatedGroup);
                    return this.Json(new GroupUploadViewModel(updatedGroup));
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, "An error occured when trying to remove a file from a group", ex);
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

                var submitModel = await this.Request.Content.ReadAsAsync<OfferSubmitDataModel>();

                if (submitModel == null)
                {
                    return this.BadRequest("Invalid submit data");
                }

                var offer = await this.ApiService.GetOfferAsync(guid, false);

                if (offer == null)
                {
                    return this.StatusCode(HttpStatusCode.NoContent);
                }

                if (offer.IsAccepted)
                {
                    return this.BadRequest("Offer is already accepted");
                }

                await this.ApiService.AcceptOfferAsync(offer, submitModel, this.SessionProvider.GetId());

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
    }
}
