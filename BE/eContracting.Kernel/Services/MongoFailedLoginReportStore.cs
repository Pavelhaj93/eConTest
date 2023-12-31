using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eContracting.Kernel.Models;
using MongoDB.Driver;
using Sitecore.Analytics.Data.DataAccess.MongoDb;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace eContracting.Kernel.Services
{
    public class MongoFailedLoginReportStore : IFailedLoginReportStore
    {
        /// <summary>
        /// The collection
        /// </summary>
        protected readonly IMongoCollection<FailedLoginInfoModel> Collection;

        /// <summary>
        /// The collection
        /// </summary>
        protected readonly IMongoCollection<OfferLoginReportModel> OfferLoginCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoFailedLoginReportStore"/> class.
        /// </summary>
        public MongoFailedLoginReportStore() : this(GetLoginsCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoFailedLoginReportStore"/> class.
        /// </summary>
        /// <param name="maxFailedAttempts">The maximum failed attempts.</param>
        /// <param name="delayAfterFailedAttempts">The delay after failed attempts.</param>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">collection</exception>
        public MongoFailedLoginReportStore(IMongoCollection<FailedLoginInfoModel> collection)
        {
            this.Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        /// <summary>
        /// Determines whether user with <paramref name="guid"/> can log-in.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///   <c>true</c> if this instance can login the specified unique identifier; otherwise, <c>false</c>.
        /// </returns>
        public bool CanLogin(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts)
        {
            try
            {
                guid = this.NormalizeGuid(guid);

                var builder = Builders<FailedLoginInfoModel>.Filter;
                var filter = builder.Eq(x => x.Guid, guid);
                var find = this.Collection.FindSync(filter);
                var result = find.FirstOrDefault();

                // just to be sure
                if (result == null)
                {
                    Log.Info($"[{guid}] Can log-in (no result)", this);
                    return true;
                }

                // If number of failed logins if less then 'MaxFailedAttemps', user still can try to log-in
                if (result.FailedAttempts < maxFailedAttempts)
                {
                    Log.Info($"[{guid}] Can log-in (number of failed attempts: {result.FailedAttempts})", this);
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

                        this.Collection.DeleteOne(deleteFilter);

                        Log.Info($"[{guid}] Failed log-in attempts were deleted due to expiration", this);

                        //var update = Builders<LoginGuidInfo>.Update
                        //    .PullAll(x => x.FailedAttemptsInfo, result.FailedAttemptsInfo)
                        //    .Set(x => x.FailedAttempts, 0)
                        //    .Set(x => x.LastFailedAttemptTimestamp, DateTime.MinValue);

                        //collection.UpdateOne(updateFilter, update);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal($"[{guid}] Cannot clear information about failed log-ins. Records can grow up to infinity! Please fix!", ex, this);
                    }

                    Log.Info($"[{guid}] Can log-in (had max failed log-in attempts but waiting for time to expire)", this);

                    return true;
                }
                else
                {
                    TimeSpan difference = result.LastFailedAttemptTimestamp.Add(delayAfterFailedAttempts) - DateTime.UtcNow;

                    Log.Info($"[{guid}] Cannot log-in (max failed log-in attempts ({maxFailedAttempts}) reached and waiting time not expired yet - {difference.ToString("c")})", this);
                    return false;
                }
            }
            catch (Exception exception)
            {
                Log.Error($"[{guid}] Cannot validate if had failed log-in attempts. We must allow him to log-in.", exception, this);
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

                var infoRecord = new FailedLoginInfoDetailModel()
                {
                    Timestamp = DateTime.UtcNow,
                    SessionId = sessionId,
                    BrowserInfo = browserInfo
                };

                var builder = Builders<FailedLoginInfoModel>.Filter;
                var filter = builder.Eq(x => x.Guid, guid);
                var result = this.Collection.FindSync(filter);
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

                    this.Collection.InsertOne(m);

                    Log.Info($"[{guid}] Failed log-in attempt increased to 1", this);
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

                    this.Collection.UpdateOne(updateFilter, update);

                    Log.Info($"[{guid}] Failed log-in attempt increased to {value}", this);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Cannot update failed log-in attempts", ex, this);
            }
        }

        /// <summary>
        /// Gets the MongoDB collection for failed logins.
        /// </summary>
        /// <returns>Instance of <see cref="IMongoCollection{LoginGuidInfo}"/></returns>
        protected static IMongoCollection<FailedLoginInfoModel> GetLoginsCollection()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["OfferDB"];
            var connectionStringValue = connectionStringSettings.ConnectionString;
            var mongoUrl = new MongoUrl(connectionStringValue);
            var mongoSettings = MongoClientSettings.FromUrl(mongoUrl);
            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase(mongoUrl.DatabaseName);
            var collection = database.GetCollection<FailedLoginInfoModel>("FailedLoginAttempts");

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
