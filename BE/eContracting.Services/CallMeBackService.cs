using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.ServiceModel.Description;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;
using eContracting.SAP.CrmUtilitiesUmc;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Drawing.Exif;
using Sitecore.SecurityModel;
using Sitecore.Text;
using static Sitecore.Configuration.State;

namespace eContracting.Services
{
    public class CallMeBackService : ICallMeBackService
    {
        protected const string ENDPOINT_PATH = "ZCreateActivitySet";
        protected const string SAP_DATE_FORMAT = "yyyyMMddHHmmss";
        protected const string EMPTY_SAP_DATE = "0000000000";
        protected readonly ISettingsReaderService SettingsReader;
        protected readonly ILogger Logger;
        //protected static HttpClient HttpClient = new HttpClient();
        //[ThreadStatic]
        //protected static CookieContainer CookieJar = new CookieContainer();

        public CallMeBackService(ISettingsReaderService settingsReader, ILogger logger)
        {
            this.SettingsReader = settingsReader;
            this.Logger = logger;
        }

        public IEnumerable<KeyValuePair<string, string>> GetAvailableTimes(ICallMeBackModalWindow datasource)
        {
            IWorkingDay workingDay = this.GetWorkingDay(datasource);

            TimeSpan from;

            if (!TimeSpan.TryParse(workingDay.WorkingHoursFrom, out from))
            {
                from = new TimeSpan(9, 0, 0);
            }

            TimeSpan to;

            if (!TimeSpan.TryParse(workingDay.WorkingHoursTo, out to))
            {
                to = new TimeSpan(17, 0, 0);
            }

            var times = new List<KeyValuePair<string, string>>();

            for (int hour = from.Hours; hour < to.Hours; hour++)
            {
                var ts1 = new TimeSpan(hour, 0, 0);
                var ts2 = new TimeSpan(hour + 1, 0, 0);
                var key = ts1.ToString("hh\\:mm") + "-" + ts2.ToString("hh\\:mm");
                var value = ts1.ToString("hh\\:mm") + " - " + ts2.ToString("hh\\:mm");
                times.Add(new KeyValuePair<string, string>(key, value));
            }

            return times;
        }

        public bool Send(CallMeBackModel model, UserCacheDataModel userData, ICallMeBackModalWindow datasource)
        {
            var zActivity = this.GetCmbModel(model, datasource);
            return this.SendCmbData(model, zActivity, userData);
        }

        public bool IsValidFileType(string originalFileName, ICallMeBackModalWindow datasource)
        {
            var extension = Path.GetExtension(originalFileName) ?? "???";
            var allowedExtensions = datasource.SettingsAllowedFileTypes?.ToLowerInvariant().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
            return allowedExtensions.Any(x => x.Equals(extension));
        }

        public bool IsValidFileSize(int fileSize, ICallMeBackModalWindow datasource)
        {
            return fileSize <= datasource.SettingsMaxFileSize;
        }

        protected internal bool SendCmbData(CallMeBackModel model, CallMeBackDataModel data, UserCacheDataModel userData)
        {
            // Disable SSL certificate trust
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // Nastaveni Tls12
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var tokenResult = this.GetCsrfToken(model, userData);
            var token = tokenResult.Item1;

            if (string.IsNullOrEmpty(token))
            {
                this.Logger.Error(model.OfferGuid, "[CMB] Cannot send CMB because token was not received.");
                return false;
            }

            var cookieContainer = tokenResult.Item2 ?? new CookieContainer();
            var apiId = this.SettingsReader.SapApiGatewayId;
            var baseUri = userData.IsCognito ? this.SettingsReader.CrmCognitoUrl : this.SettingsReader.CrmAnonymousUrl;
            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.Path = Sitecore.MainUtil.MakeFilePath(uriBuilder.Path, ENDPOINT_PATH);
            var endpoint = uriBuilder.Uri;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = HttpMethod.Post.Method;
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Timeout = 10000;

            var cookies = cookieContainer.GetCookies(baseUri);
            var headerCookie = string.Empty;

            foreach (Cookie cookie in cookies)
            {
                headerCookie += string.IsNullOrEmpty(headerCookie) ? cookie.ToString() : string.Concat(";", cookie.ToString());
            }

            request.Headers.Add("Cookie", headerCookie);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("X-CSRF-Token", token);
            }

            if (userData.IsCognito)
            {
                request.Headers.Add("x-apigw-api-id", apiId);
                request.Headers.Add("Authorization", $"Bearer {userData.Tokens.AccessToken}");
            }
            else
            {
                request.Credentials = new NetworkCredential(this.SettingsReader.CrmAnonymousUser, this.SettingsReader.CrmAnonymousPassword);
            }

            try
            {
                var bodyText = JsonConvert.SerializeObject(data);

                this.Logger.Debug(model.OfferGuid, this.GetRequestLogMessage(request, model, userData, token));

                using (var stream = request.GetRequestStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(bodyText);
                    }
                }

                var response = (HttpWebResponse)request.GetResponse();
                
                this.Logger.Info(model.OfferGuid, this.GetResponseLogMessage(response));

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }

                return false;
            }
            catch (WebException ex)
            {
                var logBuilder = new StringBuilder();
                logBuilder.AppendLine("[CMB] Failed to send CMB data");

                if (ex.Response is HttpWebResponse)
                {
                    var response = (HttpWebResponse)ex.Response;

                    logBuilder.AppendLine($" - Status code: {response.StatusCode}");
                    logBuilder.AppendLine($" - Message: {response.StatusDescription}");

                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var body = reader.ReadToEnd();
                        logBuilder.AppendLine($" - Body: {body}");

                    }
                }

                this.Logger.Fatal(model.OfferGuid, logBuilder.ToString(), ex);

                throw;
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(model.OfferGuid, "[CMB] Failed to send CMB data", ex);
                throw;
            }
        }

        protected internal Tuple<string, CookieContainer> GetCsrfToken(CallMeBackModel model, UserCacheDataModel userData)
        {
            if (userData.IsCognito)
            {
                return this.GetCsrfTokenAsCognito(model, userData);
            }
            else
            {
                return this.GetCsrfTokenAsAnonymous(model);
            }
        }

        /// <summary>
        /// Gets CSRF token for non Congnito logged in user.
        /// </summary>
        /// <remarks>
        ///     <para>Calling endpoint - <see cref="ISettingsReaderService.CrmAnonymousUrl"/></para>
        ///     <para>Using username - <see cref="ISettingsReaderService.CrmAnonymousUser"/></para>
        ///     <para>Using password - <see cref="ISettingsReaderService.CrmAnonymousPassword"/></para>
        /// </remarks>
        /// <param name="model">Call me back data model.</param>
        /// <returns>CSRF token | received cookies</returns>
        protected internal Tuple<string, CookieContainer> GetCsrfTokenAsAnonymous(CallMeBackModel model)
        {
            try
            {
                this.Logger.Debug(model.OfferGuid, "[CMB] Trying to get CSRF token as anonymous");

                var uri = this.SettingsReader.CrmAnonymousUrl;
                var username = this.SettingsReader.CrmAnonymousUser;
                var password = this.SettingsReader.CrmAnonymousPassword;
                var authData = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{username}:{password}"));
                
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add("X-CSRF-Token", "Fetch");
                request.AllowAutoRedirect = false;
                request.Headers.Add("Authorization", $"Basic {authData}"); //; new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authData).ToString());
                request.CookieContainer = new CookieContainer();

                var response = (HttpWebResponse)request.GetResponse();
                var token = response.Headers.Get("X-CSRF-Token");
                
                if (!string.IsNullOrEmpty(token))
                {
                    this.Logger.Info(model.OfferGuid, "[CMB] CSRF token as anonymous received");
                    return new Tuple<string, CookieContainer>(token, request.CookieContainer);
                }
                else
                {
                    var logBuilder = new StringBuilder();
                    logBuilder.AppendLine("[CMB] Response for anonymous doesn't contain CSRF token");
                    logBuilder.AppendLine($" - Status Code: {(int)response.StatusCode}");
                    logBuilder.AppendLine($" - Message: {response.StatusDescription}");
                    this.Logger.Fatal(model.OfferGuid, logBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(model.OfferGuid, "[CMB] Failed to get CSRF token as anonymous", ex);
            }

            return new Tuple<string, CookieContainer>(null, null);
        }

        /// <summary>
        /// Gets CSRF token for Cognito logged in user.
        /// </summary>
        /// <remarks>
        ///     <para>Calling endpoint - <see cref="ISettingsReaderService.CrmCognitoUrl"/></para>
        ///     <para>Using API ID - <see cref="ISettingsReaderService.SapApiGatewayId"/></para>
        ///     <para>Using Bearer authorization with <see cref="OAuthTokensModel.AccessToken"/></para>
        /// </remarks>
        /// <param name="model">Call me back data model.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>CSRF token | received cookies</returns>
        protected internal Tuple<string, CookieContainer> GetCsrfTokenAsCognito(CallMeBackModel model, UserCacheDataModel userData)
        {
            try
            {
                this.Logger.Debug(model.OfferGuid, "[CMB] Trying to get CSRF token for Cognito user");
                var apiId = this.SettingsReader.SapApiGatewayId;

                var request = (HttpWebRequest)WebRequest.Create(this.SettingsReader.CrmCognitoUrl);
                request.Method = HttpMethod.Get.Method;
                request.Accept = "*/*";
                request.Headers.Add("X-CSRF-Token", "Fetch");
                request.Headers.Add("x-apigw-api-id", apiId); // <add key="ApiGateway.Api.Id" value="d6bwzpwoq4" />
                request.Headers.Add("Authorization", $"Bearer {userData.Tokens.AccessToken}");
                request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                request.AllowAutoRedirect = false;
                //CookieJar = new CookieContainer();
                request.CookieContainer = new CookieContainer();

                var response = (HttpWebResponse)request.GetResponse();
                var token = response.Headers.Get("X-CSRF-Token");

                if (!string.IsNullOrEmpty(token))
                {
                    this.Logger.Info(model.OfferGuid, "[CMB] CSRF token for Cognito received");
                    return new Tuple<string, CookieContainer>(token, request.CookieContainer);
                }
                else
                {
                    var logBuilder = new StringBuilder();
                    logBuilder.AppendLine("[CMB] Response for Cognito doesn't contain CSRF token");
                    logBuilder.AppendLine($" - Status Code: {(int)response.StatusCode}");
                    logBuilder.AppendLine($" - Message: {response.StatusDescription}");
                    this.Logger.Fatal(model.OfferGuid, logBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(model.OfferGuid, "[CMB] Failed to get CSRF token for Cognito user", ex);
            }

            return new Tuple<string, CookieContainer>(null, null);
        }

        protected internal string GetTokenFromRequestMessage(CallMeBackModel cmb, HttpRequestMessage request)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }))
            {
                var task = Task.Run(() => httpClient.SendAsync(request));
                task.Wait();
                using (var response = task.Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Headers.TryGetValues("X-CSRF-Token", out IEnumerable<string> values))
                        {
                            var value = values.First();
                            return value;
                        }
                        else
                        {
                            this.Logger.Error(cmb.OfferGuid, "[CMB] Header doesn't contain 'X-CSRF-Token'");
                        }
                    }
                    else
                    {
                        this.Logger.Error(cmb.OfferGuid, "[CMB] Request for token failed: " + response.ReasonPhrase);
                    }
                }
            }

            return null;
        }

        protected internal IWorkingDay GetWorkingDay(ICallMeBackModalWindow datasource)
        {
            var currentDay = (DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
            IWorkingDay workingDay = this.GetCurrentWorkingDay(currentDay, datasource);

            if (workingDay != null)
            {
                TimeSpan from;

                if (!TimeSpan.TryParse(workingDay.WorkingHoursFrom, out from))
                {
                    from = new TimeSpan(9, 0, 0);
                }

                TimeSpan to;

                if (!TimeSpan.TryParse(workingDay.WorkingHoursTo, out to))
                {
                    to = new TimeSpan(17, 0, 0);
                }

                if (DateTime.Now.TimeOfDay > to)
                {
                    workingDay = this.GetCurrentWorkingDay(currentDay == 7 ? 0 : currentDay + 1, datasource);
                }
            }

            if (workingDay == null)
            {
                workingDay = datasource.AvailabilityMonday;
            }

            return workingDay;
        }

        protected internal IWorkingDay GetCurrentWorkingDay(int currentDay, ICallMeBackModalWindow datasource)
        {
            if (currentDay == datasource.AvailabilityMonday.DayInWeek)
            {
                return datasource.AvailabilityMonday;
            }
            else if (currentDay == datasource.AvailabilityThuesday.DayInWeek)
            {
                return datasource.AvailabilityThuesday;
            }
            else if (currentDay == datasource.AvailabilityWednesday.DayInWeek)
            {
                return datasource.AvailabilityWednesday;
            }
            else if (currentDay == datasource.AvailabilityThursday.DayInWeek)
            {
                return datasource.AvailabilityThursday;
            }
            else if (currentDay == datasource.AvailabilityFriday.DayInWeek)
            {
                return datasource.AvailabilityFriday;
            }
            else
            {
                return null;
            }
        }

        protected internal CallMeBackDataModel GetCmbModel(CallMeBackModel model, ICallMeBackModalWindow datasource)
        {
            var zFiles = this.GetFiles(model, datasource);
            var zParameters = this.GetParameters(model, datasource);

            var zActivity = new CallMeBackDataModel();
            zActivity.Description = datasource.SettingsDescription;
            zActivity.Partner = model.Partner;
            zActivity.Note = model.Note;
            zActivity.Eicean = model.EicEan;
            zActivity.PhoneNumber = model.Phone;
            zActivity.Process = model.Process;
            zActivity.ExternalSystem = model.ExternalSystem;
            zActivity.ExternalKey = String.Empty;
            zActivity.ZCreateActivityFilesSet = new Collection<CallMeBackFileDataModel>(zFiles.ToList());
            zActivity.ZCreateActivityParametersSet = new Collection<CallMeBackAttributeDataModel>(zParameters.ToList());

            return zActivity;
        }

        protected internal CallMeBackFileDataModel[] GetFiles(CallMeBackModel cmb, ICallMeBackModalWindow datasource)
        {
            var files = new List<CallMeBackFileDataModel>();

            if (cmb.Files?.Length > 0)
            {
                for (int i = 0; i < cmb.Files.Length; i++)
                {
                    var f = cmb.Files[i];

                    if (f == null)
                    {
                        this.Logger.Debug(cmb.OfferGuid, $"[CMB] File {i} is null");
                        continue;
                    }

                    if (f.Content?.Length < 1)
                    {
                        this.Logger.Debug(cmb.OfferGuid, $"[CMB] File {i} has no content");
                        continue;
                    }

                    var extension = Path.GetExtension(f.Name).ToLowerInvariant();

                    if (!cmb.AllowedExtensions.Contains(extension))
                    {
                        this.Logger.Debug(cmb.OfferGuid, $"[CMB] File {i} ({f.Name}) has invalid extension");
                        cmb.Errors.Add(datasource.ErrorInvalidFile);
                    }

                    if (f.Content.Length > datasource.SettingsMaxFileSize)
                    {
                        this.Logger.Debug(cmb.OfferGuid, $"[CMB] File {i} ({f.Name}) is too big ({f.Content.Length})");
                        cmb.Errors.Add(datasource.ErrorTooBigFile);
                    }

                    var file = new CallMeBackFileDataModel();
                    file.Filename = f.Name;
                    file.Description = $"Příloha {i + 1}";
                    file.Mimetype = f.MimeType;
                    file.Content = Convert.ToBase64String(f.Content);
                    files.Add(file);
                }
            }

            return files.ToArray();
        }

        protected internal CallMeBackAttributeDataModel[] GetParameters(CallMeBackModel cmb, ICallMeBackModalWindow datasource)
        {
            var callTime = this.GetCallTime(cmb);
            var list = new List<CallMeBackAttributeDataModel>();
            list.Add(new CallMeBackAttributeDataModel("processtype", datasource.SettingsProcessType));
            list.Add(new CallMeBackAttributeDataModel("cmbid", datasource.ID.ToString()));
            //list.Add(new CallMeBackAttributeDataModel("url", cmb.RequestedUrl));
            list.Add(new CallMeBackAttributeDataModel("pageTitle", datasource.Title));
            list.Add(new CallMeBackAttributeDataModel("preferredTime", cmb.SelectedTime));
            list.Add(new CallMeBackAttributeDataModel("callTime", callTime));
            return list.ToArray();
        }

        protected internal string GetCallTime(CallMeBackModel model)
        {
            if (string.IsNullOrEmpty(model.SelectedTime))
            {
                return EMPTY_SAP_DATE;
            }

            string timeFrom = string.Empty;
            string timeTo = string.Empty;
            int hoursFrom = 0;

            var times = model.SelectedTime.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (times.Length == 0)
            {
                this.Logger.Warn(model.OfferGuid, $"[CMB] Selected time ({model.SelectedTime}) doesn't contain range of times. Returning default '{EMPTY_SAP_DATE}'.");
                return EMPTY_SAP_DATE;
            }

            if (times.Length == 1)
            {
                this.Logger.Warn(model.OfferGuid, $"[CMB] Selected time ({model.SelectedTime}) doesn't contain range of times. Taking first.");
                timeFrom = times[0];
            }
            else
            {
                timeFrom = times[0];
                timeTo = times[1];
            }

            if (!timeFrom.Contains(":"))
            {
                this.Logger.Debug(model.OfferGuid, $"[CMB] Time ({timeFrom}) doesn't contain ':' as divider for hh:mm.");

                if (!int.TryParse(timeFrom, out hoursFrom))
                {
                    this.Logger.Warn(model.OfferGuid, $"[CMB] Time ({timeFrom}) is not integer. Returning default '{EMPTY_SAP_DATE}'.");
                    return EMPTY_SAP_DATE;
                }
            }
            else
            {
                if (!int.TryParse(timeFrom.Split(':').First(), out hoursFrom))
                {
                    this.Logger.Warn(model.OfferGuid, $"[CMB] First part of time ({timeFrom}) is not integer. Returning default '{EMPTY_SAP_DATE}'.");
                    return EMPTY_SAP_DATE;
                }
            }

            var plannedHour = DateTime.Today.AddHours(hoursFrom);

            if (plannedHour < DateTime.Now)
            {
                // when it's in past, moving 1 day forward
                plannedHour = plannedHour.AddDays(1);
            }

            if (plannedHour.DayOfWeek == DayOfWeek.Saturday || plannedHour.DayOfWeek == DayOfWeek.Sunday)
            {
                // when it's weekend, moving to Monday
                plannedHour = plannedHour.AddDays(((int)DayOfWeek.Monday - (int)plannedHour.DayOfWeek + 7) % 7);
            }

            return plannedHour.ToUniversalTime().ToString(SAP_DATE_FORMAT);
        }

        private string GetRequestLogMessage(HttpWebRequest request, CallMeBackModel model, UserCacheDataModel userData, string token)
        {
            var logBuilder = new StringBuilder();
            logBuilder.AppendLine("[CMB] Request for CMB data:");
            logBuilder.AppendLine($" - Method: {request.Method}");
            logBuilder.AppendLine($" - CSRF token: {token}");

            if (userData.IsCognito && !string.IsNullOrEmpty(userData.Tokens.AccessToken))
            {
                logBuilder.AppendLine($" - Access token: {userData.Tokens.AccessToken.Substring(0, 50)} ...");
            }

            logBuilder.AppendLine(" - Headers:");

            foreach (var key in request.Headers.AllKeys)
            {
                logBuilder.AppendLine($"   - {key}: {request.Headers[key]}");
            }

            return logBuilder.ToString();
        }

        private string GetResponseLogMessage(HttpWebResponse response)
        {
            var statusCode = (int)response.StatusCode;

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine("[CMB] Response for CMB data retrieved:");
            logBuilder.AppendLine($" - Status code: {statusCode}");
            logBuilder.AppendLine($" - Message: {response.StatusDescription}");

            logBuilder.AppendLine(" - Headers:");

            foreach (var key in response.Headers.AllKeys)
            {
                logBuilder.AppendLine($"   - {key}: {response.Headers[key]}");
            }

            if (response.StatusCode == HttpStatusCode.Created)
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var result = reader.ReadToEnd();
                    logBuilder.AppendLine($" - Body: {result}");
                }
            }

            return logBuilder.ToString();
        }

        protected internal class CallMeBackDataModel
        {
            public string Description { get; set; }
            public string Partner { get; set; }
            public string Note { get; set; }
            public string Eicean { get; set; }
            public string PhoneNumber { get; set; }
            public string Process { get; set; }
            public string ExternalSystem { get; set; }
            public string ExternalKey { get; set; }
            public Collection<CallMeBackFileDataModel> ZCreateActivityFilesSet { get; set; }
            public Collection<CallMeBackAttributeDataModel> ZCreateActivityParametersSet { get; set; }
        }

        protected internal class CallMeBackFileDataModel
        {
            public string Filename { get; set; }
            public string Description { get; set; }
            public string Mimetype { get; set; }
            public string Content { get; set; }
            public Guid Guid { get; set; }
        }

        protected internal class CallMeBackAttributeDataModel
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public CallMeBackAttributeDataModel(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }
    }
}
