using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc;

namespace eContracting.Services
{
    public class OfferJsonDescriptor : IOfferJsonDescriptor
    {
        protected readonly ILogger Logger;
        protected readonly ISitecoreContext Context;
        protected readonly IApiService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ISettingsReaderService SettingsReaderService;

        public OfferJsonDescriptor(
            ILogger logger,
            ISitecoreContext context,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
        }

        public async Task<JsonOfferAcceptedModel> GetAcceptedAsync(OfferModel offer)
        {
            var groups = new List<JsonFilesSectionModel>();
            var version = offer.Version;
            var documents = offer.Documents;
            var files = await this.ApiService.GetAttachmentsAsync(offer);
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var page = this.GetAcceptedPageModel();
            var textParameters = offer.TextParameters;

            if (version == 1)
            {
                var fileGroups = files.GroupBy(x => x.Group);

                foreach (IGrouping<string, OfferAttachmentModel> fileGroup in fileGroups)
                {
                    var g = this.GetSection(fileGroup.Key, fileGroup, page, offer);
                    groups.AddRange(g);
                }
            }

            return new JsonOfferAcceptedModel(groups);
        }

        public async Task<JsonOfferNotAcceptedModel> GetNewAsync(OfferModel offer)
        {
            var model = new JsonOfferNotAcceptedModel();

            if (offer.Version > 1)
            {
                var definition = this.SettingsReaderService.GetDefinition(offer);
                model.Perex = this.GetPerex(offer.TextParameters, definition);
            }

            return await Task.FromResult(model);
        }

        protected internal IEnumerable<JsonFilesSectionModel> GetSection(string groupName, IEnumerable<OfferAttachmentModel> attachments, AcceptedOfferPageModel definition, OfferModel offer)
        {
            var list = new List<JsonFilesSectionModel>();

            if (groupName == "COMMODITY")
            {
                var acceptFiles = attachments.Where(x => x.Group == "COMMODITY" && !x.IsSignReq);
                var signFiles = attachments.Where(x => x.Group == "COMMODITY" && x.IsSignReq);

                if (acceptFiles.Any())
                {
                    var title = definition.AcceptedDocumentsTitle;
                    list.Add(new JsonFilesSectionModel(acceptFiles.Select(x => new JsonFileModel(x)), title));
                }

                if (signFiles.Any())
                {
                    var title = definition.SignedDocumentsTitle;
                    list.Add(new JsonFilesSectionModel(signFiles.Select(x => new JsonFileModel(x)), title));
                }
            }
            else if (groupName == "DLS")
            {
                var files = attachments.Where(x => x.Group == "DLS");
                var title = definition.AdditionalServicesTitle;
                list.Add(new JsonFilesSectionModel(files.Select(x => new JsonFileModel(x)), title));
            }
            else if (groupName == "NONCOMMODITY")
            {
                var files = attachments.Where(x => x.Group == "NONCOMMODITY");
                var title = definition.OthersTitle;
                list.Add(new JsonFilesSectionModel(files.Select(x => new JsonFileModel(x)), title));
            }
            else
            {
                this.Logger.Fatal(offer.Guid, $"Unknown group '{groupName}' found");
            }

            return list;
        }

        protected internal string GetGroupTitle(DefinitionCombinationModel definition, OfferAttachmentModel attachment, string groupName)
        {
            if (groupName == "COMMODITY")
            {
                return definition.OfferDocumentsForAcceptanceSection1.Text;
            }

            return null;
        }

        protected internal AcceptedOfferPageModel GetAcceptedPageModel()
        {
            Guid guid = this.SettingsReaderService.GetSiteSettings().AcceptedOffer?.TargetId ?? Guid.Empty;

            if (guid == Guid.Empty)
            {
                return new AcceptedOfferPageModel();
            }

            return this.Context.GetItem<AcceptedOfferPageModel>(guid) ?? new AcceptedOfferPageModel();
        }

        protected internal JsonOfferPerexModel GetPerex(IDictionary<string, string> textParameters, DefinitionCombinationModel definition)
        {
            var parameters = new List<JsonParamModel>();

            var model = new JsonOfferPerexModel();
            model.Title = "perex";

            var keys = textParameters.Keys.Where(x => x.StartsWith("COMMODITY_OFFER_SUMMARY_ATRIB_NAME")).ToArray();
            var values = textParameters.Keys.Where(x => x.StartsWith("COMMODITY_OFFER_SUMMARY_ATRIB_VALUE")).ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var title = textParameters[key];
                var value = this.GetEnumPairValue(key, textParameters);

                if (value != null)
                {
                    parameters.Add(new JsonParamModel(title, value));
                }
            }

            model.Parameters = parameters.ToArray();

            return model;
        }

        protected internal string GetEnumPairValue(string key, IDictionary<string, string> textParameters)
        {
            var valueKey = key.Replace("ATRIB_NAME", "ATRIB_VALUE");
            
            if (textParameters.TryGetValue(valueKey, out string value))
            {
                return value;
            }

            return null;
        }
    }
}
