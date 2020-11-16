﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Services
{
    public class SitecoreSettingsReaderService : ISettingsReaderService
    {
        /// <summary>
        /// The context.
        /// </summary>
        protected readonly ISitecoreContext Context;

        /// <summary>
        /// The context wrapper.
        /// </summary>
        protected readonly IContextWrapper ContextWrapper;

        /// <summary>
        /// The randomizer.
        /// </summary>
        protected readonly static Random Rand = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreSettingsReaderService"/> class.
        /// </summary>
        public SitecoreSettingsReaderService(ISitecoreContext sitecoreContext, IContextWrapper contextWrapper)
        {
            this.Context = sitecoreContext ?? throw new ArgumentNullException(nameof(sitecoreContext));
            this.ContextWrapper = contextWrapper ?? throw new ArgumentNullException(nameof(contextWrapper));
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetAllLoginTypes()
        {
            var items = this.Context.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES);

            if (items?.Count() == 0)
            {
                throw new MissingDatasourceException("No login types found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            var items =  this.Context.GetItems<ProcessModel>(Constants.SitecorePaths.PROCESSES);

            if (items?.Count() == 0)
            {
                throw new MissingDatasourceException("No processes found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessTypeModel> GetAllProcessTypes()
        {
            var items = this.Context.GetItems<ProcessTypeModel>(Constants.SitecorePaths.PROCESS_TYPES);

            if (items?.Count() == 0)
            {
                throw new MissingDatasourceException("No process types found");
            }

            return items;
        }

        /// <inheritdoc/>
        public CacheApiServiceOptions GetApiServiceOptions()
        {
            var generalSettings = this.GetSiteSettings();

            var url = generalSettings.ServiceUrl;
            var username = generalSettings.ServiceUser;
            var password = generalSettings.ServicePassword;

            if (string.IsNullOrWhiteSpace(url))
            {
                url = this.ContextWrapper.GetSetting("eContracting.ServiceUrl");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                username = this.ContextWrapper.GetSetting("eContracting.ServiceUser");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                password = this.ContextWrapper.GetSetting("eContracting.ServicePassword");
            }

            var options = new CacheApiServiceOptions(url, username, password);
            return options;
        }

        /// <inheritdoc/>
        public DefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            var definitions = this.Context.GetItems<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);
            return definitions.FirstOrDefault(x => x.Process.Code == offer.Process && x.ProcessType.Code == offer.ProcessType);
        }

        /// <inheritdoc/>
        public DefinitionCombinationModel GetDefinition(string process, string processType)
        {
            var definitions = this.Context.GetItems<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);
            return definitions.FirstOrDefault(x => x.Process.Code.Equals(process, StringComparison.InvariantCultureIgnoreCase) && x.ProcessType.Code.Equals(processType, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetLoginTypes(OfferModel offer)
        {
            var definition = this.GetDefinition(offer);
            var loginTypes = definition.LoginTypes.ToList();

            if (loginTypes.Count == 0)
            {
                var availableLoginTypes = this.GetAllLoginTypes().ToArray();
                var count = availableLoginTypes.Length;

                if (count > 1)
                {
                    var index = Rand.Next(0, count);
                    loginTypes.Add(availableLoginTypes[index]);
                }
                else
                {
                    loginTypes.AddRange(availableLoginTypes);
                }
            }

            return loginTypes;
        }

        /// <inheritdoc/>
        public RichTextModel GetMainTextForLogin(OfferModel offer)
        {
            var definition = this.GetDefinition(offer);
            return offer.IsAccepted ? definition.MainTextLoginAccepted : definition.MainTextLogin;
        }

        /// <inheritdoc/>
        public string GetPageLink(PAGE_LINK_TYPES type)
        {
            var settings = this.GetSiteSettings();

            switch (type)
            {
                case PAGE_LINK_TYPES.Offer:
                    return settings.Offer.Url;
                case PAGE_LINK_TYPES.SessionExpired:
                    return settings.SessionExpired.Url;
                case PAGE_LINK_TYPES.UserBlocked:
                    return settings.UserBlocked.Url;
                case PAGE_LINK_TYPES.AcceptedOffer:
                    return settings.AcceptedOffer.Url;
                case PAGE_LINK_TYPES.WrongUrl:
                    return settings.WrongUrl.Url;
                case PAGE_LINK_TYPES.OfferExpired:
                    return settings.OfferExpired.Url;
                case PAGE_LINK_TYPES.ThankYou:
                    return settings.ThankYou.Url;
                case PAGE_LINK_TYPES.SystemError:
                    return settings.SystemError.Url;
                case PAGE_LINK_TYPES.Welcome:
                    return settings.Welcome.Url;
                case PAGE_LINK_TYPES.Login:
                    return settings.Login.Url;
                default:
                    throw new InvalidOperationException($"Invalid page type ({Enum.GetName(typeof(PAGE_LINK_TYPES), type)}).");
            }
        }

        /// <inheritdoc/>
        public SiteSettingsModel GetSiteSettings()
        {
            var settings = this.Context.GetItem<SiteSettingsModel>(this.ContextWrapper.GetSiteRoot());
            return settings ?? throw new MissingDatasourceException("Site settings could not be resolved.");
        }

        /// <inheritdoc/>
        public ProcessStepModel[] GetSteps(ProcessStepModel currentStep)
        {
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));
            var items = this.Context.GetItems<ProcessStepModel>(parentPath);

            foreach (var item in items)
            {
                if (item.ID == currentStep.ID)
                {
                    item.IsSelected = true;
                }
            }

            return items.ToArray();
        }
    }
}
