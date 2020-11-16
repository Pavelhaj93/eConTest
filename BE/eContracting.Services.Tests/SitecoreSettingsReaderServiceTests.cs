using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Maps;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Fields;
using Moq;
using Moq.Language.Flow;
using Sitecore.Web;
using Xunit;

namespace eContracting.Services.Tests
{
    [ExcludeFromCodeCoverage]
    public class SitecoreSettingsReaderServiceTests
    {
        public SitecoreSettingsReaderServiceTests()
        {
            var config = new Config();
            var dependencyResolver = new Glass.Mapper.Sc.IoC.DependencyResolver(config);
            var context = Glass.Mapper.Context.Create(dependencyResolver);
            var configurationMap = new ConfigurationMap(dependencyResolver);
            var configurationLoader = configurationMap.GetConfigurationLoader<SitecoreFluentConfigurationLoader>();
            context.Load(configurationLoader);
        }

        [Fact]
        public void GetSiteSettings_Throws_MissingDatasourceException_When_Not_Found()
        {
            var siteRoot = "/site";
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns((SiteSettingsModel)null);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);

            Assert.Throws<MissingDatasourceException>(() => service.GetSiteSettings());
        }

        [Fact]
        public void GetAllLoginTypes_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<LoginTypeModel>());
            var mockContextWrapper = new Mock<IContextWrapper>();

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            Assert.Throws<MissingDatasourceException>(() => service.GetAllLoginTypes());
        }

        [Fact]
        public void GetAllProcesses_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<LoginTypeModel>());
            var mockContextWrapper = new Mock<IContextWrapper>();

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            Assert.Throws<MissingDatasourceException>(() => service.GetAllProcesses());
        }

        [Fact]
        public void GetAllProcessTypes_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<LoginTypeModel>());
            var mockContextWrapper = new Mock<IContextWrapper>();

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            Assert.Throws<MissingDatasourceException>(() => service.GetAllProcessTypes());
        }

        [Fact]
        public void GetApiServiceOptions_Takes_Only_Url_From_GetSetting()
        {
            var url = "http://sap.cz";
            string user = "joe";
            string password = "secret";

            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.ServiceUrl = "";
            siteSettings.ServiceUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(user));
            siteSettings.ServicePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(url);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        public void GetApiServiceOptions_Takes_Only_Username_From_GetSetting()
        {
            string user = "joe";
            string password = "secret";

            var url = "http://sap.cz";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.ServiceUrl = url;
            siteSettings.ServiceUser = "";
            siteSettings.ServicePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(user)));
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        public void GetApiServiceOptions_Takes_Only_Password_From_GetSetting()
        {
            string user = "joe";
            string password = "secret";

            var url = "http://sap.cz";

            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.ServiceUrl = url;
            siteSettings.ServiceUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(user));
            siteSettings.ServicePassword = "";

            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        public void GetApiServiceOptions_Get_Data_From_GetSetting()
        {
            var url = "http://sap.cz";
            string user = "joe";
            string password = "secret";

            var siteSettings = new SiteSettingsModel();

            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>("/sitecore/content", false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns("/sitecore/content");
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(url);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(user)));
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        public void GetApiServiceOptions_Get_Data_From_GeneralSettings()
        {
            var url = "http://sap.cz";
            string user = "joe";
            string password = "secret";

            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.ServiceUrl = url;
            siteSettings.ServiceUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(user));
            siteSettings.ServicePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        public void GetPageLink_Returns_Login_Url()
        {
            var expected = "/login";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.Login = new Link();
            siteSettings.Login.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.Login);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_Offer_Url()
        {
            var expected = "/offer";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.Offer = new Link();
            siteSettings.Offer.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.Offer);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_OfferExpired_Url()
        {
            var expected = "/expired-offer";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.OfferExpired = new Link();
            siteSettings.OfferExpired.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.OfferExpired);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_AcceptedOffer_Url()
        {
            var expected = "/accepted-offer";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.AcceptedOffer = new Link();
            siteSettings.AcceptedOffer.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.AcceptedOffer);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_SessionExpired_Url()
        {
            var expected = "/session-expired";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.SessionExpired = new Link();
            siteSettings.SessionExpired.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.SessionExpired);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_SystemError_Url()
        {
            var expected = "/system-error";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.SystemError = new Link();
            siteSettings.SystemError.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.SystemError);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_ThankYou_Url()
        {
            var expected = "/thank-you";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.ThankYou = new Link();
            siteSettings.ThankYou.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.ThankYou);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_UserBlocked_Url()
        {
            var expected = "/user-blocked";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.UserBlocked = new Link();
            siteSettings.UserBlocked.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.UserBlocked);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPageLink_Returns_WrongUrl_Url()
        {
            var expected = "/error";
            var siteRoot = "/site";
            var siteSettings = new SiteSettingsModel();
            siteSettings.WrongUrl = new Link();
            siteSettings.WrongUrl.Url = expected;
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItem<SiteSettingsModel>(siteRoot, false, false)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object, mockContextWrapper.Object);
            var result = service.GetPageLink(PAGE_LINK_TYPES.WrongUrl);

            Assert.Equal(expected, result);
        }
    }
}
