using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using eContracting.Kernel.Models;
using MongoDB.Driver;
using Sitecore.Diagnostics;

namespace eContracting.Kernel.Services
{
    public class MongoOfferLoginReportService
    {
        private const string OfferLoginCollectionName = "OfferLoginReport";
        private const string ConnectionStringItem = "OfferDB";

        private MongoClient MongoClient { get; set; }
        private IMongoDatabase MongoDatabase { get; set; }
        private IMongoCollection<OfferLoginReportModel> OfferLoginReportCollection { get; set; }

        public MongoOfferLoginReportService()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringItem];
            var connectionStringValue = connectionStringSettings.ConnectionString;
            var mongoUrl = new MongoUrl(connectionStringValue);
            var mongoSettings = MongoClientSettings.FromUrl(mongoUrl);
            this.MongoClient = new MongoClient(mongoSettings);
            this.MongoDatabase = this.MongoClient.GetDatabase(mongoUrl.DatabaseName);
            this.OfferLoginReportCollection = this.MongoDatabase.GetCollection<OfferLoginReportModel>(OfferLoginCollectionName);
        }

        public void AddOfferLoginAttempt(string sessionId, string timestamp, string guid, string type, bool birthdayDate = false, bool wrongPostalCode = false , bool WrongResidencePostalCode = false, bool WrongIdentityCardNumber = false, bool generalError = false)
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
                Log.Error($"[{guid}] Cannot update failed log-in attempts", ex, this);
            }
        }

        private void InsertNewLoginReportItem(OfferLoginReportModel newModel)
        {
            try
            {
                this.OfferLoginReportCollection.InsertOne(newModel);

                Log.Info($"[{newModel.Guid}] Inserted new log-in attempt", this);

            }
            catch (Exception ex)
            {
                Log.Error($"[{newModel.Guid}] Failed to Inserted new log-in attempt: {ex.Message}", this);
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

                this.OfferLoginReportCollection.UpdateOne(updateFilter, finalUpd);

                Log.Info($"[{current.Guid}] Updated offer login report.", this);
            }
            catch (Exception ex)
            {
                Log.Info($"[{current.Guid}] Failed to update offer login report: {ex.Message}", this);
            }
        }

        private OfferLoginReportModel GetOfferLoginReportModel(bool birthdayDate, bool wrongPostalCode, bool wrongResidencePostalCode, bool wrongIdentityCardNumber, bool generalError)
        {
            var list = new List<bool> { birthdayDate, wrongPostalCode, wrongResidencePostalCode, wrongIdentityCardNumber };
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
            var result = this.OfferLoginReportCollection.FindSync(filter);
            var model = result.FirstOrDefault();

            return model;
        }

        private string NormalizeGuid(string guid)
        {
            return guid?.ToUpperInvariant();
        }
    }
}
