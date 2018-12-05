using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Kernel.Models;
using MongoDB.Driver;
using Sitecore.Analytics.Data.DataAccess.MongoDb;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace eContracting.Kernel.Services
{
    public class LoginsCheckerClient
    {
        protected int MaxFailedAttemps { get; set; }
        protected TimeSpan DelayAfterFailedAttemps { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginsCheckerClient"/> class.
        /// </summary>
        public LoginsCheckerClient() : this(
            Settings.GetIntSetting("eContracting.MaxFailedAttempts", 3),
            Settings.GetTimeSpanSetting("eContracting.DelayAfterFailedAttempts", new TimeSpan(0,30,0)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginsCheckerClient"/> class.
        /// </summary>
        /// <param name="maxFailedAttempts">The maximum failed attempts.</param>
        /// <param name="delayAfterFailedAttempts">The delay after failed attempts.</param>
        public LoginsCheckerClient(int maxFailedAttempts, TimeSpan delayAfterFailedAttempts)
        {
            this.MaxFailedAttemps = maxFailedAttempts;
            this.DelayAfterFailedAttemps = delayAfterFailedAttempts;
        }

        /// <summary>
        /// Determines whether user with <paramref name="guid"/> can log-in.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///   <c>true</c> if this instance can login the specified unique identifier; otherwise, <c>false</c>.
        /// </returns>
        public bool CanLogin(string guid)
        {
            try
            {
                guid = this.NormalizeGuid(guid);

                FilterDefinitionBuilder<FailedLoginInfoModel> builder = Builders<FailedLoginInfoModel>.Filter;
                FilterDefinition<FailedLoginInfoModel> filter = builder.Eq(x => x.Guid, guid);

                IMongoCollection<FailedLoginInfoModel> collection = this.GetLoginsCollection();
                IFindFluent<FailedLoginInfoModel, FailedLoginInfoModel> find = collection.Find(filter);

                if (find.Count() == 0)
                {
                    Log.Debug("User '" + guid + "' can log-in (nothing found)", this);
                    return true;
                }

                var result = find.FirstOrDefault();

                // just to be sure
                if (result == null)
                {
                    Log.Debug("User '" + guid + "' can log-in (no result)", this);
                    return true;
                }

                // If number of failed logins if less then 'MaxFailedAttemps', user still can try to log-in
                if (result.FailedAttempts < this.MaxFailedAttemps)
                {
                    Log.Debug("User '" + guid + "' can log-in (number of failed attempts: " + result.FailedAttempts + ")", this);
                    return true;
                }

                // if logins for 'guid' exceeded number of failed attempts and last time was before 'DelayAfterFailedAttemps',
                // use can login and his data must be updated (reset values)
                if (result.LastFailedAttemptTimestamp.Add(this.DelayAfterFailedAttemps) < DateTime.Now)
                {
                    try
                    {
                        FilterDefinitionBuilder<FailedLoginInfoModel> deleteBuilder = Builders<FailedLoginInfoModel>.Filter;
                        FilterDefinition<FailedLoginInfoModel> deleteFilter = deleteBuilder.Eq(x => x.Id, result.Id);

                        collection.DeleteOne(deleteFilter);

                        //var update = Builders<LoginGuidInfo>.Update
                        //    .PullAll(x => x.FailedAttemptsInfo, result.FailedAttemptsInfo)
                        //    .Set(x => x.FailedAttempts, 0)
                        //    .Set(x => x.LastFailedAttemptTimestamp, DateTime.MinValue);

                        //collection.UpdateOne(updateFilter, update);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal("Cannot clear information about failed logins for user '" + guid + "'. Records can grow up to infinity! Please fix!", ex, this);
                    }

                    Log.Debug("User '" + guid + "' can log-in (had max failed login attempts but waiting time expired)", this);

                    return true;
                }
                else
                {
                    TimeSpan difference = result.LastFailedAttemptTimestamp.Add(this.DelayAfterFailedAttemps) - DateTime.Now;

                    Log.Debug("User '" + guid + "' cannot log-in (had max failed login attempts and waiting time not expired yet - " + difference.ToString("c") + ")", this);
                    return false;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Cannot validate if user '" + guid + "' had failed login attempts. We must allow him to log-in.", exception, this);
                return true;
            }
        }

        /// <summary>
        /// Adds the failed attempt.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="browserInfo">The browser information.</param>
        public void AddFailedAttempt(string guid, string sessionId, string browserInfo)
        {
            try
            {
                guid = this.NormalizeGuid(guid);

                FailedLoginInfoDetailModel infoRecord = new FailedLoginInfoDetailModel()
                {
                    Timestamp = DateTime.Now,
                    SessionId = sessionId,
                    BrowserInfo = browserInfo
                };

                FilterDefinitionBuilder<FailedLoginInfoModel> builder = Builders<FailedLoginInfoModel>.Filter;
                FilterDefinition<FailedLoginInfoModel> filter = builder.Eq(x => x.Guid, guid);

                IMongoCollection<FailedLoginInfoModel> collection = this.GetLoginsCollection();
                IFindFluent<FailedLoginInfoModel, FailedLoginInfoModel> result = collection.Find(filter);

                FailedLoginInfoModel model = result.FirstOrDefault();

                if (model == null)
                {
                    FailedLoginInfoModel m = new FailedLoginInfoModel();
                    m.Guid = guid;
                    m.LastFailedAttemptTimestamp = DateTime.Now;
                    m.FailedAttempts = 1;
                    m.FailedAttemptsInfo = new List<FailedLoginInfoDetailModel>() { infoRecord };

                    collection.InsertOne(m);

                    Log.Debug("New failed attempt record created for user '" + guid + "'", this);
                }
                else
                {
                    FilterDefinitionBuilder<FailedLoginInfoModel> updateBuilder = Builders<FailedLoginInfoModel>.Filter;
                    FilterDefinition<FailedLoginInfoModel> updateFilter = updateBuilder.Eq(x => x.Id, model.Id);

                    UpdateDefinition<FailedLoginInfoModel> update = Builders<FailedLoginInfoModel>.Update
                        .Set(x => x.FailedAttempts, model.FailedAttempts + 1)
                        .Set(x => x.LastFailedAttemptTimestamp, DateTime.Now)
                        .AddToSet(x => x.FailedAttemptsInfo, infoRecord);

                    collection.UpdateOne(updateFilter, update);

                    Log.Debug("New failed attempt record added for user '" + guid + "'", this);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Cannot update failed login attempt for user '" + guid + "'", ex, this);
            }
        }

        /// <summary>
        /// Gets the MongoDB collection for failed logins.
        /// </summary>
        /// <returns>Instance of <see cref="IMongoCollection{LoginGuidInfo}"/></returns>
        protected IMongoCollection<FailedLoginInfoModel> GetLoginsCollection()
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["OfferDB"];
            string connectionStringValue = connectionStringSettings.ConnectionString;
            MongoUrl mongoUrl = new MongoUrl(connectionStringValue);
            MongoClientSettings mongoSettings = MongoClientSettings.FromUrl(mongoUrl);
            MongoClient client = new MongoClient(mongoSettings);
            IMongoDatabase database = client.GetDatabase(mongoUrl.DatabaseName);
            IMongoCollection<FailedLoginInfoModel> collection = database.GetCollection<FailedLoginInfoModel>("FailedLoginAttempts");
            return collection;
        }

        /// <summary>
        /// Normalizes the unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Upper case value of <paramref name="guid"/></returns>
        protected string NormalizeGuid(string guid)
        {
            return guid?.ToUpperInvariant();
        }
    }
}
