﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Models.JsonDescriptor;
using Glass.Mapper;
using Glass.Mapper.Sc;
using JSNLog.Infrastructure;
using Sitecore.ApplicationCenter.Applications;
using Sitecore.Pipelines.RenderField;
using Sitecore.Publishing.Explanations;
using Sitecore.Security.Accounts;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.StringExtensions;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using static eContracting.Constants;
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
        public ContainerModel GetSummary2(OffersModel offer, UserCacheDataModel user)
        {
            var container = new ContainerModel();
            var contractualData = this.GetPersonalData2(offer);

            if (contractualData != null)
            {
                container.Data.Add(contractualData);
            }

            var productData = this.GetProductData2(offer);

            if (productData != null)
            {
                container.Data.Add(productData);
            }

            bool excludeCommodity = productData != null;
            var benefitData = this.GetBenefitsData(offer.TextParameters, excludeCommodity);

            foreach (var benefit in benefitData)
            {
                container.Data.Add(benefit);
            }

            var competitorData = this.GetCompetitorData(offer);

            if (competitorData != null)
            {
                container.Data.Add(competitorData);
            }

            var definition = this.SettingsReaderService.GetDefinition(offer);
            var giftData = this.GetGifts2(offer.TextParameters, definition);

            if (giftData != null)
            {
                container.Data.Add(giftData);
            }

            return container;
        }

        /// <inheritdoc/>
        public JsonOfferNotAcceptedModel GetNew(OffersModel offer, UserCacheDataModel user)
        {
            var attachments = this.OfferService.GetAttachments(offer, user);
            return this.GetNew(offer, attachments);
        }

        public ContainerModel GetNew2(OffersModel offer, UserCacheDataModel user)
        {
            var attachments = this.OfferService.GetAttachments(offer, user);
            return this.GetNew2(offer, attachments);
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
            var header = new TitleDataHeaderModel();
            header.Title = this.TextService.FindByKey("CONTRACTUAL_DATA");
            model.Header = header;

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
            model.Body = body; 

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
        /// <returns></returns>
        protected internal IDataModel GetProductData2(OffersModel offer)
        {
            if (!offer.ShowPrices)
            {
                return null;
            }

            var model = new ProductDataModel();
            model.Header = this.GetProductDataHeader(offer);            
            model.Body = GetProductDataBody(offer);
            if (model.Body == null)
            { 
                return null; 
            }
            return model;
        }

        private ProductDataBodyModel GetProductDataBody(OffersModel offer)
        {
            var body = new ProductDataBodyModel();
            body.Prices = this.GetProductPrices(offer);
            body.Infos = this.GetProductDataInfos(offer);
            if (body.Infos.Count() > 0 && offer.TextParameters.HasValue("SA06_MIDDLE_TEXT"))
            {
                body.InfoHelp = offer.TextParameters["SA06_MIDDLE_TEXT"];
            }
            body.Points = this.GetProductDataPoints(offer);

            if (!body.Prices.Any() && !body.Infos.Any() && !body.Points.Any())
            {
                return null;
            }

            return body;
        }


        /// <summary>
        /// Gets container when user / offer changes a distributor (dismissal previous dealer).
        /// </summary>
        /// <param name="offer"></param>
        /// <returns>Null when text parameter <c>PERSON_COMPETITOR_NAME</c> is missing OR <see cref="OfferModel.Process"/> not equals to <c>01</c>. Otherwise container object.</returns>
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

        protected internal CompetitorDataModel GetCompetitorData(OffersModel offer)
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
            var model = new CompetitorDataModel();
            var header = new TitleDataHeaderModel();
            header.Title = summaryPage.DistributorChange_Title;
            model.Header = header;
            var body = new CompetitorDataBodyModel();
            body.Name = offer.TextParameters["PERSON_COMPETITOR_NAME"];
            body.Text = summaryPage.DistributorChange_Text;
            model.Body = body;
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
            // ToDo: Delete this after AnalyzeOfferCommand is rewritten
            
            var definition = this.SettingsReaderService.GetDefinition(offer);
            //var productInfos = this.SettingsReaderService.GetAllProductInfos();
            var model = new JsonOfferNotAcceptedModel();

            if (offer.Version > 1)
            {
                if (definition.OfferPerexShow)
                {
                    //model.Perex = this.GetPerex(offer.TextParameters, definition);
                }

                // disabled by https://jira.innogy.cz/browse/TRUNW-475
                //if (definition.OfferGiftsShow)
                //{
                //    model.Gifts = this.GetGifts(offer.TextParameters, definition);
                //}

                // for version 3 container are picked up by GetSummary method
                //if (offer.Version == 2)
                //{
                //    model.SalesArguments = this.GetCommoditySalesArguments(offer.TextParameters, definition);
                //}
            }

            //model.Documents = this.GetDocuments(offer, definition, productInfos, attachments);
            
            model.AcceptanceDialog = this.GetAcceptance(offer, definition); //ToDo: Implement Acceptance dialogue to the new JSON structure
            return model;
        }

        /// <summary>
        /// Gets container for new non-accepted offer.
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="attachments"></param>
        /// <returns>Always returns the object.</returns>
        protected internal ContainerModel GetNew2(OffersModel offer, OfferAttachmentModel[] attachments)
        {
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var productInfos = this.SettingsReaderService.GetAllProductInfos();
            
            var container = new ContainerModel();          
            
            // perex moved to docsCheck section

            if (offer.Version == 2)
            {
                var benefitData = this.GetCommoditySalesArguments2(offer.TextParameters, definition);
                if (benefitData != null)
                {
                    container.Data.Add(benefitData);
                }
            }
                                    
            var docsAcceptanceData = this.GetDocumentsAcceptance2(offer, definition, productInfos, attachments);
            if (docsAcceptanceData != null)
            {
                container.Data.Add(docsAcceptanceData);            
            }            
            var docsSignData = this.GetDocumentsSign2(offer, definition, productInfos, attachments);            
            if (docsSignData != null)
            {
                container.Data.Add(docsSignData);
            }
            
            var docsOtherServices = this.GetOtherProducts2(offer, attachments, definition, productInfos); 
            
            if (docsOtherServices != null)
            {
                container.Data.Add(docsOtherServices);
            }

            var docsAdditionalServices = this.GetAdditionalServices2(offer, attachments, definition, productInfos);
            if (docsAdditionalServices != null)
            {
                container.Data.Add(docsAdditionalServices);
            }

            var returnMandatory = true;
            var  mandatoryUploads = this.GetUploadDataModel2(offer, attachments, definition, returnMandatory);
            if (mandatoryUploads != null)
            {
                container.Data.Add(mandatoryUploads);
            }

            returnMandatory = false;
            var optionalUploads = this.GetUploadDataModel2(offer, attachments, definition, returnMandatory);
            if (optionalUploads != null)
            {
                container.Data.Add(optionalUploads);
            }

            //ToDo: ("type": "confirm")
            //model.AcceptanceDialog = this.GetAcceptance(offer, definition);

            return container;
        }

        public ContainerModel GetUploads(OffersModel offer, UserCacheDataModel user)
        {
            var attachments = this.OfferService.GetAttachments(offer, user);
            var definition = this.SettingsReaderService.GetDefinition(offer);

            var container = new ContainerModel();
            
            var returnMandatory = true;
            var mandatoryUploads = this.GetUploadDataModel2(offer, attachments, definition, returnMandatory);
            if (mandatoryUploads != null && ((UploadDataBodyModel)mandatoryUploads.Body).Docs?.Files?.Count() > 0)
            {
                container.Data.Add(mandatoryUploads);
            }

            returnMandatory = false;
            var optionalUploads = this.GetUploadDataModel2(offer, attachments, definition, returnMandatory);
            if (optionalUploads != null && ((UploadDataBodyModel)optionalUploads.Body).Docs?.Files?.Count() > 0)
            {
                container.Data.Add(optionalUploads);
            }            

            return container;
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

        protected internal PerexDataModel GetPerex2(IDictionary<string, string> textParameters, IDefinitionCombinationModel definition)
        {
            var parameters = new List<TitleAndValueModel>();
            var keys = textParameters.Keys.Where(x => x.StartsWith("COMMODITY_OFFER_SUMMARY_ATRIB_NAME")).ToArray();
            var values = textParameters.Keys.Where(x => x.StartsWith("COMMODITY_OFFER_SUMMARY_ATRIB_VALUE")).ToArray();


            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var title = textParameters[key];
                var value = this.GetEnumPairValue(key, textParameters);

                if (value != null)
                {
                    parameters.Add(new TitleAndValueModel(title, value));
                }
            }

            if (parameters.Count > 0)
            {
                var model = new PerexDataModel();                

                var header = new TitleDataHeaderModel();
                header.Title = definition.OfferPerexTitle?.Text.Trim();
                model.Header = header;

                var body = new PerexBodyModel();
                body.Params = parameters;
                model.Body = body;
                
                return model;
            }

            return null;                        
        }

        /// <summary>
        /// Gets all benefits data (former sales arguments).
        /// </summary>
        /// <param name="textParameters">Text parameters.</param>
        /// <param name="excludeCommodity">if true commodity aren't returned</param>
        /// <returns>All sales arguments or empty array.</returns>
        protected internal IEnumerable<IDataModel> GetBenefitsData(IDictionary<string, string> textParameters, bool excludeCommodity)
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


        //ToDo: Temporarily kept for potentional use in new perex, so that FE can distinguish where to render for two commodities.
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

        protected internal string GetCommodityProductType(IDictionary<string, string> textParameters)
        {
            var values = textParameters.Where(x => x.Key.StartsWith("COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE")).Select(x => x.Value).ToArray();

            if (values.Length == 0)
            {
                return null;
            }
            
            var commodityProductTypeAttribute = textParameters.FirstOrDefault(x => x.Key.StartsWith("COMMODITY_PRODUCT"));
            if (!commodityProductTypeAttribute.Equals(default(KeyValuePair<string, string>)))
            {
                string commodityProductTypeAttributeValue = commodityProductTypeAttribute.Value;
                if (!string.IsNullOrEmpty(commodityProductTypeAttributeValue))
                {
                    if (commodityProductTypeAttributeValue.StartsWith("G_"))
                        return CommodityProductType.GAS;
                    else if (commodityProductTypeAttributeValue.StartsWith("E_") || commodityProductTypeAttributeValue.StartsWith("EE_") || commodityProductTypeAttributeValue.EndsWith("_EE") || commodityProductTypeAttributeValue.EndsWith("_E"))
                        return CommodityProductType.ELECTRICITY;
                }
            }

            return null;
        }

        protected internal string GetDocsCheckTypeByCommodity(string commodity)
        {
            if (!string.IsNullOrEmpty(commodity))
            {
                if (commodity == CommodityProductType.GAS)
                    return JsonDocumentDataModelType.DOCS_CHECK_G;
                if (commodity == CommodityProductType.ELECTRICITY)
                    return JsonDocumentDataModelType.DOCS_CHECK_E;
                if (commodity == "EG")
                    return JsonDocumentDataModelType.DOCS_CHECK_E_G;
            }
            return string.Empty;
        }

        protected internal string GetDocsSignTypeByCommodity(string commodity)
        {
            if (!string.IsNullOrEmpty(commodity))
            {
                if (commodity == CommodityProductType.GAS)
                    return JsonDocumentDataModelType.DOCS_SIGN_G;
                if (commodity == CommodityProductType.ELECTRICITY)
                    return JsonDocumentDataModelType.DOCS_SIGN_E;
                if (commodity == "EG")
                    return JsonDocumentDataModelType.DOCS_SIGN_E_G;
            }
            return string.Empty;
        }

        protected internal BenefitDataModel GetCommoditySalesArguments2(IDictionary<string, string> textParameters, IDefinitionCombinationModel definition)
        {
            var values = textParameters.Where(x => x.Key.StartsWith("COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE")).Select(x => x.Value).ToArray();
            
            if (values.Length == 0)
            {
                return null;
            }

            var model = new BenefitDataModel();

            model.Position = 2; // ToDo: Solve order

            var header = new TitleDataHeaderModel();
            header.Title = definition.OfferBenefitsTitle?.Text.Trim();
            model.Header = header;

            var body = new BenefitDataBodyModel();
            body.Points = values.Select(x => new ValueModel(x));
            model.Body = body;           

            return model;
        }

        /// <summary>
        /// Gets gifts from <c>BENEFITS</c> parameters.
        /// </summary>
        /// <param name="textParameters">Text parameters.</param>
        /// <param name="definition">The matrix combination.</param>
        /// <returns>Gets group of gifts or null if not <c>BENEFITS</c> found.</returns>
        protected internal GiftDataModel GetGifts2(IDictionary<string, string> textParameters, IDefinitionCombinationModel definition)
        {
            if (!this.IsSectionChecked(textParameters, "BENEFITS"))
            {
                return null;
            }
                        
            var keys = new[] { "BENEFITS_NOW", "BENEFITS_NEXT_SIGN", "BENEFITS_NEXT_TZD" };
            var groups = new List<GiftDataGroupModel>();


            for (int i = 0; i < keys.Length; i++)
            {
                var k = keys[i];
                var g = this.GetGiftDataGroupModel(k, textParameters);

                if (g != null)
                {
                    groups.Add(g);
                }
            }

            if (groups.Count == 0)
            {
                return null;
            }

            var model = new GiftDataModel();
            var modelHeader = new GiftDataHeaderModel();
            modelHeader.Title = definition.OfferGiftsTitle?.Text.Trim();

            if (textParameters.HasValue("BENEFITS_CLOSE"))
            {
                modelHeader.Note = textParameters["BENEFITS_CLOSE"];
            }
            model.Header = modelHeader;
            var modelBody = new GiftDataBodyModel();
            modelBody.Groups = groups;
            model.Body = modelBody;

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

        protected internal GiftDataGroupModel GetGiftDataGroupModel(string key, IDictionary<string, string> textParameters)
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

            var group = new GiftDataGroupModel();

            if (keys.Contains(keyIntro))
            {
                group.Title = textParameters[keyIntro];
            }

            var list = new List<GiftDataParamsModel>();
            var names = textParameters.Keys.Where(x => x.StartsWith(keyName)).ToArray();

            for (int i = 0; i < names.Length; i++)
            { 
                var paramsModel = new GiftDataParamsModel();
                var nameKey = names[i];

                if (textParameters.TryGetValue(nameKey, out string nameValue))
                {
                    paramsModel.Title = nameValue;
                }

                var iconKey = keys.FirstOrDefault(x => x == nameKey.Replace(keyName, keyImage));

                if (!string.IsNullOrEmpty(iconKey))
                {
                    if (textParameters.TryGetValue(iconKey, out string iconValue))
                    {
                        paramsModel.Icon = iconValue;
                    }
                }

                var countKey = keys.FirstOrDefault(x => x == nameKey.Replace(keyName, keyCount));

                if (!string.IsNullOrEmpty(countKey))
                {
                    if (textParameters.TryGetValue(countKey, out string countValue))
                    {
                        if (int.TryParse(countValue, out int c))
                        {
                            paramsModel.Count = c;
                        }
                    }
                }

                list.Add(paramsModel);
            }

            group.Params = list;
            return group;
        }

        protected internal DocumentDataModel GetDocumentsAcceptance2(OffersModel offer, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos, OfferAttachmentModel[] files)
        {
            var selectedFiles = files.Where(x => x.Group == Constants.OfferGroups.COMMODITY && x.IsPrinted == true && x.IsSignReq == false).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new DocumentDataModel();
            
            var commodityProductType = GetCommodityProductType(offer.TextParameters);
            model.Type = GetDocsCheckTypeByCommodity(commodityProductType); // docsCheck-E  |  docsCheck-G  |  docsCheck-E/G

            model.Position = 3; // ToDo: Solve order
            var header = new TitleDataHeaderModel();
            header.Title = "header -> Title"; // ToDo: Documenty k podpisu - doesn't exist in original JSON but requested in new design
            model.Header = header;

            var body = new DocumentDataBodyModel();            

            var bodyHead = new DocumentDataBodyHeadModel();
            bodyHead.Title = definition.OfferDocumentsForElectronicAcceptanceTitle?.Text.Trim();
            bodyHead.Text = Utils.GetReplacedTextTokens(definition.OfferDocumentsForElectronicAcceptanceText?.Text, offer.TextParameters);
            body.Head = bodyHead;

            body.Text = "body -> Text"; // ToDo: není rich text mezi šedými bloky
                        
            body.Docs = this.GetDocsDataModel(offer, definition, productInfos, selectedFiles, false, Constants.OfferGroups.COMMODITY);

            body.Note = null; // ToDo

            model.Body = body;
            return model;
        }

        protected internal DocumentDataModel GetDocumentsSign2(OffersModel offer, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos, OfferAttachmentModel[] files)
        {
            var selectedFiles = files.Where(x => x.Group == Constants.OfferGroups.COMMODITY && x.IsPrinted == true && x.IsSignReq == true).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new DocumentDataModel();
            model.Position = 4; // ToDo: Solve order
            
            if (offer.Process != "06") // ToDo: Check BUS_PROCESS - if odhlaska == "06"
            { 
                model.Type = "docsSign"; 
            }
            else
            {
                var commodityProductType = GetCommodityProductType(offer.TextParameters);
                model.Type = GetDocsSignTypeByCommodity(commodityProductType); // docsSign-E  |  docsSign-G  |  docsSign-E/G
            }
            
            var header = new TitleDataHeaderModel();
            header.Title = "header -> Title"; // ToDo: Documenty k podpisu - doesn't exist in original JSON but requested in new design
            model.Header = header;

            var body = new DocumentDataBodyModel();

            var bodyHead = new DocumentDataBodyHeadModel();
            bodyHead.Title = definition.OfferDocumentsForSignTitle?.Text.Trim();            
            bodyHead.Text = Utils.GetReplacedTextTokens(definition.OfferDocumentsForSignText?.Text, offer.TextParameters);
            body.Head = bodyHead;

            body.Text = "body -> Text"; // ToDo: není rich text mezi šedými bloky

            body.Docs = this.GetDocsDataModel(offer, definition, productInfos, selectedFiles, true, Constants.OfferGroups.COMMODITY);

            body.Note = "body -> Note"; // ToDo

            model.Body = body;
            return model;
        }

        private DocsDataModel GetDocsDataModel(OffersModel offer, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos, OfferAttachmentModel[] selectedFiles, bool isSignReq, string group)
        {
            var model = new DocsDataModel();            
            if (group == Constants.OfferGroups.COMMODITY && !isSignReq) //Acceptance
            {
                //perex
                if (offer.Version > 1)
                {
                    if (definition.OfferPerexShow)
                    {
                        var perexData = this.GetPerex2(offer.TextParameters, definition);

                        if (perexData != null)
                        {
                            model.Perex = perexData;
                        }
                    }
                }
                    
                model.Title = definition.OfferDocumentsForAcceptanceTitle?.Text.Trim();

                var eicEanLabel = offer.TextParameters.GetValueOrDefault("PERSON_PREMLABEL");
                var eicEanValue = offer.TextParameters.GetValueOrDefault("PERSON_PREMEXT");
                var parameters = new List<TitleAndValueModel>();
                if (eicEanLabel != null && eicEanValue != null)
                {
                    parameters.Add(new TitleAndValueModel(eicEanLabel, eicEanValue));
                }
                else
                {
                    parameters = null;
                }
                model.Params = parameters;
                
                model.Text = Utils.GetReplacedTextTokens(definition.OfferDocumentsForAcceptanceText?.Text, offer.TextParameters);
            }
            else if (group == Constants.OfferGroups.COMMODITY) // Signed
            {
                model.Title = isSignReq ? definition.OfferDocumentsForSignTitle?.Text.Trim() : definition.OfferDocumentsForAcceptanceTitle?.Text.Trim();
                model.Params = null; 
                model.Text = Utils.GetReplacedTextTokens(definition.OfferDocumentsForSignText?.Text, offer.TextParameters);
            }
            else if (group == Constants.OfferGroups.NONCOMMODITY)
            {
                model.Title = Utils.GetReplacedTextTokens(definition.OfferOtherProductsDocsTitle?.Text.Trim(), offer.TextParameters);
                model.Params = null; 
                model.Text = Utils.GetReplacedTextTokens(definition.OfferOtherProductsSummaryText?.Text, offer.TextParameters);
            }
            else // DSL
            {
                model.Title = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesDocsTitle?.Text.Trim(), offer.TextParameters);
                model.Params = null; 
                model.Text = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesDocsText?.Text.Trim(), offer.TextParameters);
            }
            
            var list = new List<FileDataModel>();

            for (int i = 0; i < selectedFiles.Length; i++)
            {
                var selectedFile = selectedFiles[i];
                var file = new FileDataModel(selectedFile);
                file.Prefix = this.GetFileLabelPrefix(selectedFile);
                list.Add(file);

                if (file.Mandatory)
                {
                    model.MandatoryGroups.Add(selectedFile.GroupGuid);
                }
            }

            this.UpdateProductInfo2(list, productInfos);

            model.Files = list;
            return model;            
        }

        /// <summary>
        /// Returns <see cref="UploadDataModel"/> for mandatory and optional files
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="files"></param>
        /// <param name="definition"></param>
        /// <param name="returnMandatoryFiles">if true <see cref="UploadDataModel"/> containing mandatory files is returned if false <see cref="UploadDataModel"/> containing optional files</param>
        /// <returns></returns>
        protected internal UploadDataModel GetUploadDataModel2(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, bool returnMandatoryFiles)
        {
            var fileTemplates = files.Where(x => x.IsPrinted == false).ToArray();

            if (fileTemplates.Length == 0)
            {
                return null;
            }

            var model = new UploadDataModel();            

            model.Type = "upload";
            var header = new TitleDataHeaderModel();
            header.Title = ""; // ToDo: Solve Upload title
            model.Header = header;

            var body = new UploadDataBodyModel();

            var docsModel = new UploadDataDocsModel();
            
            var list = new List<JsonUploadTemplateModel>();

            for (int i = 0; i < fileTemplates.Length; i++)
            {
                var template = fileTemplates[i];
                var file = new JsonUploadTemplateModel(template);
                file.Info = this.GetTemplateHelp(template.IdAttach, offer.TextParameters);

                if (file.Mandatory && returnMandatoryFiles)
                {                    
                    list.Add(file); // mandatory

                    model.Position = 7; // ToDo: Solve order
                }
                else if (!file.Mandatory && !returnMandatoryFiles)
                {
                    list.Add(file); // optional

                    model.Position = 8; // ToDo: Solve order
                }
            }

            docsModel.Title = Utils.GetReplacedTextTokens(definition.OfferUploadsTitle?.Text.Trim(), offer.TextParameters);
            docsModel.Title = docsModel.Title?.Replace("Povinné", "Nepovinné"); // ToDo: Temporary solution - New Sitecore text needed
            docsModel.Text = null; // ToDo: Check this
            docsModel.Note = Utils.GetReplacedTextTokens(definition.OfferUploadsNote?.Text.Trim(), offer.TextParameters); // ToDo: Check this
            docsModel.Files = list;

            body.Docs = docsModel;

            model.Body = body;
            return model;
        }

        protected internal DocumentDataModel GetAdditionalServices2(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
        {
            var selectedFiles = files.Where(x => x.Group == Constants.OfferGroups.DSL && x.IsPrinted == true && x.IsSignReq == false).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }

            var model = new DocumentDataModel();

            model.Position = 6; // ToDo: Solve order

            model.Type = "docsCheck";
            var header = new TitleDataHeaderModel();
            header.Title = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesTitle?.Text.Trim(), offer.TextParameters);            
            model.Header = header;

            var body = new DocumentDataBodyModel();

            var bodyHead = new DocumentDataBodyHeadModel();
            bodyHead.Title = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesSummaryTitle?.Text, offer.TextParameters);            
            bodyHead.Text = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesSummaryText?.Text, offer.TextParameters);
            body.Head = bodyHead;

            body.Docs = this.GetDocsDataModel(offer, definition, productInfos, selectedFiles, false, Constants.OfferGroups.DSL);

            body.Note = Utils.GetReplacedTextTokens(definition.OfferAdditionalServicesNote?.Text.Trim(), offer.TextParameters);

            model.Body = body;
            return model;
        }

        protected internal DocumentDataModel GetOtherProducts2(OffersModel offer, OfferAttachmentModel[] files, IDefinitionCombinationModel definition, IProductInfoModel[] productInfos)
        {
            var selectedFiles = files.Where(x => x.Group == Constants.OfferGroups.NONCOMMODITY && x.IsPrinted == true && x.IsSignReq == false && x.IsObligatory == false).ToArray();

            if (selectedFiles.Length == 0)
            {
                return null;
            }
            
            var model = new DocumentDataModel();

            model.Position = 5; // ToDo: Solve order

            model.Type = "docsCheck";
            var header = new TitleDataHeaderModel();
            header.Title = Utils.GetReplacedTextTokens(definition.OfferOtherProductsTitle?.Text.Trim(), offer.TextParameters);
            model.Header = header;

            var body = new DocumentDataBodyModel();
            
            var bodyHead = new DocumentDataBodyHeadModel();
            bodyHead.Title = Utils.GetReplacedTextTokens(definition.OfferOtherProductsSummaryTitle?.Text, offer.TextParameters);            
            bodyHead.Text = Utils.GetReplacedTextTokens(definition.OfferOtherProductsSummaryText?.Text, offer.TextParameters);
            body.Head = bodyHead;

            body.Docs = this.GetDocsDataModel(offer, definition, productInfos, selectedFiles, false, Constants.OfferGroups.NONCOMMODITY);

            body.Note = Utils.GetReplacedTextTokens(definition.OfferOtherProductsNote?.Text.Trim(), offer.TextParameters);

            model.Body = body;
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
        
        protected internal ProductDataHeaderModel GetProductDataHeader(OffersModel offer)
        {
            var header = new ProductDataHeaderModel();
            header.Title = offer.TextParameters.GetValueOrDefault("CALC_PROD_DESC");
            header.Type = this.GetProductType(offer);

            var data = new List<ProductsDataDataModel>();
            var mainData = new ProductsDataDataModel(); // price 1
            var addData = new ProductsDataDataModel(); // price 2

            if (this.CanDisplayPrice(offer.TextParameters, "CALC_TOTAL_SAVE"))
            {
                mainData.Type = "main";
                mainData.Title = offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE_DESCRIPTION");
                mainData.Value = offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE") + " " + offer.TextParameters.GetValueOrDefault("CALC_TOTAL_SAVE_DISPLAY_UNIT");

                if (offer.TextParameters.HasValue("CALC_TOTAL_SAVE_TOOLTIP"))
                {
                    mainData.Note = offer.TextParameters["CALC_TOTAL_SAVE_TOOLTIP"];
                }
            }
            else
            {
                this.Logger.Warn(offer.Guid, "Text parameter 'CALC_TOTAL_SAVE_SHOW' is 'X' but 'CALC_TOTAL_SAVE' has no price");
            }
            if (string.IsNullOrEmpty(mainData.Value))
            {
                if (this.CanDisplayPrice(offer.TextParameters, "CALC_DEP_VALUE"))
                {
                    mainData.Title = offer.TextParameters.GetValueOrDefault("CALC_DEP_VALUE_DESCRIPTION");
                    mainData.Value = offer.TextParameters.GetValueOrDefault("CALC_DEP_VALUE") + " " + offer.TextParameters.GetValueOrDefault("CALC_DEP_VALUE_DISPLAY_UNIT");

                    if (offer.TextParameters.HasValue("CALC_DEP_VALUE_TOOLTIP"))
                    {
                        mainData.Note = offer.TextParameters["CALC_DEP_VALUE_TOOLTIP"];
                    }
                }
                else
                {
                    this.Logger.Warn(offer.Guid, "Text parameter 'CALC_TOTAL_SAVE' and 'CALC_DEP_VALUE' is missing / empty.");
                }
            }
            data.Add(mainData);

            if (this.CanDisplayPrice(offer.TextParameters, "CALC_FIN_REW"))
            {
                addData.Type = "add";
                addData.Title = offer.TextParameters.GetValueOrDefault("CALC_FIN_REW_DESCRIPTION");
                addData.Value = offer.TextParameters.GetValueOrDefault("CALC_FIN_REW") + " " + offer.TextParameters.GetValueOrDefault("CALC_FIN_REW_DISPLAY_UNIT");

                if (offer.TextParameters.HasValue("CALC_FIN_REW_TOOLTIP"))
                {
                    addData.Note = offer.TextParameters["CALC_FIN_REW_TOOLTIP"];
                }
            }
            else
            {
                this.Logger.Warn(offer.Guid, "Text parameter 'CALC_FIN_REW_SHOW' is 'X' but 'CALC_FIN_REW' has no price");
            }

            data.Add(addData);

            header.Data = data;
            return header;
        }

        protected internal IEnumerable<ProductDataPricesModel> GetProductPrices(OffersModel offer)
        {
            var prices = new List<ProductDataPricesModel>();            
            
            // gas
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_GAS"))
            {
                var price = new ProductDataPricesModel();               
                price.Title = this.TextService.FindByKey("CONSUMED_GAS");
                price.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS");
                price.Unit = offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_GAS", "CALC_COMP_GAS_PRICE"))
                {
                    price.Price2 = offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_GAS_PRICE_DISPLAY_UNIT");
                }

                prices.Add(price);                
            }
            else if (this.CanDisplayPrice(offer.TextParameters, "CALC_CAP_PRICE"))
            {
                var price = new ProductDataPricesModel();
                price.Title = this.TextService.FindByKey("ANNUAL_PRICE_FOR_RESERVED_CAPACITY");
                price.Price = offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE");
                price.Unit = offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_CAP_PRICE", "CALC_CAP_PRICE_DISC"))
                {
                    price.Price2 = offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE_DISC") + " " + offer.TextParameters.GetValueOrDefault("CALC_CAP_PRICE_DISC_DISPLAY_UNIT");
                }

                prices.Add(price);                
            }

            // gas
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_FIX"))
            {
                var price = new ProductDataPricesModel();
                price.Title = this.TextService.FindByKey("STANDING_PAYMENT");
                price.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX");
                price.Unit = offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_FIX", "CALC_COMP_FIX_PRICE"))
                {
                    price.Price2 = offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_FIX_PRICE_DISPLAY_UNIT");
                }

                prices.Add(price);                
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_VT"))
            {
                var price = new ProductDataPricesModel();
                price.Title = this.TextService.FindByKey("HIGH_TARIFF");
                price.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_VT");
                price.Unit = offer.TextParameters.GetValueOrDefault("CALC_COMP_VT_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_VT", "CALC_COMP_VT_PRICE"))
                {
                    price.Price2 = offer.TextParameters.GetValueOrDefault("CALC_COMP_VT_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_VT_PRICE_DISPLAY_UNIT");
                }

                prices.Add(price);                
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_NT"))
            {
                var price = new ProductDataPricesModel();
                price.Title = this.TextService.FindByKey("LOW_TARIFF");
                price.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_NT");
                price.Unit = offer.TextParameters.GetValueOrDefault("CALC_COMP_NT_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_NT", "CALC_COMP_NT_PRICE"))
                {
                    price.Price2 = offer.TextParameters.GetValueOrDefault("CALC_COMP_NT_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_NT_PRICE_DISPLAY_UNIT");
                }

                prices.Add(price);                
            }

            // electricity
            if (this.CanDisplayPrice(offer.TextParameters, "CALC_COMP_KC"))
            {
                var price = new ProductDataPricesModel();
                price.Title = this.TextService.FindByKey("STANDING_PAYMENT");
                price.Price = offer.TextParameters.GetValueOrDefault("CALC_COMP_KC");
                price.Unit = offer.TextParameters.GetValueOrDefault("CALC_COMP_KC_DISPLAY_UNIT");

                if (this.CanDisplayPreviousPrice(offer.TextParameters, "CALC_COMP_KC", "CALC_COMP_KC_PRICE"))
                {
                    price.Price2 = offer.TextParameters.GetValueOrDefault("CALC_COMP_KC_PRICE") + " " + offer.TextParameters.GetValueOrDefault("CALC_COMP_KC_PRICE_DISPLAY_UNIT");
                }

                prices.Add(price);
            }

            return prices;
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

        protected internal IEnumerable<ValueModel> GetProductDataInfos(OffersModel offer)
        {
            var infos = new List<ValueModel>();            

            if (offer?.TextParameters == null)
            {
                return infos;
            }

            if (offer.TextParameters.HasValue("SA04_MIDDLE_TEXT"))
            {
                infos.Add(new ValueModel(offer.TextParameters["SA04_MIDDLE_TEXT"]));                
            }

            if (offer.TextParameters.HasValue("SA05_MIDDLE_TEXT"))
            {
                infos.Add(new ValueModel(offer.TextParameters["SA05_MIDDLE_TEXT"]));                
            }

            if (offer.TextParameters.HasValue("SA06_MIDDLE_TEXT"))
            {
                if (infos.Count == 0)
                {
                    infos.Add(new ValueModel(offer.TextParameters["SA06_MIDDLE_TEXT"]));                    
                }
            }

            return infos;
        }

        protected internal IEnumerable<ValueModel> GetProductDataPoints(OffersModel offer)
        {
            var points = new List<ValueModel>();


            if (offer?.TextParameters == null)
            {
                return points;
            }

            if (offer.TextParameters.HasValue("SA01_MIDDLE_TEXT"))
            {
                points.Add(new ValueModel(offer.TextParameters["SA01_MIDDLE_TEXT"]));                
            }

            if (offer.TextParameters.HasValue("SA02_MIDDLE_TEXT"))
            {
                points.Add(new ValueModel(offer.TextParameters["SA02_MIDDLE_TEXT"]));                
            }

            if (offer.TextParameters.HasValue("SA03_MIDDLE_TEXT"))
            {
                points.Add(new ValueModel(offer.TextParameters["SA03_MIDDLE_TEXT"]));                
            }

            return points;
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
                var header = new TitleDataHeaderModel();
                header.Title = Utils.StripHtml(label.Value);
                model2.Header = header;

                var body = new SummaryBenefitDataBodyModel();
                body.Infos = GetAttributesSummary(label, textParameters);
                body.Points = GetSalesArguments(label, textParameters);
                model2.Body = body;

                if (body.Infos?.Count() > 0 || body.Points?.Count() > 0)
                {
                    list2.Add(model2);
                }
            }            

            return list2;
        }

        protected internal IEnumerable<TitleAndValueModel> GetAttributesSummary(KeyValuePair<string, string> label, IDictionary<string, string> textParameters)
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

        protected internal IEnumerable<ValueModel> GetSalesArguments(KeyValuePair<string, string> label, IDictionary<string, string> textParameters)
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
        /// Go through <paramref name="files"/> and finding match with <paramref name="productInfos"/>.
        /// If match found, a <see cref="FileDataModel"/> is enriched with <see cref="IProductInfoModel"/> container.
        /// </summary>
        /// <param name="files">Collection of files in one group.</param>
        /// <param name="productInfos">Collection of all product information.</param>
        protected internal void UpdateProductInfo2(IEnumerable<FileDataModel> files, IProductInfoModel[] productInfos)
        {
            if (!(files?.Any() ?? false))
            {
                return;
            }

            var filesArray = files.ToArray();

            for (int i = 0; i < filesArray.Length; i++)
            {
                var file = filesArray[i];
                var productInfo = this.GetMatchedProductInfo2(file, productInfos);

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

        /// <summary>
        /// Finds match in <paramref name="file"/> attributes and <paramref name="productInfos"/>.
        /// </summary>
        /// <param name="file">A file to find match with <see cref="IProductInfoModel"/>.</param>
        /// <param name="productInfos">Collection of all product information.</param>
        /// <returns>If match found, returns <see cref="IProductInfoModel"/>, otherwise null.</returns>
        protected internal IProductInfoModel GetMatchedProductInfo2(FileDataModel file, IProductInfoModel[] productInfos)
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
