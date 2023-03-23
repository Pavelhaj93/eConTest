using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    [ExcludeFromCodeCoverage]
    internal class OfferDataSapService : IOfferDataService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The settings reader service.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// The API service factory.
        /// </summary>
        protected readonly IServiceFactory ServiceFactory;

        public OfferDataSapService(ILogger logger, ISettingsReaderService settingsReaderService, IServiceFactory serviceFactory)
        {
            this.Logger = logger;
            this.SettingsReaderService = settingsReaderService;
            this.ServiceFactory = serviceFactory;
        }

        /// <summary>
        /// Gets data.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <param name="fileType">Type of the file.</param>
        /// <returns>Instance of <see cref="ResponseCacheGetModel"/> or an exception.</returns>
        public ResponseCacheGetModel GetResponse(string guid, OFFER_TYPES type, string fileType = "B")
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_GET();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
                model.IV_GEFILE = fileType;

                this.Logger.Info(guid, this.GetLogMessage(model));

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_GET)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_GETRequest(model);
                    var task = api.ZCCH_CACHE_GETAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_GETResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    var result = response.ZCCH_CACHE_GETResponse;

                    if (result == null)
                    {
                        this.Logger.Error(guid, "Response ZCCH_CACHE_GETResponse1 is null");
                        return null;
                    }

                    return new ResponseCacheGetModel(result);
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Inserts data with 'NABIDKA_PRIJ'.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="files">The files.</param>
        /// <returns>The response.</returns>
        public ResponsePutModel Put(string guid, ZCCH_ST_ATTRIB[] attributes, OfferFileXmlModel[] files)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();
            //TODO: var user = this.AuthService.GetCurrentUser();
            var type = OFFER_TYPES.QUOTPRX_PRIJ;

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_PUT();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
                model.IT_ATTRIB = attributes;
                model.IT_FILES = files.Select(x => x.File).ToArray();

                this.Logger.Info(guid, this.GetLogMessage(model));

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_PUT)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_PUTRequest(model);
                    var task = api.ZCCH_CACHE_PUTAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_PUTResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    return new ResponsePutModel(response);
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Sets the <paramref name="status"/> asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">A type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <param name="timestamp">Decimal representation of a timestamp.</param>
        /// <param name="status">Value for <see cref="ZCCH_CACHE_STATUS_SET.IV_STAT"/>.</param>
        /// <returns>Response from inner service.</returns>
        public ResponseStatusSetModel SetStatus(string guid, OFFER_TYPES type, decimal timestamp, string status)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_STATUS_SET();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);
                model.IV_STAT = status;
                model.IV_TIMESTAMP = timestamp;

                this.Logger.Info(guid, this.GetLogMessage(model));

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_STATUS_SET)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_STATUS_SETRequest(model);
                    var task = api.ZCCH_CACHE_STATUS_SETAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    return new ResponseStatusSetModel(response);
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Calling <see cref="ZCCH_CACHE_ACCESS_CHECK"/> to check if Cognito user has access to offer with <paramref name="guid"/>.
        /// </summary>
        /// <param name="user">User data with <see cref="UserCacheDataModel.Guid"/>.</param>
        /// <param name="type">A type from <see cref="OFFER_TYPES"/> collection.</param>
        /// <returns><see cref="ResponseAccessCheckModel"/> model when request was successful. If user is not Cognito, returns <c>null</c>.</returns>
        /// <exception cref="EcontractingDataException">When call to <see cref="ZCCH_CACHE_ACCESS_CHECKRequest"/> failed.</exception>
        public ResponseAccessCheckModel UserAccessCheck(string guid, string userId, OFFER_TYPES type)
        {
            var options = this.SettingsReaderService.GetApiServiceOptions();

            using (var api = this.ServiceFactory.CreateApi(options))
            {
                var model = new ZCCH_CACHE_ACCESS_CHECK();
                model.IV_CCHKEY = guid;
                model.IV_CCHTYPE = Enum.GetName(typeof(OFFER_TYPES), type);

                var attributes = new List<ZCCH_ST_ATTRIB>();
                attributes.Add(new ZCCH_ST_ATTRIB()
                {
                    ATTRID = "USER_ALIAS",
                    ATTRVAL = userId
                });

                model.IT_ATTRIB = attributes.ToArray();

                this.Logger.Info(guid, this.GetLogMessage(model));

                var stop = new Stopwatch();
                stop.Start();

                var log2 = new StringBuilder();
                log2.AppendLine($"Call to {nameof(ZCCH_CACHE_ACCESS_CHECK)} finished:");

                try
                {
                    var request = new ZCCH_CACHE_ACCESS_CHECKRequest(model);
                    var task = api.ZCCH_CACHE_ACCESS_CHECKAsync(request);
                    task.Wait();
                    var response = task.Result;
                    stop.Stop();

                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: " + response.ZCCH_CACHE_ACCESS_CHECKResponse.EV_RETCODE);
                    this.Logger.Info(guid, log2.ToString());

                    var result = response.ZCCH_CACHE_ACCESS_CHECKResponse;
                    return new ResponseAccessCheckModel(result);
                }
                catch (Exception ex)
                {
                    log2.AppendLine(" Finished in: " + stop.Elapsed.ToString("hh\\:mm\\:ss\\:fff"));
                    log2.AppendLine(" Response code: unknown");
                    this.Logger.Fatal(guid, log2.ToString(), ex);
                    throw new EcontractingDataException(new ErrorModel("ZCCH_CACHE_ACCESS_CHECK", "Cannot check user"), ex);
                }
            }
        }

        private string GetLogMessage(ZCCH_CACHE_GET model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_GET)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:  {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE: {model.IV_CCHTYPE}");
            log.AppendLine($" - IV_GEFILE:  {model.IV_GEFILE}");
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_CACHE_PUT model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_PUT)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:  {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE: {model.IV_CCHTYPE}");
            log.Append(this.GetLogMessage(model.IT_ATTRIB));
            log.Append(this.GetLogMessage(model.IT_FILES));
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_CACHE_STATUS_SET model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_STATUS_SET)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:    {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE:   {model.IV_CCHTYPE}");
            log.AppendLine($" - IV_STAT:      {model.IV_STAT}");
            log.AppendLine($" - IV_TIMESTAMP: {model.IV_TIMESTAMP}");
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_CACHE_ACCESS_CHECK model)
        {
            var log = new StringBuilder();
            log.AppendLine($"Calling {nameof(ZCCH_CACHE_ACCESS_CHECK)} with parameters:");
            log.AppendLine($" - IV_CCHKEY:  {model.IV_CCHKEY}");
            log.AppendLine($" - IV_CCHTYPE: {model.IV_CCHTYPE}");
            log.Append(this.GetLogMessage(model.IT_ATTRIB));
            return log.ToString();
        }

        private string GetLogMessage(ZCCH_ST_ATTRIB[] attributes)
        {
            var log = new StringBuilder();

            if (attributes != null && attributes.Length > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    var attr = attributes[i];

                    if (attr != null)
                    {
                        log.AppendLine($" - IT_ATTRIB[{i}]{attr.ATTRID}: {attr.ATTRVAL}");
                    }
                }
            }

            return log.ToString();
        }

        private string GetLogMessage(ZCCH_ST_FILE[] files)
        {
            var log = new StringBuilder();

            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    var f = files[i];

                    if (f != null)
                    {
                        log.AppendLine($" - IT_FILES[{i}]FILENAME:  {f.FILENAME}");
                    }
                }
            }

            return log.ToString();
        }
    }
}
