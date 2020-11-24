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
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var model = new JsonOfferNotAcceptedModel();

            if (offer.Version > 1)
            {
                model.Perex = this.GetPerex(offer.TextParameters, definition);
                model.Gifts = this.GetGifts(offer.TextParameters, definition);
                model.Benefits = this.GetBenefits(offer.TextParameters, definition);
            }

            model.Documents = await this.GetDocuments(offer, definition);

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

            if (parameters.Count > 0)
            {
                var model = new JsonOfferPerexModel();
                model.Title = "perex";
                model.Parameters = parameters.ToArray();
                return model;
            }

            return null;
        }

        protected internal JsonBenefitsModel GetBenefits(IDictionary<string, string> textParameters, DefinitionCombinationModel definition)
        {
            var values = textParameters.Where(x => x.Key.StartsWith("COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE")).Select(x => x.Value).ToArray();

            if (values.Length == 0)
            {
                return null;
            }

            var model = new JsonBenefitsModel();
            model.Title = "gifts";
            model.Params = values.Select(x => new JsonArgumentModel(x)).ToArray();
            return model;
        }

        protected internal JsonGiftsModel GetGifts(IDictionary<string, string> textParameters, DefinitionCombinationModel definition)
        {
            if (!this.IsSectionChecked(textParameters, "BENEFITS"))
            {
                return null;
            }

            var keys = new[] { "BENEFITS_NOW", "BENEFITS_NEXT_SIGN", "BENEFITS_NEXT_TZD" };
            var groups = new List<JsonGiftsGroupModel>();

            for (int i = 0; i < keys.Length; i++)
            {
                var g = this.GetBenefitGroup(keys[i], textParameters);

                if (g != null)
                {
                    groups.Add(g);
                }
            }

            if (groups.Count == 0)
            {
                return null;
            }

            var model = new JsonGiftsModel();
            model.Title = "TITLE";

            if (textParameters.HasValue("BENEFITS_CLOSE"))
            {
                model.Note = textParameters["BENEFITS_CLOSE"];
            }

            model.Groups = groups;
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

        protected internal JsonGiftsGroupModel GetBenefitGroup(string key, IDictionary<string, string> textParameters)
        {
            string keyIntro = key + "_INTRO";
            string keyCount = key + "_COUNT_";
            string keyImage = key + "_IMAGE_";
            string keyName = key + "_NAME_";

            if (!this.IsSectionChecked(textParameters, key))
            {
                return null;
            }

            var keys = textParameters.Keys.Where(x => x.StartsWith(key)).ToArray();

            var group = new JsonGiftsGroupModel();

            if (keys.Contains(keyIntro))
            {
                group.Title = textParameters[keyIntro];
            }

            var count = textParameters.Keys.Where(x => x.StartsWith(keyName)).Count();

            if (count == 0)
            {
                return null;
            }

            var list = new List<JsonGiftModel>();

            for (int i = 0; i < count; i++)
            {
                var benefit = new JsonGiftModel();

                if (keys.Contains($"{keyCount}_{i}") && int.TryParse(textParameters[$"{keyCount}_{i}"], out int c))
                {
                    benefit.Count = c;
                }

                if (keys.Contains($"{keyName}_{i}"))
                {
                    benefit.Title = textParameters[$"{keyName}_{i}"];
                }

                if (keys.Contains($"{keyImage}_{i}"))
                {
                    benefit.Icon = textParameters[$"{keyImage}_{i}"];
                }

                list.Add(benefit);
            }

            group.Params = list;
            return group;
        }

        protected internal async Task<JsonOfferDocumentsModel> GetDocuments(OfferModel offer, DefinitionCombinationModel definition)
        {
            if (!this.IsSectionChecked(offer.TextParameters, "COMMODITY"))
            {
                return null;
            }

            var files = await this.ApiService.GetAttachmentsAsync(offer);

            var model = new JsonOfferDocumentsModel();
            model.Acceptance = this.GetDocumentsAcceptance(offer, files, definition);
            return model;
        }

        protected internal JsonDocumentsAcceptanceModel GetDocumentsAcceptance(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            return null;
        }

        /// <summary>
        /// Determines whether section <paramref name="key"/> has value 'X' (&lt;SECTION&gt;X&lt;/SECTION&gt;)
        /// </summary>
        /// <param name="textParameters">The text parameters.</param>
        /// <param name="key">The key.</param>
        protected internal bool IsSectionChecked(IDictionary<string, string> textParameters, string key)
        {
            if (!textParameters.ContainsKey(key))
            {
                return false;
            }

            var value = textParameters[key];

            return value.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
