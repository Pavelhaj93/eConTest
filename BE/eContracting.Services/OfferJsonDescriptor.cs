using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web;

namespace eContracting.Services
{
    public class OfferJsonDescriptor : IOfferJsonDescriptor
    {
        protected readonly ILogger Logger;
        protected readonly ITextService TextService;
        protected readonly ISitecoreContext Context;
        protected readonly IOfferService ApiService;
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferJsonDescriptor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="textService">The text service.</param>
        /// <param name="context">The context.</param>
        /// <param name="apiService">The API service.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// textService
        /// or
        /// context
        /// or
        /// apiService
        /// or
        /// settingsReaderService
        /// </exception>
        public OfferJsonDescriptor(
            ILogger logger,
            ITextService textService,
            ISitecoreContext context,
            IOfferService apiService,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.TextService = textService ?? throw new ArgumentNullException(nameof(textService));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
        }

        /// <inheritdoc/>
        public JsonOfferAcceptedModel GetAccepted(OfferModel offer)
        {
            var groups = new List<JsonFilesSectionModel>();
            var version = offer.Version;
            var documents = offer.Documents;
            var files = this.ApiService.GetAttachments(offer);
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var page = this.GetAcceptedPageModel();
            var textParameters = offer.TextParameters;
            var fileGroups = files.GroupBy(x => x.Group);

            foreach (IGrouping<string, OfferAttachmentModel> fileGroup in fileGroups)
            {
                var g = this.GetSection(fileGroup.Key, fileGroup, page, offer);
                groups.AddRange(g);
            }

            return new JsonOfferAcceptedModel(groups);
        }

        /// <inheritdoc/>
        public JsonOfferNotAcceptedModel GetNew(OfferModel offer)
        {
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var model = new JsonOfferNotAcceptedModel();

            if (offer.Version > 1)
            {
                model.Perex = this.GetPerex(offer.TextParameters, definition);
                model.Gifts = this.GetBenefits(offer.TextParameters, definition);
                model.Benefits = this.GetCommoditySalesArguments(offer.TextParameters, definition);
            }

            model.Documents = this.GetDocuments(offer, definition);
            model.AcceptanceDialog = this.GetAcceptance(offer, definition);
            return model;
        }

        protected internal IEnumerable<JsonFilesSectionModel> GetSection(string groupName, IEnumerable<OfferAttachmentModel> attachments, PageAcceptedOfferModel definition, OfferModel offer)
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
            else if (groupName == "DSL")
            {
                var files = attachments.Where(x => x.Group == "DSL");
                var title = definition.AdditionalServicesTitle;
                list.Add(new JsonFilesSectionModel(files.Select(x => new JsonFileModel(x)), title));
            }
            else
            {
                var files = attachments.Where(x => x.Group != "COMMODITY" && x.Group != "DSL" && x.IsPrinted);
                var title = definition.AdditionalServicesTitle;
                list.Add(new JsonFilesSectionModel(files.Select(x => new JsonFileModel(x)), title));
            }

            return list;
        }

        protected internal string GetGroupTitle(DefinitionCombinationModel definition, OfferAttachmentModel attachment, string groupName)
        {
            if (groupName == "COMMODITY")
            {
                return definition.OfferCommoditiesAcceptTitle.Text;
            }

            return null;
        }

        protected virtual internal PageAcceptedOfferModel GetAcceptedPageModel()
        {
            Guid guid = this.SettingsReaderService.GetSiteSettings().AcceptedOffer?.TargetId ?? Guid.Empty;

            if (guid == Guid.Empty)
            {
                return new PageAcceptedOfferModel();
            }

            return this.Context.GetItem<PageAcceptedOfferModel>(guid) ?? new PageAcceptedOfferModel();
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
                model.Title = definition.OfferPerexTitle.Text;
                model.Parameters = parameters.ToArray();
                return model;
            }

            return null;
        }

        protected internal JsonSalesArgumentsModel GetCommoditySalesArguments(IDictionary<string, string> textParameters, DefinitionCombinationModel definition)
        {
            var values = textParameters.Where(x => x.Key.StartsWith("COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE")).Select(x => x.Value).ToArray();

            if (values.Length == 0)
            {
                return null;
            }

            var model = new JsonSalesArgumentsModel();
            model.Title = definition.OfferBenefitsTitle.Text;
            model.Params = values.Select(x => new JsonArgumentModel(x)).ToArray();
            return model;
        }

        protected internal JsonAllBenefitsModel GetBenefits(IDictionary<string, string> textParameters, DefinitionCombinationModel definition)
        {
            if (!this.IsSectionChecked(textParameters, "BENEFITS"))
            {
                return null;
            }

            var keys = new[] { "BENEFITS_NOW", "BENEFITS_NEXT_SIGN", "BENEFITS_NEXT_TZD" };
            var groups = new List<JsonBenefitsGroupModel>();

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

            var model = new JsonAllBenefitsModel();
            model.Title = definition.OfferBenefitsTitle.Text;

            if (textParameters.HasValue("BENEFITS_CLOSE"))
            {
                model.Note = textParameters["BENEFITS_CLOSE"];
            }

            model.Groups = groups;
            return model;
        }

        protected internal JsonBenefitsGroupModel GetBenefitGroup(string key, IDictionary<string, string> textParameters)
        {
            string keyIntro = key + "_INTRO";
            string keyCount = key + "_COUNT";
            string keyImage = key + "_IMAGE";
            string keyName = key + "_NAME";

            if (!this.IsSectionChecked(textParameters, key))
            {
                return null;
            }

            var keys = textParameters.Keys.Where(x => x.StartsWith(key)).ToArray();

            var group = new JsonBenefitsGroupModel();

            if (keys.Contains(keyIntro))
            {
                group.Title = textParameters[keyIntro];
            }

            var list = new List<JsonBenefitModel>();
            var names = textParameters.Keys.Where(x => x.StartsWith(keyName)).ToArray();

            for (int i = 0; i < names.Length; i++)
            {
                var benefit = new JsonBenefitModel();

                var nameKey = names[i];

                if (textParameters.TryGetValue(nameKey, out string nameValue))
                {
                    benefit.Title = nameValue;
                }

                var iconKey = keys.FirstOrDefault(x => x == nameKey.Replace(keyName, keyImage));

                if (textParameters.TryGetValue(iconKey, out string iconValue))
                {
                    benefit.Icon = iconValue;
                }

                var countKey = keys.FirstOrDefault(x => x == nameKey.Replace(keyName, keyCount));

                if (textParameters.TryGetValue(countKey, out string countValue))
                {
                    if (int.TryParse(countValue, out int c))
                    {
                        benefit.Count = c;
                    }
                }

                list.Add(benefit);
            }

            group.Params = list;
            return group;
        }

        protected internal JsonOfferDocumentsModel GetDocuments(OfferModel offer, DefinitionCombinationModel definition)
        {
            var files = this.ApiService.GetAttachments(offer);

            if (files.Length == 0)
            {
                return null;
            }

            var acceptance = this.GetDocumentsAcceptance(offer, files, definition);
            var uploads = this.GetUploads(offer, files, definition);
            var other = this.GetOther(offer, files, definition);

            if (acceptance == null && uploads == null && other == null)
            {
                return null;
            }

            var model = new JsonOfferDocumentsModel();
            model.Acceptance = acceptance;
            model.Uploads = uploads;
            model.Other = other;
            return model;
        }

        protected internal JsonDocumentsAcceptanceModel GetDocumentsAcceptance(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            var accept = this.GetAcceptanceDocumentsAccept(offer, files, definition);
            var sign = this.GetAcceptanceDocumentsSign(offer, files, definition);

            if (accept == null && sign == null)
            {
                return null;
            }

            var model = new JsonDocumentsAcceptanceModel();
            model.Title = definition.OfferCommoditiesTitle.Text;
            model.Text = Utils.GetReplacedTextTokens(definition.OfferCommoditiesText.Text, offer.TextParameters);
            model.Accept = accept;
            model.Sign = sign;
            return model;
        }

        protected internal JsonDocumentsAcceptModel GetAcceptanceDocumentsAccept(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            var selectedFiles = files.Where(x => x.Group == "COMMODITY" && x.IsPrinted == true && x.IsSignReq == false).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new JsonDocumentsAcceptModel();
            model.Title = definition.OfferCommoditiesAcceptTitle.Text;
            model.SubTitle = Utils.GetReplacedTextTokens(definition.OfferCommoditiesAcceptText.Text, offer.TextParameters);

            var list = new List<JsonAcceptFileModel>();

            for (int i = 0; i < selectedFiles.Length; i++)
            {
                var selectedFile = selectedFiles[i];
                var file = new JsonAcceptFileModel();
                file.Key = selectedFile.UniqueKey;
                file.Group = selectedFile.GroupGuid;
                file.IdAttach = selectedFile.IdAttach;
                file.Label = selectedFile.FileName;
                file.MimeType = selectedFile.MimeType;
                file.Mandatory = selectedFile.IsGroupOblig;
                file.Prefix = this.GetFileLabelPrefix(selectedFile);

                list.Add(file);

                if (selectedFile.IsObligatory)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }
            }

            model.Files = list;
            return model;
        }

        protected internal JsonDocumentsAcceptModel GetAcceptanceDocumentsSign(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            var selectedFiles = files.Where(x => x.Group == "COMMODITY" && x.IsPrinted == true && x.IsSignReq == true).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new JsonDocumentsAcceptModel();
            model.Title = definition.OfferCommoditiesSignTitle.Text;
            model.SubTitle = Utils.GetReplacedTextTokens(definition.OfferCommoditiesSignText.Text, offer.TextParameters);
            model.Note = null; //TODO: create text
            var list = new List<JsonAcceptFileModel>();

            for (int i = 0; i < selectedFiles.Length; i++)
            {
                var selectedFile = selectedFiles[i];
                var file = new JsonAcceptFileModel();
                file.Key = selectedFile.UniqueKey;
                file.Group = selectedFile.GroupGuid;
                file.IdAttach = selectedFile.IdAttach;
                file.Label = selectedFile.FileName;
                file.MimeType = selectedFile.MimeType;
                file.Mandatory = selectedFile.IsGroupOblig;
                file.Prefix = this.GetFileLabelPrefix(selectedFile);
                list.Add(file);

                if (selectedFile.IsObligatory)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }
            }

            model.Files = list;
            return model;
        }

        protected internal JsonDocumentsUploadsModel GetUploads(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            var fileTemplates = files.Where(x => x.IsPrinted == false).ToArray();

            if (fileTemplates.Length == 0)
            {
                return null;
            }

            var model = new JsonDocumentsUploadsModel();
            var list = new List<JsonUploadTemplateModel>();

            for (int i = 0; i < fileTemplates.Length; i++)
            {
                var template = fileTemplates[i];
                var file = new JsonUploadTemplateModel();
                file.GroupId = template.UniqueKey;
                file.IdAttach = template.IdAttach;
                file.Title = template.FileName;
                file.Info = this.GetTemplateHelp(template.IdAttach, offer.TextParameters);
                file.Mandatory = template.IsObligatory;

                if (template.IsObligatory)
                {
                    model.MandatoryGroups.Add(template.GroupGuid);
                }

                list.Add(file);
            }

            // logic moved to OfferParserService
            //var customFile = new JsonUploadTemplateModel();
            //customFile.GroupId = Utils.GetUniqueKeyForCustomUpload(offer);
            //customFile.Title = definition.OfferUploadsExtraText.Text;
            //customFile.Info = definition.OfferUploadsExtraHelp.Text;
            //customFile.Mandatory = false;
            //list.Add(customFile);

            model.Title = definition.OfferUploadsTitle.Text;
            model.Note = definition.OfferUploadsNote.Text;
            model.Types = list;
            return model;
        }

        protected internal JsonDocumentsOthersModel GetOther(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            var products = this.GetOtherProducts(offer, files, definition);
            var services = this.GetAdditionalServices(offer, files, definition);

            if (products == null && services == null)
            {
                return null;
            }

            return new JsonDocumentsOthersModel(products, services);
        }

        protected internal JsonDocumentsAdditionalServicesModel GetAdditionalServices(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            var selectedFiles = files.Where(x => x.Group == "DSL" && x.IsPrinted == true && x.IsSignReq == false).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new JsonDocumentsAdditionalServicesModel();
            var list = new List<JsonAcceptFileModel>();

            for (int i = 0; i < selectedFiles.Length; i++)
            {
                var selectedFile = selectedFiles[i];
                var file = new JsonAcceptFileModel();
                file.Key = selectedFile.UniqueKey;
                file.Group = selectedFile.GroupGuid;
                file.IdAttach = selectedFile.IdAttach;
                file.Label = selectedFile.FileName;
                file.Prefix = this.GetFileLabelPrefix(selectedFile);
                file.MimeType = selectedFile.MimeType;
                file.Mandatory = selectedFile.IsObligatory;

                if (selectedFile.IsObligatory || selectedFile.IsGroupOblig)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }

                list.Add(file);
            }

            model.Title = definition.OfferAdditionalServicesTitle.Text;
            model.Text = definition.OfferAdditionalServicesText.Text;
            model.Files = list;
            return model;
        }

        protected internal JsonDocumentsOtherProductsModel GetOtherProducts(OfferModel offer, OfferAttachmentModel[] files, DefinitionCombinationModel definition)
        {
            var selectedFiles = files.Where(x => x.Group == "NONCOMMODITY" && x.IsPrinted == true && x.IsSignReq == false && x.IsObligatory == false).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new JsonDocumentsOtherProductsModel();
            var list = new List<JsonAcceptFileModel>();

            for (int i = 0; i < selectedFiles.Length; i++)
            {
                var selectedFile = selectedFiles[i];
                var file = new JsonAcceptFileModel();
                file.Key = selectedFile.UniqueKey;
                file.Group = selectedFile.GroupGuid;
                file.IdAttach = selectedFile.IdAttach;
                file.Label = selectedFile.FileName;
                file.Prefix = this.GetFileLabelPrefix(selectedFile);
                file.MimeType = selectedFile.MimeType;
                file.Mandatory = selectedFile.IsObligatory;

                if (selectedFile.IsObligatory || selectedFile.IsGroupOblig)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }

                list.Add(file);
            }

            var parameters = new List<JsonParamModel>();
            var salesArguments = new List<JsonArgumentModel>();

            foreach (var item in offer.TextParameters.Where(x => x.Key.StartsWith("NONCOMMODITY_OFFER_SUMMARY_ATRIB_NAME")))
            {
                var key = item.Value;
                var value = this.GetEnumPairValue(item.Key, offer.TextParameters);
                parameters.Add(new JsonParamModel(key, value));
            }

            foreach (var item in offer.TextParameters.Where(x => x.Key.StartsWith("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_VALUE")))
            {
                salesArguments.Add(new JsonArgumentModel(item.Value));
            }

            model.Title = definition.OfferOtherProductsTitle.Text;
            model.Note = definition.OfferOtherProductsNote.Text;
            model.SalesArguments = salesArguments;
            model.Params = parameters;
            model.SubTitle = definition.OfferOtherProductsSummaryTitle.Text;
            model.SubTitle2 = definition.OfferOtherProductsDocsTitle.Text;
            model.Text = definition.OfferOtherProductsDocsText.Text;
            model.Files = list;
            return model;
        }

        protected internal JsonAcceptanceDialogViewModel GetAcceptance(OfferModel offer, DefinitionCombinationModel definition)
        {
            var acceptGuids = offer.TextParameters.Where(x => x.Key.Contains("_ACCEPT_LABEL_GUID")).ToArray();

            var list = new List<JsonAcceptanceDialogParamViewModel>();

            for (int i = 0; i < acceptGuids.Length; i++)
            {
                var group = acceptGuids[i].Value;
                var labelKey = acceptGuids[i].Key.Replace("_GUID", "");
                var title = offer.TextParameters.FirstOrDefault(x => x.Key == labelKey).Value;
                list.Add(new JsonAcceptanceDialogParamViewModel(group, title));
            }

            var model = new JsonAcceptanceDialogViewModel();
            model.Parameters = list;
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

        protected internal string GetTemplateHelp(string idAttach, IDictionary<string, string> textParameters)
        {
            var key = $"USER_ATTACH_{idAttach}_HELP";

            if (textParameters.ContainsKey(key))
            {
                return textParameters[key];
            }

            return null;
        }

        protected internal string GetFileLabelPrefix(OfferAttachmentModel model)
        {
            return this.TextService.FindByKey("DOCUMENT_PREFIX_" + model.ConsentType);
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
