using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Kernel.Models;
using MongoDB.Driver;
using Sitecore.Diagnostics;

namespace eContracting.Kernel.Services
{
    public class MongoService
    {
        private const string OfferLoginCollectionName = "OfferLoginReport";
        private const string ConnectionStringItem = "OfferDB";

        public MongoClient MongoClient { get; private set; }
        public IMongoDatabase MongoDatabase { get; private set; }
        public IMongoCollection<OfferLoginReportModel> OfferLoginReportCollection { get; private set; }

        public MongoService()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringItem];
            var connectionStringValue = connectionStringSettings.ConnectionString;
            var mongoUrl = new MongoUrl(connectionStringValue);
            var mongoSettings = MongoClientSettings.FromUrl(mongoUrl);
            this.MongoClient = new MongoClient(mongoSettings);
            this.MongoDatabase = this.MongoClient.GetDatabase(mongoUrl.DatabaseName);
            this.OfferLoginReportCollection = this.MongoDatabase.GetCollection<OfferLoginReportModel>(OfferLoginCollectionName);
        }

        public void AddOfferLoginAttempt(string sessionId, string timestamp, string guid, string type, bool birthdayDate = true, bool wrongPostalCode = false , bool WrongResidencePostalCode = false, bool WrongIdentityCardNumber = false)
        {
            try
            {
                guid = this.NormalizeGuid(guid);

                var builder = Builders<OfferLoginReportModel>.Filter;
                var filter = builder.Eq(x => x.Guid, guid);
                var result = this.OfferLoginReportCollection.FindSync(filter);
                var model = result.FirstOrDefault();

                if (model == null)
                {
                    var m = new FailedLoginInfoModel
                    {
                        Guid = guid,
                        LastFailedAttemptTimestamp = DateTime.UtcNow,
                        FailedAttempts = 1,
                        FailedAttemptsInfo = new List<FailedLoginInfoDetailModel>() { recor }
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
                        .AddToSet(x => x.FailedAttemptsInfo, recor);

                    this.Collection.UpdateOne(updateFilter, update);

                    Log.Info($"[{guid}] Failed log-in attempt increased to {value}", this);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Cannot update failed log-in attempts", ex, this);
            }
        }

        private string NormalizeGuid(string guid)
        {
            return guid?.ToUpperInvariant();
        }

    }
}
