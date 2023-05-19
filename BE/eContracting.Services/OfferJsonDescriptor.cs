using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Models.JsonDescriptor;
using Glass.Mapper.Sc;
using JSNLog.Infrastructure;
using Sitecore.ApplicationCenter.Applications;
using Sitecore.Pipelines.RenderField;
using Sitecore.Publishing.Explanations;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.StringExtensions;
using Sitecore.Web;
using static eContracting.Models.JsonOfferPersonalDataModel;
using static eContracting.Models.JsonOfferProductModel;

namespace eContracting.Services
{
    public class OfferJsonDescriptor : IOfferJsonDescriptor
    {
        protected readonly ILogger Logger;
        protected readonly ITextService TextService;
        protected readonly ISitecoreService SitecoreService;
        protected readonly IOfferService OfferService;
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferJsonDescriptor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="textService">The text service.</param>
        /// <param name="sitecoreService">The context.</param>
        /// <param name="offerService">The API service.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// textService
        /// or
        /// context
        /// or
        /// offerService
        /// or
        /// settingsReaderService
        /// </exception>
        public OfferJsonDescriptor(
            ILogger logger,
            ITextService textService,
            ISitecoreService sitecoreService,
            IOfferService offerService,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.TextService = textService ?? throw new ArgumentNullException(nameof(textService));
            this.SitecoreService = sitecoreService ?? throw new ArgumentNullException(nameof(sitecoreService));
            this.OfferService = offerService ?? throw new ArgumentNullException(nameof(offerService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
        }

        /// <inheritdoc/>
        public JsonOfferSummaryModel GetSummary(OffersModel offer, UserCacheDataModel user)
        {
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var model = new JsonOfferSummaryModel();
            model.User = this.GetPersonalData(offer);
            model.DistributorChange = this.GetDistributorChange(offer);
            model.Product = this.GetProductData(offer);
            model.Gifts = this.GetGifts(offer.TextParameters, definition);
            model.SalesArguments = this.GetAllSalesArguments(offer.TextParameters, model.Product != null);
            return model;
        }

<<<<<<< HEAD
        public ContainerModel GetSummary2(OffersModel offer, UserCacheDataModel user)
        {
            var container = new ContainerModel();
            var contractualData = this.GetPersonalData2(offer);

            if (contractualData != null)
            {
                container.Data.Add(contractualData);
            }
            var benefitData = this.GetAllSalesArguments2(offer.TextParameters, false); // ToDo: when GetProductData2 developed do ekvivalent of this condition model.Product != null
            foreach (var benefit in benefitData)
            {
                container.Data.Add(benefit);
            }

            return container;
        }

        /// <inheritdoc/>
        public JsonOfferNotAcceptedModel GetNew(OffersModel offer, UserCacheDataModel user)
        {
=======
        /// <inheritdoc/>
        public JsonOfferNotAcceptedModel GetNew(OffersModel offer, UserCacheDataModel user)
        {
>>>>>>> feature/pinn_r1_summary_FE
            var attachments = this.OfferService.GetAttachments(offer, user);
            return this.GetNew(offer, attachments);
        }

        /// <inheritdoc/>
        public JsonOfferAcceptedModel GetAccepted(OffersModel offer, UserCacheDataModel user)
        {
            var attachments = this.OfferService.GetAttachments(offer, user);
            return this.GetAccepted(offer, attachments);
        }


        /// <summary>
        /// Gets personal container.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        protected internal JsonOfferPersonalDataModel GetPersonalData(OffersModel offer)
        {
            if (!this.IsSectionChecked(offer.TextParameters, "PERSON"))
            {
                this.Logger.Warn(offer.Guid, "Missing PERSON container in text parameters (<PERSON></PERSON>)");
                return null;
            }

            var model = new JsonOfferPersonalDataModel();
            model.Title = this.TextService.FindByKey("CONTRACTUAL_DATA");

            var infos = new List<JsonOfferPersonalDataModel.PersonalDataInfo>();

            if (offer.TextParameters.HasValue("PERSON_CUSTNAME"))
            {
                var info = new JsonOfferPersonalDataModel.PersonalDataInfo();
                info.Title = this.TextService.FindByKey("PERSONAL_INFORMATION");
                var lines = new List<string>();
                lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTNAME"));
                lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTEMAIL"));
                lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTTEL1"));
                info.Lines = lines;
                infos.Add(info);
            }

            if (offer.TextParameters.HasValue("PERSON_CUSTADDRESS"))
            {
                var info = new JsonOfferPersonalDataModel.PersonalDataInfo();
                info.Title = this.TextService.FindByKey("PERMANENT_ADDRESS");
                var lines = new List<string>();
                var streetName = offer.TextParameters.HasValue("PERSON_CUSTSTREET") ? offer.TextParameters.GetValueOrDefault("PERSON_CUSTSTREET") : offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY");
                lines.Add(streetName + " " + offer.TextParameters.GetValueOrDefault("PERSON_CUSTSTREET_NUMBER"));
                lines.Add(offer.TextParameters["PERSON_CUSTCITY"]);

                if (offer.TextParameters.HasValue("PERSON_CUSTCITY_PART") && (offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY_PART") != offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY")))
                {
                    lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY_PART"));
                }

                lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTPOSTAL_CODE"));
                info.Lines = lines;
                infos.Add(info);
            }

            if (offer.TextParameters.HasValue("PERSON_CORADDRESS"))
            {
                var info = new JsonOfferPersonalDataModel.PersonalDataInfo();
                info.Title = this.TextService.FindByKey("MAILING_ADDRESS");
                var lines = new List<string>();
                var streetName = offer.TextParameters.HasValue("PERSON_CORSTREET") ? offer.TextParameters.GetValueOrDefault("PERSON_CORSTREET") : offer.TextParameters.GetValueOrDefault("PERSON_CORCITY");
                lines.Add(streetName + " " + offer.TextParameters.GetValueOrDefault("PERSON_CORSTREET_NUMBER"));
                lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CORCITY"));

                if (offer.TextParameters.HasValue("PERSON_CORCITY_PART") && (offer.TextParameters.GetValueOrDefault("PERSON_CORCITY_PART") != offer.TextParameters.GetValueOrDefault("PERSON_CORCITY")))
                {
                    lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CORCITY_PART"));
                }

                lines.Add(offer.TextParameters.GetValueOrDefault("PERSON_CORPOSTAL_CODE"));
                info.Lines = lines;
                infos.Add(info);
            }

            CommunicationMethodInfo communicationMethod = null;

            if (offer.TextParameters.HasValue("PERSON_CUSTSZK"))
            {
                communicationMethod = new CommunicationMethodInfo();
                communicationMethod.Title = this.TextService.FindByKey("AGREED_WAY_OF_COMMUNICATION");
                communicationMethod.Value = offer.TextParameters.GetValueOrDefault("PERSON_CUSTSZK");
                model.AgreedCommunicationMethod = communicationMethod;
            }

            if (infos.Count == 0 && communicationMethod == null)
            {
                this.Logger.Warn(offer.Guid, "No PERSON container gathered from text parameters");
                return null;
            }

            model.Infos = infos;
            model.AgreedCommunicationMethod = communicationMethod;

            return model;
        }

        protected internal IDataModel GetPersonalData2(OffersModel offer)
        {
            if (!this.IsSectionChecked(offer.TextParameters, "PERSON"))
            {
                this.Logger.Warn(offer.Guid, "Missing PERSON container in text parameters (<PERSON></PERSON>)");
                return null;
            }

            var model = new ContractualDataModel();
            var header = new ContractualDataHeaderModel();
            header.Title = this.TextService.FindByKey("CONTRACTUAL_DATA");

            var body = new ContractualDataBodyModel();
            var personalData = this.GetPersonalInfo(offer);

            if (personalData != null)
            {
                body.PersonalData = new[] { personalData };
            }

            var addresses = new List<TitleAndValuesModel>();
            addresses.AddIfNotNull(this.GetPersonalCustomAddress(offer));
            addresses.AddIfNotNull(this.GetPersonalCoreAddress(offer));

            if (addresses.Count > 0)
            {
                body.Addresses = addresses;
            }

            var contact = this.GetPersonalCommunicationMethod(offer);

            if (contact != null)
            {
                body.Contacts = new[] { contact };
            }

            if (body.PersonalData == null && body.Addresses == null && body.Contacts == null)
            {
                this.Logger.Warn(offer.Guid, "No PERSON container gathered from text parameters");
                return null;
            }

            return model;
        }

        protected internal TitleAndValuesModel GetPersonalInfo(OffersModel offer)
        {
            if (!offer.TextParameters.HasValue("PERSON_CUSTNAME"))
            {
                return null;
            }

            var data = new TitleAndValuesModel();
            data.Title = this.TextService.FindByKey("PERSONAL_INFORMATION");
            var values = new List<string>();
            values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTNAME"));
            values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTEMAIL"));
            values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTTEL1"));
            data.Values = values;
            return data;
        }

        protected internal TitleAndValuesModel GetPersonalCustomAddress(OffersModel offer)
        {
            if (!offer.TextParameters.HasValue("PERSON_CUSTADDRESS"))
            {
                return null;
            }

            var data = new TitleAndValuesModel();
            data.Title = this.TextService.FindByKey("PERMANENT_ADDRESS");
            var values = new List<string>();
            var streetName = offer.TextParameters.HasValue("PERSON_CUSTSTREET") ? offer.TextParameters.GetValueOrDefault("PERSON_CUSTSTREET") : offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY");
            values.Add(streetName + " " + offer.TextParameters.GetValueOrDefault("PERSON_CUSTSTREET_NUMBER"));
            values.Add(offer.TextParameters["PERSON_CUSTCITY"]);

            if (offer.TextParameters.HasValue("PERSON_CUSTCITY_PART") && (offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY_PART") != offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY")))
            {
                values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTCITY_PART"));
            }

            values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CUSTPOSTAL_CODE"));
            data.Values = values;
            return data;
        }

        protected internal TitleAndValuesModel GetPersonalCoreAddress(OffersModel offer)
        {
            if (!offer.TextParameters.HasValue("PERSON_CORADDRESS"))
            {
                return null;
            }
            var data = new TitleAndValuesModel();
            data.Title = this.TextService.FindByKey("MAILING_ADDRESS");
            var values = new List<string>();
            var streetName = offer.TextParameters.HasValue("PERSON_CORSTREET") ? offer.TextParameters.GetValueOrDefault("PERSON_CORSTREET") : offer.TextParameters.GetValueOrDefault("PERSON_CORCITY");
            values.Add(streetName + " " + offer.TextParameters.GetValueOrDefault("PERSON_CORSTREET_NUMBER"));
            values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CORCITY"));

            if (offer.TextParameters.HasValue("PERSON_CORCITY_PART") && (offer.TextParameters.GetValueOrDefault("PERSON_CORCITY_PART") != offer.TextParameters.GetValueOrDefault("PERSON_CORCITY")))
            {
                values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CORCITY_PART"));
            }

            values.Add(offer.TextParameters.GetValueOrDefault("PERSON_CORPOSTAL_CODE"));
            data.Values = values;
            return data;
        }

        protected internal TitleAndValuesModel GetPersonalCommunicationMethod(OffersModel offer)
        {
            if (!offer.TextParameters.HasValue("PERSON_CUSTSZK"))
            {
                return null;
            }

            var data = new TitleAndValuesModel();
            data.Title = this.TextService.FindByKey("AGREED_WAY_OF_COMMUNICATION");
            data.Values = new[] { offer.TextParameters.GetValueOrDefault("PERSON_CUSTSZK") };
            return data;
        }

        /// <summary>
        /// Gets product container with all prices.
        /// </summary>
        /// <param name="offer"></param>
<<<<<<< HEAD
        /// <returns>Null when <see cref="OfferModel.ShowPrices"/> == false OR there are no container to display. Otherwise container object.</returns>
=======
        /// <returns>Null when <see cref="OfferModel.ShowPrices"/> == false OR there are no data to display. Otherwise data object.</returns>
>>>>>>> feature/pinn_r1_summary_FE
        protected internal JsonOfferProductModel GetProductData(OffersModel offer)
        {
            if (!offer.ShowPrices)
            {
                return null;
            }

            var model = new JsonOfferProductModel();
            model.Header = this.GetHeaderData(model, offer);
            model.ProductInfos = this.GetProductInfos(offer);
            model.Type = this.GetProductType(offer);
            //model.Header.Note = this.GetProductNote(model.Type);
            model.MiddleTexts = this.GetMiddleTexts(offer);

            if (model.MiddleTexts.Count() > 0 && offer.TextParameters.HasValue("SA06_MIDDLE_TEXT"))
            {
                model.MiddleTextsHelp = offer.TextParameters["SA06_MIDDLE_TEXT"];
            }

            model.Benefits = this.GetBenefits(offer);

            if (!model.ProductInfos.Any() && !model.MiddleTexts.Any() && !model.Benefits.Any())
            {
                return null;
            }

            //if (!string.IsNullOrEmpty(model.Type))
            //{
            //    var productsRoot = this.SitecoreService.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS);

            //    if (productsRoot != null)
            //    {
                    
            //    }
            //}

            return model;
        }

        /// <summary>
        /// Gets container when user / offer changes a distributor (dismissal previous dealer).
        /// </summary>
        /// <param name="offer"></param>
<<<<<<< HEAD
        /// <returns>Null when text parameter <c>PERSON_COMPETITOR_NAME</c> is missing OR <see cref="OfferModel.Process"/> not equals to <c>01</c>. Otherwise container object.</returns>
=======
        /// <returns>Null when text parameter <c>PERSON_COMPETITOR_NAME</c> is missing OR <see cref="OfferModel.Process"/> not equals to <c>01</c>. Otherwise data object.</returns>
>>>>>>> feature/pinn_r1_summary_FE
        protected internal JsonOfferDistributorChangeModel GetDistributorChange(OffersModel offer)
        {
            if (!offer.TextParameters.HasValue("PERSON_COMPETITOR_NAME"))
            {
                this.Logger.Debug(offer.Guid, "Value PERSON_COMPETITOR_NAME doesn't exist");
                return null;
            }

            if (offer.Process != "01")
            {
                this.Logger.Debug(offer.Guid, "Value PERSON_COMPETITOR_NAME exists, but is hidden because BUS_PROCESS != 01");
                return null;
            }

            var siteSettings = this.SettingsReaderService.GetSiteSettings();
            var summaryPageId = siteSettings.Summary.TargetId;
            var summaryPage = this.SitecoreService.GetItem<IPageSummaryOfferModel>(summaryPageId);
            var model = new JsonOfferDistributorChangeModel();
            model.Title = summaryPage.DistributorChange_Title;
            model.Name = offer.TextParameters["PERSON_COMPETITOR_NAME"];
            model.Description = summaryPage.DistributorChange_Text;
            return model;
        }

        /// <summary>
        /// Gets container for accepted offer.
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="attachments"></param>
        /// <returns>Always returns the object.</returns>
        protected internal JsonOfferAcceptedModel GetAccepted(OffersModel offer, OfferAttachmentModel[] attachments)
        {
            var groups = new List<JsonFilesSectionModel>();
            var version = offer.Version;
            var documents = offer.Documents;
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var textParameters = offer.TextParameters;
            var fileGroups = attachments.GroupBy(x => x.Group);

            foreach (IGrouping<string, OfferAttachmentModel> fileGroup in fileGroups)
            {
                var g = this.GetSection(fileGroup.Key, fileGroup, definition, offer);
                groups.AddRange(g);
            }

            return new JsonOfferAcceptedModel(groups.OrderBy(x => x.Position));
        }

        /// <summary>
        /// Gets container for new non-accepted offer.
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="attachments"></param>
        /// <returns>Always returns the object.</returns>
        protected internal JsonOfferNotAcceptedModel GetNew(OffersModel offer, OfferAttachmentModel[] attachments)
        {
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var productInfos = this.SettingsReaderService.GetAllProductInfos();
            var model = new JsonOfferNotAcceptedModel();

            if (offer.Version > 1)
            {
                if (definition.OfferPerexShow)
                {
                    model.Perex = this.GetPerex(offer.TextParameters, definition);
                }

                // disabled by https://jira.innogy.cz/browse/TRUNW-475
                //if (definition.OfferGiftsShow)
                //{
                //    model.Gifts = this.GetGifts(offer.TextParameters, definition);
                //}

                // for version 3 container are picked up by GetSummary method
                if (offer.Version == 2)
                {
                    model.SalesArguments = this.GetCommoditySalesArguments(offer.TextParameters, definition);
                }
            }

            model.Documents = this.GetDocuments(offer, definition, productInfos, attachments);
            model.AcceptanceDialog = this.GetAcceptance(offer, definition);
            return model;
        }

        protected internal IEnumerable<JsonFilesSectionModel> GetSection(string groupName, IEnumerable<OfferAttachmentModel> attachments, IDefinitionCombinationModel definition, OffersModel offer)
        {
            var list = new List<JsonFilesSectionModel>();

            if (groupName == "COMMODITY")
            {
                var acceptFiles = attachments.Where(x => x.Group == "COMMODITY" && !x.IsSignReq);
                var signFiles = attachments.Where(x => x.Group == "COMMODITY" && x.IsSignReq);

                if (acceptFiles.Any())
                {
                    var title = definition.OfferDocumentsForAcceptanceTitle?.Text.Trim(); //definition.AcceptedDocumentsTitle;
                    list.Add(new JsonFilesSectionModel(acceptFiles.Select(x => new JsonFileModel(x)), title, 0));
                }

                if (signFiles.Any())
                {
                    var title = definition.OfferDocumentsForSignTitle?.Text.Trim(); //definition.SignedDocumentsTitle;
                    list.Add(new JsonFilesSectionModel(signFiles.Select(x => new JsonFileModel(x)), title, 1));
                }
            }
            else if (groupName == "DSL")
            {
                var files = attachments.Where(x => x.Group == "DSL");
                var title = definition.OfferAdditionalServicesTitle?.Text.Trim(); //definition.AdditionalServicesTitle;
                list.Add(new JsonFilesSectionModel(files.Select(x => new JsonFileModel(x)), title, 2));
            }
            else
            {
                var files = attachments.Where(x => x.Group != "COMMODITY" && x.Group != "DSL" && x.IsPrinted);
                var title = definition.OfferOtherProductsDocsTitle?.Text.Trim(); //definition.AdditionalServicesTitle;
                list.Add(new JsonFilesSectionModel(files.Select(x => new JsonFileModel(x)), title, 3));
            }

            return list;
        }

        protected internal JsonOfferPerexModel GetPerex(IDictionary<string, string> textParameters, IDefinitionCombinationModel definition)
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
                model.Title = definition.OfferPerexTitle?.Text.Trim();
                model.Parameters = parameters.ToArray();
                return model;
            }

            return null;
        }

        /// <summary>
        /// Gets all sales arguments.
        /// </summary>
        /// <param name="textParameters">Text parameters.</param>
        /// <returns>All sales arguments or empty array.</returns>
        protected internal IEnumerable<JsonSalesArgumentsModel> GetAllSalesArguments(IDictionary<string, string> textParameters, bool excludeCommodity)
        {
            var list = new List<JsonSalesArgumentsModel>();

            var addServices = this.GetSalesArgumentsWithPrefix(textParameters, "ADD_SERVICES");

            if (addServices.Any())
            {
                list.AddRange(addServices);
            }

            var nonCommodities = this.GetSalesArgumentsWithPrefix(textParameters, "NONCOMMODITY");

            if (nonCommodities.Any())
            {
                list.AddRange(nonCommodities);
            }

            if (!excludeCommodity)
            {
                var commodities = this.GetSalesArgumentsWithPrefix(textParameters, "COMMODITY");

                if (commodities.Any())
                {
                    list.AddRange(commodities);
                }
            }

            return list;
        }

        protected internal IEnumerable<IDataModel> GetAllSalesArguments2(IDictionary<string, string> textParameters, bool excludeCommodity)
        {
            var list = new List<IDataModel>();

            var addServices = this.GetSalesArgumentsWithPrefix2(textParameters, "ADD_SERVICES");

            if (addServices.Any())
            {
                list.AddRange(addServices);
            }

            var nonCommodities = this.GetSalesArgumentsWithPrefix2(textParameters, "NONCOMMODITY");

            if (nonCommodities.Any())
            {
                list.AddRange(nonCommodities);
            }

            if (!excludeCommodity)
            {
                var commodities = this.GetSalesArgumentsWithPrefix2(textParameters, "COMMODITY");

                if (commodities.Any())
                {
                    list.AddRange(commodities);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets sales arguments from <c>_SALES_ARGUMENTS_</c> text parameters.
        /// </summary>
        /// <param name="textParameters">Text parameters.</param>
        /// <param name="definition">The matrix definition.</param>
        /// <returns>All sales arguments or null.</returns>
        protected internal JsonSalesArgumentsModel GetCommoditySalesArguments(IDictionary<string, string> textParameters, IDefinitionCombinationModel definition)
        {
            var values = textParameters.Where(x => x.Key.StartsWith("COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE")).Select(x => x.Value).ToArray();

            if (values.Length == 0)
            {
                return null;
            }

            var model = new JsonSalesArgumentsModel();
            model.Title = definition.OfferBenefitsTitle?.Text.Trim();
            model.Arguments = values.Select(x => new JsonArgumentModel(x)).ToArray();

            var commodityProductTypeAttribute = textParameters.FirstOrDefault(x => x.Key.StartsWith("COMMODITY_PRODUCT"));
            if (!commodityProductTypeAttribute.Equals(default(KeyValuePair<string, string>)))
            {
                string commodityProductTypeAttributeValue = commodityProductTypeAttribute.Value;
                if (!string.IsNullOrEmpty(commodityProductTypeAttributeValue))
                {
                    if (commodityProductTypeAttributeValue.StartsWith("G_"))
                        model.CommodityProductType = "G";
                    else if (commodityProductTypeAttributeValue.StartsWith("E_") || commodityProductTypeAttributeValue.StartsWith("EE_") || commodityProductTypeAttributeValue.EndsWith("_EE") || commodityProductTypeAttributeValue.EndsWith("_E"))
                        model.CommodityProductType = "E";
                }
            }

            return model;
        }

        protected internal JsonSalesArgumentsModel GetAdditionalServiceSalesArguments(IDictionary<string, string> textParameters, IDefinitionCombinationModel definition)
        {
            var values = textParameters.Where(x => x.Key.StartsWith("ADD_SERVICES_SALES_ARGUMENTS_ATRIB_VALUE")).Select(x => x.Value).ToArray();

            if (values.Length == 0)
            {
                return null;
            }

            var model = new JsonSalesArgumentsModel();
            model.Title = definition.OfferBenefitsTitle.Text;
            model.Arguments = values.Select(x => new JsonArgumentModel(x)).ToArray();
            return model;
        }

        /// <summary>
        /// Gets gifts from <c>BENEFITS</c> parameters.
        /// </summary>
        /// <param name="textParameters">Text parameters.</param>
        /// <param name="definition">The matrix combination.</param>
        /// <returns>Gets group of gifts or null if not <c>BENEFITS</c> found.</returns>
        protected internal JsonAllBenefitsModel GetGifts(IDictionary<string, string> textParameters, IDefinitionCombinationModel definition)
        {
            if (!this.IsSectionChecked(textParameters, "BENEFITS"))
            {
                return null;
            }

            var keys = new[] { "BENEFITS_NOW", "BENEFITS_NEXT_SIGN", "BENEFITS_NEXT_TZD" };
            var groups = new List<JsonBenefitsGroupModel>();

            for (int i = 0; i < keys.Length; i++)
            {
                var k = keys[i];
                var g = this.GetBenefitGroup(k, textParameters);

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
            model.Title = definition.OfferGiftsTitle?.Text.Trim();

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

                if (!string.IsNullOrEmpty(iconKey))
                {
                    if (textParameters.TryGetValue(iconKey, out string iconValue))
                    {
                        benefit.Icon = iconValue;
                    }
                }

                var countKey = keys.FirstOrDefault(x => x == nameKey.Replace(keyName, keyCount));

                if (!string.IsNullOrEmpty(countKey))
                {
                    if (textParameters.TryGetValue(countKey, out string countValue))
                    {
                        if (int.TryParse(countValue, out int c))
                        {
                            benefit.Count = c;
                        }
                    }
                }

                list.Add(benefit);
            }

            group.Params = list;
            return group;
        }

        protected internal JsonOfferDocumentsModel GetDocuments(OffersModel offer, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos, OfferAttachmentModel[] files)
        {
            if (files.Length == 0)
            {
                return null;
            }

            var acceptance = this.GetDocumentsAcceptance(offer, files, definition, productInfos);
            var uploads = this.GetUploads(offer, files, definition);
            var other = this.GetOther(offer, files, definition, productInfos);

            if (acceptance == null && uploads == null && other == null)
            {
                return null;
            }

            var model = new JsonOfferDocumentsModel();
            model.Description = definition.OfferDocumentsDescription?.Text;
            model.Acceptance = acceptance;
            model.Uploads = uploads;
            model.Other = other;
            return model;
        }

        protected internal JsonDocumentsAcceptanceModel GetDocumentsAcceptance(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
        {
            var accept = this.GetAcceptanceDocumentsAccept(offer, files, definition, productInfos);
            var sign = this.GetAcceptanceDocumentsSign(offer, files, definition, productInfos);

            if (accept == null && sign == null)
            {
                return null;
            }

            var model = new JsonDocumentsAcceptanceModel();
            model.AcceptanceInfoBox = new JsonSectionAcceptanceBoxModel();
            model.AcceptanceInfoBox.Title = definition.OfferDocumentsForElectronicAcceptanceTitle?.Text.Trim();
            model.AcceptanceInfoBox.Text = Utils.GetReplacedTextTokens(definition.OfferDocumentsForElectronicAcceptanceText?.Text, offer.TextParameters);
            model.AcceptanceInfoBox.Note = Utils.GetReplacedTextTokens(definition.OfferDocumentsDescription?.Text, offer.TextParameters);
            model.Title = model.AcceptanceInfoBox.Title;
            model.Text = model.AcceptanceInfoBox.Text;
            model.Accept = accept;
            model.Sign = sign;

            return model;
        }

        protected internal JsonDocumentsAcceptModel GetAcceptanceDocumentsAccept(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
        {
            var selectedFiles = files.Where(x => x.Group == "COMMODITY" && x.IsPrinted == true && x.IsSignReq == false).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new JsonDocumentsAcceptModel();
            model.Title = definition.OfferDocumentsForAcceptanceTitle?.Text.Trim();
            model.SubTitle = Utils.GetReplacedTextTokens(definition.OfferDocumentsForAcceptanceText?.Text, offer.TextParameters);

            var list = new List<JsonAcceptFileModel>();

            for (int i = 0; i < selectedFiles.Length; i++)
            {
                var selectedFile = selectedFiles[i];
                var file = new JsonAcceptFileModel(selectedFile);
                file.Prefix = this.GetFileLabelPrefix(selectedFile);
                list.Add(file);

                if (file.Mandatory)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }
            }

            this.UpdateProductInfo(list, productInfos);

            model.Files = list;
            return model;
        }

        protected internal JsonDocumentsAcceptModel GetAcceptanceDocumentsSign(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
        {
            var selectedFiles = files.Where(x => x.Group == "COMMODITY" && x.IsPrinted == true && x.IsSignReq == true).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new JsonDocumentsAcceptModel();
            model.Title = definition.OfferDocumentsForSignTitle?.Text.Trim();
            model.SubTitle = Utils.GetReplacedTextTokens(definition.OfferDocumentsForSignText?.Text, offer.TextParameters);
            model.Note = null; //TODO: create text
            var list = new List<JsonAcceptFileModel>();

            for (int i = 0; i < selectedFiles.Length; i++)
            {
                var selectedFile = selectedFiles[i];
                var file = new JsonAcceptFileModel(selectedFile);
                file.Prefix = this.GetFileLabelPrefix(selectedFile);
                list.Add(file);

                if (file.Mandatory)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }
            }

            this.UpdateProductInfo(list, productInfos);

            model.Files = list;
            return model;
        }

        protected internal JsonDocumentsUploadsModel GetUploads(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition)
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
                var file = new JsonUploadTemplateModel(template);
                file.Info = this.GetTemplateHelp(template.IdAttach, offer.TextParameters);

                if (file.Mandatory)
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

            model.Title = Utils.GetReplacedTextTokens(definition.OfferUploadsTitle?.Text.Trim(), offer.TextParameters);
            model.Note = Utils.GetReplacedTextTokens(definition.OfferUploadsNote?.Text.Trim(), offer.TextParameters);
            model.Types = list;
            return model;
        }

        protected internal JsonDocumentsOthersModel GetOther(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
        {
            var products = this.GetOtherProducts(offer, files, definition, productInfos);
            var services = this.GetAdditionalServices(offer, files, definition, productInfos);

            if (products == null && services == null)
            {
                return null;
            }

            return new JsonDocumentsOthersModel(products, services);
        }

        protected internal JsonDocumentsAdditionalServicesModel GetAdditionalServices(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
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
                var file = new JsonAcceptFileModel(selectedFile);
                file.Prefix = this.GetFileLabelPrefix(selectedFile);

                if (file.Mandatory)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }

                list.Add(file);
            }

            this.UpdateProductInfo(list, productInfos);

            var parameters = new List<JsonParamModel>();
            var salesArguments = new List<JsonArgumentModel>();

            if (offer.Version < 3)
            {
                foreach (var item in offer.TextParameters.Where(x => x.Key.StartsWith("ADD_SERVICES_OFFER_SUMMARY_ATRIB_NAME")))
                {
                    var key = item.Value;
                    var value = this.GetEnumPairValue(item.Key, offer.TextParameters);
                    parameters.Add(new JsonParamModel(key, value));
                }

                foreach (var item in offer.TextParameters.Where(x => x.Key.StartsWith("ADD_SERVICES_SALES_ARGUMENTS_ATRIB_VALUE")))
                {
                    salesArguments.Add(new JsonArgumentModel(item.Value));
                }
            }

            if (definition.OfferAdditionalServicesDescription != null)
            {
                model.AcceptanceInfoBox = new JsonSectionAcceptanceBoxModel();
                model.AcceptanceInfoBox.Title = definition.OfferAdditionalServicesTitle?.Text.Trim();
                model.AcceptanceInfoBox.Text = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesSummaryText?.Text, offer.TextParameters);
                model.AcceptanceInfoBox.Note = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesDescription?.Text, offer.TextParameters);
            }

            model.Title = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesTitle?.Text.Trim(), offer.TextParameters);
            model.Note = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesNote?.Text.Trim(), offer.TextParameters);
            model.SalesArguments = salesArguments;
            model.Params = parameters;
            model.SubTitle = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesSummaryTitle?.Text.Trim(), offer.TextParameters);
            model.SubTitle2 = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesDocsTitle?.Text.Trim(), offer.TextParameters);
            model.Text = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesDocsText?.Text.Trim(), offer.TextParameters);
            model.Files = list;
            return model;
        }

        protected internal JsonDocumentsOtherProductsModel GetOtherProducts(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
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
                var file = new JsonAcceptFileModel(selectedFile);
                file.Prefix = this.GetFileLabelPrefix(selectedFile);

                if (file.Mandatory)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }

                list.Add(file);
            }

            this.UpdateProductInfo(list, productInfos);

            var parameters = new List<JsonParamModel>();
            var salesArguments = new List<JsonArgumentModel>();

            if (offer.Version < 3)
            {
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
            }

            model.AcceptanceInfoBox = new JsonSectionAcceptanceBoxModel();
            model.AcceptanceInfoBox.Title = definition.OfferOtherProductsTitle?.Text.Trim();
            model.AcceptanceInfoBox.Text = Utils.GetReplacedTextTokens(definition.OfferOtherProductsSummaryText?.Text, offer.TextParameters);
            model.AcceptanceInfoBox.Note = Utils.GetReplacedTextTokens(definition.OfferOtherProductsDescription?.Text, offer.TextParameters);
            model.Title = Utils.GetReplacedTextTokens(model.AcceptanceInfoBox.Title, offer.TextParameters);
            model.Description = Utils.GetReplacedTextTokens(model.AcceptanceInfoBox.Note, offer.TextParameters);
            model.Note = Utils.GetReplacedTextTokens(definition.OfferOtherProductsNote?.Text.Trim(), offer.TextParameters);
            model.SalesArguments = salesArguments;
            model.Params = parameters;
            model.SubTitle = Utils.GetReplacedTextTokens(definition.OfferOtherProductsSummaryTitle.Text, offer.TextParameters);
            model.SubTitle2 = Utils.GetReplacedTextTokens(definition.OfferOtherProductsDocsTitle?.Text.Trim(), offer.TextParameters);
            model.Text = Utils.GetReplacedTextTokens(definition.OfferOtherProductsDocsText?.Text.Trim(), offer.TextParameters);
            model.Files = list;

            return model;
        }

        protected internal JsonAcceptanceDialogViewModel GetAcceptance(OffersModel offer, IDefinitionCombinationModel definition)
        {
            var acceptGuids = offer.TextParameters.Where(x => x.Key.Contains("_ACCEPT_LABEL_GUID")).ToArray();

            var list = new List<JsonAcceptanceDialogParamViewModel>();

            for (int i = 0; i < acceptGuids.Length; i++)
            {
                var group = acceptGuids[i].Value;
                var labelKey = acceptGuids[i].Key.Replace("_GUID", "");
                var title = offer.TextParameters.FirstOrDefault(x => x.Key == labelKey).Value;

                if (!string.IsNullOrEmpty(title))
                {
                    list.Add(new JsonAcceptanceDialogParamViewModel(group, title));
                }
            }

            var model = new JsonAcceptanceDialogViewModel();
            model.Parameters = list;
            return model;
        }

        protected internal HeaderData GetHeaderData(JsonOfferProductModel model, OffersModel offer)
        {
            var header = new HeaderData();
            header.Name = offer.TextParameters.GetValueOrDefault("CALC_PROD_DESC");

            if (this.CanDisplayPrice(offer.TextParameters, "CALC_TOTAL_SAVE"))
            {
                header.Price1Description = offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE_DESCRIPTION");
                header.Price1Value = offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE") + " " + offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE_DISPLAY_UNIT");

                if (offer.TextParameters.HasValue("CALC_TOTAL_SAVE_TOOLTIP"))
                {
                    header.Price1Note = offer.TextParameters["CALC_TOTAL_SAVE_TOOLTIP"];
                }
            }
            else
            {
                this.Logger.Warn(offer.Guid, "Text parameter 'CALC_TOTAL_SAVE_SHOW' is 'X' but 'CALC_TOTAL_SAVE' has no price");
            }

            if (string.IsNullOrEmpty(header.Price1Value)/* && this.IsSectionChecked(offer.TextParameters, "CALC_DEP_VALUE_SHOW")*/)
            {
                if (this.CanDisplayPrice(offer.TextParameters, "CALC_DEP_VALUE"))
                {
                    header.Price1Description = offer.TextParameters.GetValueOrDefault("CALC_DEP_VALUE_DESCRIPTION");
                    header.Price1Value = offer.TextParameters.GetValueOrDefault("CALC_DEP_VALUE") + " " + offer.TextParameters.GetValueOrDefault("CALC_DEP_VALUE_DISPLAY_UNIT");

                    if (offer.TextParameters.HasValue("CALC_DEP_VALUE_TOOLTIP"))
                    {
                        header.Price1Note = offer.TextParameters["CALC_DEP_VALUE_TOOLTIP"];
                    }
                }
                else
                {
                    this.Logger.Warn(offer.Guid, "Text parameter 'CALC_TOTAL_SAVE' and 'CALC_DEP_VALUE' is missing / empty.");
                }
            }

            if (this.CanDisplayPrice(offer.TextParameters, "CALC_FIN_REW"))
            {
                header.Price2Description = offer.TextParameters.GetValueOrDefault("CALC_FIN_REW_DESCRIPTION");
                header.Price2Value = offer.TextParameters.GetValueOrDefault("CALC_FIN_REW") + " " + offer.TextParameters.GetValueOrDefault("CALC_FIN_REW_DISPLAY_UNIT");

                if (offer.TextParameters.HasValue("CALC_FIN_REW_TOOLTIP"))
                {
                    header.Price2Note = offer.TextParameters["CALC_FIN_REW_TOOLTIP"];
                }
            }
            else
            {
                this.Logger.Warn(offer.Guid, "Text parameter 'CALC_FIN_REW_SHOW' is 'X' but 'CALC_FIN_REW' has no price");
            }

            //if (offer.TextParameters.HasValue("CALC_TOTAL_SAVE", 0.1))
            //{
            //    header.Price2Description = offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE_DESCRIPTION");
            //    header.Price2Value = offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE") + " " + offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE_DISPLAY_UNIT");
            //}

            return header;
        }

        protected internal ProductInfoPrice[] GetProductInfos(OffersModel offer)
        {
            var infoPrices = new List<ProductInfoPrice>();
            var productType = string.Empty;

            // gas
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_GAS"))
            {
                var infoPrice = new ProductInfoPrice();
                infoPrice.Title = this.TextService.FindByKey("CONSUMED_GAS");
                infoPrice.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS");
                infoPrice.PriceUnit = offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_GAS", "CALC_COMP_GAS_PRICE"))
                {
                    infoPrice.PreviousPrice = offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS_PRICE_DISPLAY_UNIT");
                }

                infoPrices.Add(infoPrice);
                productType = "G";
            }
            else if (this.CanDisplayPrice(offer.TextParameters, "CALC_CAP_PRICE"))
            {
                var infoPrice = new ProductInfoPrice();
                infoPrice.Title = this.TextService.FindByKey("ANNUAL_PRICE_FOR_RESERVED_CAPACITY");
                infoPrice.Price = offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE");
                infoPrice.PriceUnit = offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_CAP_PRICE", "CALC_CAP_PRICE_DISC"))
                {
                    infoPrice.PreviousPrice = offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE_DISC") + " " + offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE_DISC_DISPLAY_UNIT");
                }

                infoPrices.Add(infoPrice);
                productType = "G";
            }

            // gas
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_FIX"))
            {
                var infoPrice = new ProductInfoPrice();
                infoPrice.Title = this.TextService.FindByKey("STANDING_PAYMENT");
                infoPrice.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX");
                infoPrice.PriceUnit = offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_FIX", "CALC_COMP_FIX_PRICE"))
                {
                    infoPrice.PreviousPrice = offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX_PRICE_DISPLAY_UNIT");
                }

                infoPrices.Add(infoPrice);
                productType = "G";
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_VT"))
            {
                var infoPrice = new ProductInfoPrice();
                infoPrice.Title = this.TextService.FindByKey("HIGH_TARIFF");
                infoPrice.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_VT");
                infoPrice.PriceUnit = offer.TextParameters.GetValueOrDefault("CALC_COMP_VT_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_VT", "CALC_COMP_VT_PRICE"))
                {
                    infoPrice.PreviousPrice = offer.TextParameters.GetValueOrDefault("CALC_COMP_VT_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_VT_PRICE_DISPLAY_UNIT");
                }

                infoPrices.Add(infoPrice);
                productType = "E";
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_NT"))
            {
                var infoPrice = new ProductInfoPrice();
                infoPrice.Title = this.TextService.FindByKey("LOW_TARIFF");
                infoPrice.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_NT");
                infoPrice.PriceUnit = offer.TextParameters.GetValueOrDefault("CALC_COMP_NT_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_NT", "CALC_COMP_NT_PRICE"))
                {
                    infoPrice.PreviousPrice = offer.TextParameters.GetValueOrDefault("CALC_COMP_NT_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_NT_PRICE_DISPLAY_UNIT");
                }

                infoPrices.Add(infoPrice);
                productType = "E";
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_KC"))
            {
                var infoPrice = new ProductInfoPrice();
                infoPrice.Title = this.TextService.FindByKey("STANDING_PAYMENT");
                infoPrice.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_KC");
                infoPrice.PriceUnit = offer.TextParameters.GetValueOrDefault("CALC_COMP_KC_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_KC", "CALC_COMP_KC_PRICE"))
                {
                    infoPrice.PreviousPrice = offer.TextParameters.GetValueOrDefault("CALC_COMP_KC_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_KC_PRICE_DISPLAY_UNIT");
                }

                infoPrices.Add(infoPrice);
                productType = "E";
            }

            return infoPrices.ToArray();
        }

        /// <summary>
        /// Gets <c>G</c> for gas, <c>E</c> for electricity.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        protected internal string GetProductType(OffersModel offer)
        {
            if (!string.IsNullOrEmpty(offer.EanOrAndEic))
            {
                if (offer.EanOrAndEic.StartsWith("859182400"))
                {
                    return "E";
                }

                if (offer.EanOrAndEic.StartsWith("27ZG"))
                {
                    return "G";
                }
            }

            if (offer.TextParameters.ContainsKey("PERSON_PREMLABEL"))
            {
                if (offer.TextParameters["PERSON_PREMLABEL"] == "EIC")
                {
                    return "G";
                }
                
                if (offer.TextParameters["PERSON_PREMLABEL"] == "EAN")
                {
                    return "E";
                }
            }

            // gas
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_GAS"))
            {
                return "G";
            }
            else if (this.CanDisplayPrice(offer.TextParameters, "CALC_CAP_PRICE"))
            {
                return "G";
            }

            // gas
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_FIX"))
            {
                return "G";
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_VT"))
            {
                return "E";
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_NT"))
            {
                return "E";
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_KC"))
            {
                return "E";
            }

            return String.Empty;
        }

        protected internal string GetProductNote(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }

            var productsRoot = this.SitecoreService.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS);

            if (productsRoot == null)
            {
                return null;
            }

            if (type == "G")
            {
                var value = productsRoot.InfoGas?.Trim();

                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                return value;
            }

            if (type == "E")
            {
                var value = productsRoot.InfoElectricity?.Trim();

                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                return value;
            }

            return null;
        }

        protected internal string[] GetMiddleTexts(OffersModel offer)
        {
            var middleTexts = new List<string>();

            if (offer?.TextParameters == null)
            {
                return middleTexts.ToArray();
            }

            if (offer.TextParameters.HasValue("SA04_MIDDLE_TEXT"))
            {
                middleTexts.Add(offer.TextParameters["SA04_MIDDLE_TEXT"]);
            }

            if (offer.TextParameters.HasValue("SA05_MIDDLE_TEXT"))
            {
                middleTexts.Add(offer.TextParameters["SA05_MIDDLE_TEXT"]);
            }

            if (offer.TextParameters.HasValue("SA06_MIDDLE_TEXT"))
            {
                if (middleTexts.Count == 0)
                {
                    middleTexts.Add(offer.TextParameters["SA06_MIDDLE_TEXT"]);
                }
            }

            return middleTexts.ToArray();
        }

        protected internal string[] GetBenefits(OffersModel offer)
        {
            var benefits = new List<string>();

            if (offer?.TextParameters == null)
            {
                return benefits.ToArray();
            }

            if (offer.TextParameters.HasValue("SA01_MIDDLE_TEXT"))
            {
                benefits.Add(offer.TextParameters["SA01_MIDDLE_TEXT"]);
            }

            if (offer.TextParameters.HasValue("SA02_MIDDLE_TEXT"))
            {
                benefits.Add(offer.TextParameters["SA02_MIDDLE_TEXT"]);
            }

            if (offer.TextParameters.HasValue("SA03_MIDDLE_TEXT"))
            {
                benefits.Add(offer.TextParameters["SA03_MIDDLE_TEXT"]);
            }

            return benefits.ToArray();
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

        protected internal IEnumerable<JsonSalesArgumentsModel> GetSalesArgumentsWithPrefix(IDictionary<string, string> textParameters, string prefix)
        {
            var list = new List<JsonSalesArgumentsModel>();

            if (!this.IsSectionChecked(textParameters, prefix))
            {
                return list;
            }

            var regex = new Regex($"^{prefix}_ACCEPT_LABEL(_[0-9])?$");
            var labels = textParameters.Where(x => regex.Match(x.Key).Success);

            if (!labels.Any())
            {
                return list;
            }

            foreach (var label in labels)
            { 
                var model = new JsonSalesArgumentsExtendedModel();
                model.Title = Utils.StripHtml(label.Value);

                // ADD_SERVICES_ACCEPT_LABEL
                // ADD_SERVICES_SALES_ARGUMENTS_ATRIB_VALUE
                // ADD_SERVICES_SALES_ARGUMENTS_ATRIB_VALUE_1

                // ADD_SERVICES_ACCEPT_LABEL_1
                // ADD_SERVICES_SALES_ARGUMENTS_1_ATRIB_VALUE
                // ADD_SERVICES_SALES_ARGUMENTS_1_ATRIB_VALUE_1

                // ADD_SERVICES_ACCEPT_LABEL_2
                // ADD_SERVICES_SALES_ARGUMENTS_2_ATRIB_VALUE
                // ADD_SERVICES_SALES_ARGUMENTS_2_ATRIB_VALUE_1

                var parameterName = label.Key.Replace("_ACCEPT_LABEL", "_OFFER_SUMMARY") + "_ATRIB_NAME";
                var parameterNames = textParameters.Where(x => x.Key.StartsWith(parameterName));

                if (parameterNames.Any())
                {
                    var parameters = new List<JsonParamModel>();

                    foreach (var item in parameterNames)
                    {
                        var name = item.Value;
                        var value = textParameters[item.Key.Replace("_ATRIB_NAME", "_ATRIB_VALUE")];
                        parameters.Add(new JsonParamModel(name, value));
                    }

                    model.Summary = parameters;
                }

                var attributeName = label.Key.Replace("_ACCEPT_LABEL", "_SALES_ARGUMENTS") + "_ATRIB_VALUE";
                var saleArgumentValues = textParameters.Where(x => x.Key.StartsWith(attributeName));

                if (saleArgumentValues.Any())
                {
                    var salesArguments = new List<JsonArgumentModel>();

                    foreach (var item in saleArgumentValues)
                    {
                        salesArguments.Add(new JsonArgumentModel(item.Value));
                    }

                    model.Arguments = salesArguments;
                }

                if (model.Summary?.Count() > 0 || model.Arguments?.Count() > 0)
                {
                    list.Add(model);
                }
            }

            return list;
        }

        protected internal IEnumerable<IDataModel> GetSalesArgumentsWithPrefix2(IDictionary<string, string> textParameters, string prefix)
        {
            var list2 = new List<IDataModel>();

            if (!this.IsSectionChecked(textParameters, prefix))
            {
                return list2;
            }

            var regex = new Regex($"^{prefix}_ACCEPT_LABEL(_[0-9])?$");
            var labels = textParameters.Where(x => regex.Match(x.Key).Success);

            if (!labels.Any())
            {
                return list2;
            }

            foreach (var label in labels)
            {
                var model2 = new BenefitDataModel();
                var header = new BenefitDataHeaderModel();
                header.Title = Utils.StripHtml(label.Value);
                model2.Header = header;

                var body = new BenefitDataBodyModel();
                body.Infos = GetInfos(label, textParameters);
                body.Points = GetPoints(label, textParameters);
                model2.Body = body;

                if (body.Infos?.Count() > 0 || body.Points?.Count() > 0)
                {
                    list2.Add(model2);
                }
            }            

            return list2;
        }

        protected internal IEnumerable<TitleAndValueModel> GetInfos(KeyValuePair<string, string> label, IDictionary<string, string> textParameters)
        {
            var parameterName = label.Key.Replace("_ACCEPT_LABEL", "_OFFER_SUMMARY") + "_ATRIB_NAME";
            var parameterNames = textParameters.Where(x => x.Key.StartsWith(parameterName));

            if (parameterNames.Any())
            {
                var infos = new List<TitleAndValueModel>();

                foreach (var item in parameterNames)
                {
                    var name = item.Value;
                    var value = textParameters[item.Key.Replace("_ATRIB_NAME", "_ATRIB_VALUE")];
                    infos.Add(new TitleAndValueModel(name, value));
                }

                return infos;
            }
            return null;
        }

        protected internal IEnumerable<ValueModel> GetPoints(KeyValuePair<string, string> label, IDictionary<string, string> textParameters)
        {
            var attributeName = label.Key.Replace("_ACCEPT_LABEL", "_SALES_ARGUMENTS") + "_ATRIB_VALUE";
            var saleArgumentValues = textParameters.Where(x => x.Key.StartsWith(attributeName));

            if (saleArgumentValues.Any())
            {
                var points = new List<ValueModel>();

                foreach (var item in saleArgumentValues)
                {
                    points.Add(new ValueModel(item.Value));
                }

                return points;
            }
            return null;
        }


        protected internal bool HasValue(IDictionary<string, string> textParameters, string key, double minValue)
        {
            if (!textParameters.HasValue(key))
            {
                return false;
            }

            var d = textParameters[key].Replace(" ", string.Empty).Replace(',', '.');

            if (double.TryParse(d, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                if (value >= minValue)
                {
                    return true;
                }
            }

            return false;
        }

        protected internal bool IsVisible(IDictionary<string, string> textParameters, string keyPrefix)
        {
            var key = keyPrefix + "_VISIBILITY";

            if (!textParameters.ContainsKey(key))
            {
                return true;
            }

            return textParameters[key] != Constants.HIDDEN;
        }

        protected internal bool CanDisplayPrice(IDictionary<string, string> textParameters, string prefix)
        {
            if (!this.IsVisible(textParameters, prefix))
            {
                return false;
            }

            if (!this.HasValue(textParameters, prefix, 0.1))
            {
                return false;
            }

            return true;
        }

        protected internal bool CanDisplayPreviousPrice(IDictionary<string, string> textParameters, string currentPriceKey, string previousPriceKey)
        {
            if (!this.IsVisible(textParameters, previousPriceKey))
            {
                return false;
            }

            if (!this.HasValue(textParameters, previousPriceKey, 0.1))
            {
                return false;
            }

            if (!textParameters.IsDifferentDoubleValue(currentPriceKey, previousPriceKey))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Go through <paramref name="files"/> and finding match with <paramref name="productInfos"/>.
        /// If match found, a <see cref="JsonAcceptFileModel"/> is enriched with <see cref="IProductInfoModel"/> container.
        /// </summary>
        /// <param name="files">Collection of files in one group.</param>
        /// <param name="productInfos">Collection of all product information.</param>
        protected internal void UpdateProductInfo(IEnumerable<JsonAcceptFileModel> files, IProductInfoModel[] productInfos)
        {
            if (!(files?.Any() ?? false))
            {
                return;
            }

            var filesArray = files.ToArray();

            for (int i = 0; i < filesArray.Length; i++)
            {
                var file = filesArray[i];
                var productInfo = this.GetMatchedProductInfo(file, productInfos);

                if (productInfo != null)
                {
                    var note = productInfo.Note;
                    file.Note = note;
                    file.MatchedProductInfo = productInfo;

                    if (i > 0)
                    {
                        var previousFile = filesArray[i - 1];

                        if (file.MatchedProductInfo == productInfo && previousFile.Note == note)
                        {
                            previousFile.Note = null;
                            previousFile.MatchedProductInfo = null;
                        }
                    }
                }
            }

            //var filesArray = files.ToArray();
            //var productGroup = filesArray.First().Product;

            //for (int i = 0; i < filesArray.Length; i++)
            //{
            //    var file = filesArray[i];

            //    if (filesArray.Length == 1)
            //    {
            //        var data = productInfos.FirstOrDefault(x => x.Key == file.Product);
            //        file.Note = data?.Note;
            //    }
            //    else if (i >= 1)
            //    {
            //        if (filesArray[i - 1].Product != file.Product)
            //        {
            //            var previousFile = filesArray[i - 1];
            //            var data = productInfos.FirstOrDefault(x => x.Key == previousFile.Product);
            //            previousFile.Note = data?.Note;
            //        }

            //        if (i == filesArray.Length - 1)
            //        {
            //            var data = productInfos.FirstOrDefault(x => x.Key == file.Product);
            //            file.Note = data?.Note;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Finds match in <paramref name="file"/> attributes and <paramref name="productInfos"/>.
        /// </summary>
        /// <param name="file">A file to find match with <see cref="IProductInfoModel"/>.</param>
        /// <param name="productInfos">Collection of all product information.</param>
        /// <returns>If match found, returns <see cref="IProductInfoModel"/>, otherwise null.</returns>
        protected internal IProductInfoModel GetMatchedProductInfo(JsonAcceptFileModel file, IProductInfoModel[] productInfos)
        {
            var matchedProductInfos = new List<KeyValuePair<IProductInfoModel, string[]>>();

            foreach (var productInfo in productInfos)
            {
                int matches = 0;
                var matchesKeys = new List<string>();
                var attributes = productInfo.XmlAttributes;

                foreach (var key in attributes.AllKeys)
                {
                    var value = attributes[key];

                    if (value == "-" || value == "–")
                    {
                        value = string.Empty;
                    }

                    if (file.XmlAttributes.ContainsKey(key) && file.XmlAttributes[key] == value)
                    {
                        matches++;
                        matchesKeys.Add(key);
                    }
                }

                if (matches == productInfo.XmlAttributes.Count)
                {
                    matchedProductInfos.Add(new KeyValuePair<IProductInfoModel, string[]>(productInfo, matchesKeys.ToArray()));
                }
            }

            if (matchedProductInfos.Count == 0)
            {
                return null;
            }

            if (matchedProductInfos.Count == 1)
            {
                return matchedProductInfos.First().Key;
            }

            var mostMatchesProductInfoKeys = matchedProductInfos.OrderByDescending(x => x.Value.Length).First().Key;

            return mostMatchesProductInfoKeys;
        }
    }
}
