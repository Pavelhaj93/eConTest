using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using eContracting.Models;
using MongoDB.Driver;

namespace eContracting.Services
{
    public class MongoLoginReportStore : ILoginReportStore
    {
        private const string OfferLoginCollectionName = "OfferLoginReport";
        private const string FailedLoginAttemptsCollectionName = "FailedLoginAttempts";
        private const string ConnectionStringItem = "OfferDB";

        /// <summary>
        /// The client
        /// </summary>
        protected readonly MongoClient MongoClient;

        /// <summary>
        /// The database of the collections.
        /// </summary>
        protected readonly IMongoDatabase MongoDatabase;

        protected readonly ILogger Logger;
        protected readonly ISettingsReaderService Settings;

        /// <summary>
        /// Collection of all login attempts.
        /// </summary>
        protected readonly IMongoCollection<OfferLoginReportModel> LoginsLogCollection;

        /// <summary>
        /// Collection of failed login attempts.
        /// </summary>
        protected readonly IMongoCollection<FailedLoginInfoModel> FailedLoginsCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoLoginReportStore"/> class.
        /// </summary>
        public MongoLoginReportStore(ILogger logger, ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger;
            this.Settings = settingsReaderService;
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringItem];
            var connectionStringValue = connectionStringSettings.ConnectionString;
            var mongoUrl = new MongoUrl(connectionStringValue);
            var mongoSettings = MongoClientSettings.FromUrl(mongoUrl);
            this.MongoClient = new MongoClient(mongoSettings);
            this.MongoDatabase = this.MongoClient.GetDatabase(mongoUrl.DatabaseName);
            this.LoginsLogCollection = this.MongoDatabase.GetCollection<OfferLoginReportModel>(OfferLoginCollectionName);
            this.FailedLoginsCollection = this.MongoDatabase.GetCollection<FailedLoginInfoModel>(FailedLoginAttemptsCollectionName);
        }

        /// <inheritdoc/>
        public void AddLoginAttempt(string sessionId, string timestamp, string guid, string type, bool birthdayDate = false, bool wrongPostalCode = false, bool WrongResidencePostalCode = false, bool WrongIdentityCardNumber = false, bool generalError = false)
        {
            try
            {
                guid = this.NormalizeGuid(guid);
                var model = this.GetStoredOfferLoginReportModel(guid, sessionId);

                var currentModel = this.GetOfferLoginReportModel(birthdayDate, wrongPostalCode, WrongResidencePostalCode, WrongIdentityCardNumber, generalError);
                currentModel.SessionId = sessionId;
                currentModel.LastLoginTime = timestamp;
                currentModel.Guid = guid;
                currentModel.OfferTypeIdentifier = type;

                if (model == null)
                {
                    this.InsertNewLoginReportItem(currentModel);
                }
                else
                {
                    this.UpdateLoginReportItem(model, currentModel);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{guid}] Cannot update failed log-in attempts", ex);
            }
        }

        public bool CanLogin(string guid)
        {
            var siteSettings = this.Settings.GetSiteSettings();
            return this.CanLogin(guid, siteSettings.MaxFailedAttempts, siteSettings.DelayAfterFailedAttemptsTimeSpan);
        }

        /// <inheritdoc/>
        public bool CanLogin(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts)
        {
            try
            {
                guid = this.NormalizeGuid(guid);

                var builder = Builders<FailedLoginInfoModel>.Filter;
                var filter = builder.Eq(x => x.Guid, guid);
                var find = this.FailedLoginsCollection.FindSync(filter);
                var result = find.FirstOrDefault();

                // just to be sure
                if (result == null)
                {
                    this.Logger.Info($"[{guid}] Can log-in (no result)");
                    return true;
                }

                // If number of failed logins if less then 'MaxFailedAttemps', user still can try to log-in
                if (result.FailedAttempts < maxFailedAttempts)
                {
                    this.Logger.Info($"[{guid}] Can log-in (number of failed attempts: {result.FailedAttempts})");
                    return true;
                }

                // if logins for 'guid' exceeded number of failed attempts and last time was before 'DelayAfterFailedAttemps',
                // use can login and his data must be updated (reset values)
                if (result.LastFailedAttemptTimestamp.Add(delayAfterFailedAttempts) < DateTime.UtcNow)
                {
                    try
                    {
                        FilterDefinitionBuilder<FailedLoginInfoModel> deleteBuilder = Builders<FailedLoginInfoModel>.Filter;
                        FilterDefinition<FailedLoginInfoModel> deleteFilter = deleteBuilder.Eq(x => x.Id, result.Id);

                        this.FailedLoginsCollection.DeleteOne(deleteFilter);

                        this.Logger.Info($"[{guid}] Failed log-in attempts were deleted due to expiration");

                        //var update = Builders<LoginGuidInfo>.Update
                        //    .PullAll(x => x.FailedAttemptsInfo, result.FailedAttemptsInfo)
                        //    .Set(x => x.FailedAttempts, 0)
                        //    .Set(x => x.LastFailedAttemptTimestamp, DateTime.MinValue);

                        //collection.UpdateOne(updateFilter, update);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Fatal($"[{guid}] Cannot clear information about failed log-ins. Records can grow up to infinity! Please fix!", ex);
                    }

                    this.Logger.Info($"[{guid}] Can log-in (had max failed log-in attempts but waiting for time to expire)");

                    return true;
                }
                else
                {
                    TimeSpan difference = result.LastFailedAttemptTimestamp.Add(delayAfterFailedAttempts) - DateTime.UtcNow;

                    this.Logger.Info($"[{guid}] Cannot log-in (max failed log-in attempts ({maxFailedAttempts}) reached and waiting time not expired yet - {difference.ToString("c")})");
                    return false;
                }
            }
            catch (ApplicationException exception)
            {
                this.Logger.Error($"[{guid}] Cannot validate if had failed log-in attempts. We must allow him to log-in.", exception);
                return true;
            }
        }

        /// <inheritdoc/>
        public void AddFailedAttempt(string guid, string sessionId, string browserInfo)
        {
            try
            {
                guid = this.NormalizeGuid(guid);

                var infoRecord = new FailedLoginInfoDetailModel()
                {
                    Timestamp = DateTime.UtcNow,
                    SessionId = sessionId,
                    BrowserInfo = browserInfo
                };

                var builder = Builders<FailedLoginInfoModel>.Filter;
                var filter = builder.Eq(x => x.Guid, guid);
                var result = this.FailedLoginsCollection.FindSync(filter);
                var model = result.FirstOrDefault();

                if (model == null)
                {
                    var m = new FailedLoginInfoModel
                    {
                        Guid = guid,
                        LastFailedAttemptTimestamp = DateTime.UtcNow,
                        FailedAttempts = 1,
                        FailedAttemptsInfo = new List<FailedLoginInfoDetailModel>() { infoRecord }
                    };

                    this.FailedLoginsCollection.InsertOne(m);

                    this.Logger.Info($"[{guid}] Failed log-in attempt increased to 1");
                }
                else
                {
                    var value = model.FailedAttempts + 1;
                    var updateBuilder = Builders<FailedLoginInfoModel>.Filter;
                    var updateFilter = updateBuilder.Eq(x => x.Id, model.Id);
                    var update = Builders<FailedLoginInfoModel>.Update
                        .Set(x => x.FailedAttempts, value)
                        .Set(x => x.LastFailedAttemptTimestamp, DateTime.UtcNow)
                        .AddToSet(x => x.FailedAttemptsInfo, infoRecord);

                    this.FailedLoginsCollection.UpdateOne(updateFilter, update);

                    this.Logger.Info($"[{guid}] Failed log-in attempt increased to {value}");
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{guid}] Cannot update failed log-in attempts", ex);
            }
        }

        private void InsertNewLoginReportItem(OfferLoginReportModel newModel)
        {
            try
            {
                this.LoginsLogCollection.InsertOne(newModel);

                this.Logger.Info($"[{newModel.Guid}] Inserted new log-in attempt");

            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{newModel.Guid}] Failed to Inserted new log-in attempt: {ex.Message}");
            }
        }

        private void UpdateLoginReportItem(OfferLoginReportModel model, OfferLoginReportModel current)
        {
            try
            {
                var updateBuilder = Builders<OfferLoginReportModel>.Filter;
                var updateFilter = updateBuilder.Eq(x => x.Id, model.Id);

                var updateList = new List<UpdateDefinition<OfferLoginReportModel>>()
                {
                    Builders<OfferLoginReportModel>.Update.Set(x => x.LastLoginTime, current.LastLoginTime)
                };

                if (current.SuccessAttemptCount > 0)
                {
                    updateList.Add(Builders<OfferLoginReportModel>.Update.Set(x => x.SuccessAttemptCount, ++model.SuccessAttemptCount));
                }

                if (current.WrongBirthdayDateCount > 0)
                {
                    updateList.Add(Builders<OfferLoginReportModel>.Update.Set(x => x.WrongBirthdayDateCount, ++model.WrongBirthdayDateCount));
                }

                if (current.WrongIdentityCardNumberCount > 0)
                {
                    updateList.Add(Builders<OfferLoginReportModel>.Update.Set(x => x.WrongIdentityCardNumberCount, ++model.WrongIdentityCardNumberCount));
                }

                if (current.GeneralErrorCount > 0)
                {
                    updateList.Add(Builders<OfferLoginReportModel>.Update.Set(x => x.GeneralErrorCount, ++model.GeneralErrorCount));
                }

                if (current.WrongPostalCodeCount > 0)
                {
                    updateList.Add(Builders<OfferLoginReportModel>.Update.Set(x => x.WrongPostalCodeCount, ++model.WrongPostalCodeCount));
                }

                if (current.WrongResidencePostalCodeCount > 0)
                {
                    updateList.Add(Builders<OfferLoginReportModel>.Update.Set(x => x.WrongResidencePostalCodeCount, ++model.WrongResidencePostalCodeCount));
                }

                var finalUpd = Builders<OfferLoginReportModel>.Update.Combine(updateList);

                this.LoginsLogCollection.UpdateOne(updateFilter, finalUpd);

                this.Logger.Info($"[{current.Guid}] Updated offer login report.");
            }
            catch (Exception ex)
            {
                this.Logger.Info($"[{current.Guid}] Failed to update offer login report: {ex.Message}");
            }
        }

        private OfferLoginReportModel GetOfferLoginReportModel(bool birthdayDate, bool wrongPostalCode, bool wrongResidencePostalCode, bool wrongIdentityCardNumber, bool generalError)
        {
            var list = new List<bool> { birthdayDate, wrongPostalCode, wrongResidencePostalCode, wrongIdentityCardNumber, generalError };
            var res = new OfferLoginReportModel
            {
                WrongBirthdayDateCount = birthdayDate ? 1 : 0,
                WrongPostalCodeCount = wrongPostalCode ? 1 : 0,
                WrongResidencePostalCodeCount = wrongResidencePostalCode ? 1 : 0,
                WrongIdentityCardNumberCount = wrongIdentityCardNumber ? 1 : 0,
                GeneralErrorCount = generalError ? 1 : 0,
                SuccessAttemptCount = list.Any(a => a) ? 0 : 1
            };

            return res;
        }

        private OfferLoginReportModel GetStoredOfferLoginReportModel(string guid, string sessionId)
        {
            var builder = Builders<OfferLoginReportModel>.Filter;
            var filter = (builder.Eq(x => x.Guid, guid)) & (builder.Eq(x => x.SessionId, sessionId));
            var result = this.LoginsLogCollection.FindSync(filter);
            var model = result.FirstOrDefault();

            return model;
        }

        /// <summary>
        /// Normalizes the unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Upper case value of <paramref name="guid"/></returns>
        private string NormalizeGuid(string guid)
        {
            return guid?.ToUpperInvariant();
        }
    }
}
