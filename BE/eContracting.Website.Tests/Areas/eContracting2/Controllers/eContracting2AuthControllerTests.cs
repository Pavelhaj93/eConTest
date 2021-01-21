using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eContracting.Models;
using eContracting.Tests;
using eContracting.Website.Areas.eContracting2.Controllers;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Maps;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Web;
using Moq;
using Xunit;

namespace eContracting.Website.Tests.Areas.eContracting.Controllers
{
    [ExcludeFromCodeCoverage]
    public class eContracting2AuthControllerTests : BaseTestClass
    {
        public eContracting2AuthControllerTests()
        {
            var config = new Config();
            var dependencyResolver = new Glass.Mapper.Sc.IoC.DependencyResolver(config);
            var context = Glass.Mapper.Context.Create(dependencyResolver);

            //if (dependencyResolver.ConfigurationMapFactory is ConfigurationMapConfigFactory)
            //{
            //    GlassMapperScCustom.AddMaps(dependencyResolver.ConfigurationMapFactory);
            //}

            var configurationMap = new ConfigurationMap(dependencyResolver);
            var configurationLoader = configurationMap.GetConfigurationLoader<SitecoreFluentConfigurationLoader>();
            context.Load(configurationLoader);
        }

        [Fact]
        public void Login_Get_CanLogin_USER_BLOCKED()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var eventLogger = new MemoryEventLogger();
            var datasource = new PageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            //mockSessionProvider.Setup(x => x.GetId()).Returns("b83235e2ab544a53b52950325327adbf");
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, datasource.MaxFailedAttempts, datasource.DelayAfterFailedAttemptsTimeSpan)).Returns(false);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();
            
            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LoginStates.USER_BLOCKED, result);
        }

        [Fact]
        public void Login_Get_CanLogin_OFFER_NOT_FOUND()
        {
            var guid = Guid.NewGuid().ToString("N");

            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new PageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, datasource.MaxFailedAttempts, datasource.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.IsAbleToLogin(guid, (OfferModel)null, datasource);

            Assert.Equal(LoginStates.OFFER_NOT_FOUND, result);
        }

        [Fact]
        public void Login_Get_CanLogin_OFFER_STATE_1()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "1";
            var offer = this.CreateOffer(guid, true, 2, state);
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new PageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, datasource.MaxFailedAttempts, datasource.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LoginStates.OFFER_STATE_1, result);
        }

        [Fact]
        public void Login_Get_CanLogin_MISSING_BIRTHDAY()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "3";
            var offer = this.CreateOffer(guid, true, 2, state);
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new PageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, datasource.MaxFailedAttempts, datasource.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LoginStates.MISSING_BIRTHDAY, result);
        }

        [Fact]
        public void Login_Get_CanLogin_OK()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "3";
            var offer = this.CreateOffer(guid, true, 2, state);
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new PageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, datasource.MaxFailedAttempts, datasource.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LoginStates.OK, result);
        }

        [Fact]
        public void Login_Get_Returns_PreviewView_When_In_Preview_Mode()
        {
            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(false);
            mockContextWrapper.Setup(x => x.IsEditMode()).Returns(false);
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetAllLoginTypes()).Returns(new LoginTypeModel[] { });
            mockSettingsReader.Setup(x => x.GetSteps(loginPageModel.Step)).Returns(new ProcessStepModel[] { });
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(new PageLoginModel());
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", "http://localhost", "");
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<ViewResult>(result);
                var actionResult = (ViewResult)result;

                Assert.Equal("/Areas/eContracting2/Views/Preview/Login.cshtml", actionResult.ViewName);
            }
        }

        [Fact]
        public void Login_Get_Returns_EditView_When_In_Editing_Mode()
        {
            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(false);
            mockContextWrapper.Setup(x => x.IsEditMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetAllLoginTypes()).Returns(new LoginTypeModel[] { });
            mockSettingsReader.Setup(x => x.GetSteps(loginPageModel.Step)).Returns(new ProcessStepModel[] { });
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(new PageLoginModel());
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", "http://localhost", "");
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<ViewResult>(result);
                var actionResult = (ViewResult)result;

                Assert.Equal("/Areas/eContracting2/Views/Edit/Login.cshtml", actionResult.ViewName);
            }
        }

        [Fact]
        public void Login_Get_Redirect_When_Missing_Guid()
        {
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "";
            var redirectUrl = "http://localhost/error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.INVALID_GUID;

            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(new PageLoginModel());
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        public void Login_Get_Redirect_When_Offer_Not_Found()
        {
            var guid = Guid.NewGuid().ToString("N");
            var requestQuery = "guid=" + guid;
            var requestUrl = "http://localhost/login?" + requestQuery;
            var redirectUrl = "http://localhost/error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.OFFER_NOT_FOUND;

            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns((OfferModel)null);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, loginPageModel.MaxFailedAttempts, loginPageModel.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(loginPageModel);
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        public void Login_Get_Redirect_When_User_Blocked()
        {
            var guid = Guid.NewGuid().ToString("N");
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var redirectUrl = "http://localhost/blocked";

            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns((OfferModel)null);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.UserBlocked)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, loginPageModel.MaxFailedAttempts, loginPageModel.DelayAfterFailedAttemptsTimeSpan)).Returns(false);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(loginPageModel);
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(redirectUrl, actionResult.Url);
            }
        }

        [Fact]
        public void Login_Get_Redirect_Offer_State_1()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "1";
            var offer = this.CreateOffer(guid, true, 2, state);
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var redirectUrl = "http://localhost/error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.OFFER_STATE_1;
            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, loginPageModel.MaxFailedAttempts, loginPageModel.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(loginPageModel);
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        public void Login_Get_Redirect_Missing_Birthdate()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "2";
            var offer = this.CreateOffer(guid, true, 2, state);
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var redirectUrl = "http://localhost/error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.MISSING_BIRTDATE;
            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, loginPageModel.MaxFailedAttempts, loginPageModel.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(new PageLoginModel());
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        public void Login_Get_ReadOffer_When_State_3()
        {
            bool wasRead = false;
            var guid = Guid.NewGuid().ToString("N");
            var state = "3";
            var offer = this.CreateOffer(guid, true, 2, state);
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new PageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            mockApiService.Setup(x => x.ReadOffer(guid)).Returns(() =>
            {
                wasRead = true;
                return true;
            });
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetLoginTypes(offer)).Returns(new LoginTypeModel[] { new LoginTypeModel() });
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, datasource.MaxFailedAttempts, datasource.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(new PageLoginModel());
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.True(wasRead);

                Assert.IsType<ViewResult>(result);
                var actionResult = (ViewResult)result;
                Assert.Equal("/Areas/eContracting2/Views/Login.cshtml", actionResult.ViewName);
                Assert.IsType<LoginViewModel>(actionResult.Model);
                var loginViewModel = actionResult.Model as LoginViewModel;
            }
        }

        [Fact]
        public void Login_Get_Succeeded()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "4";
            var offer = this.CreateOffer(guid, true, 2, state);
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new PageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            mockApiService.Setup(x => x.ReadOffer(guid)).Returns(true);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetLoginTypes(offer)).Returns(new LoginTypeModel[] { new LoginTypeModel() });
            var mockLoginReportService = new Mock<ILoginReportStore>();
            mockLoginReportService.Setup(x => x.CanLogin(guid, datasource.MaxFailedAttempts, datasource.DelayAfterFailedAttemptsTimeSpan)).Returns(true);
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(new PageLoginModel());
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<ViewResult>(result);
                var actionResult = (ViewResult)result;
                Assert.Equal("/Areas/eContracting2/Views/Login.cshtml", actionResult.ViewName);
                Assert.IsType<LoginViewModel>(actionResult.Model);
                var loginViewModel = actionResult.Model as LoginViewModel;
            }
        }

        [Fact]
        public void Loging_Get_Throws_AggregateException_With_EndpointNotFoundException_When_Call_GetOffer()
        {
            var guid = Guid.NewGuid().ToString("N");
            var requestQuery = "guid=" + guid;
            var requestUrl = "http://localhost/login?" + requestQuery;
            var redirectUrl = "http://localhost/system-error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.AUTH1_CACHE;

            var aggregageException = new AggregateException(new EndpointNotFoundException());

            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(() => { throw aggregageException; });
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.SystemError)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;

                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        public void Loging_Get_Throws_AggregateException_When_Call_GetOffer()
        {
            var guid = Guid.NewGuid().ToString("N");
            var requestQuery = "guid=" + guid;
            var requestUrl = "http://localhost/login?" + requestQuery;
            var redirectUrl = "http://localhost/system-error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.AUTH1_CACHE2;

            var aggregageException = new AggregateException();

            var loginPageModel = new PageLoginModel();
            loginPageModel.Step = new ProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(() => { throw aggregageException; });
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.SystemError)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;

                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Theory]
        [InlineData(AUTH_RESULT_STATES.INVALID_BIRTHDATE, Constants.ValidationCodes.INVALID_BIRTHDATE)]
        [InlineData(AUTH_RESULT_STATES.INVALID_VALUE, Constants.ValidationCodes.INVALID_VALUE)]
        [InlineData(AUTH_RESULT_STATES.INVALID_BIRTHDATE_AND_VALUE, Constants.ValidationCodes.INVALID_BIRTHDATE_AND_VALUE)]
        [InlineData(AUTH_RESULT_STATES.INVALID_BIRTHDATE_DEFINITION, Constants.ValidationCodes.INVALID_BIRTHDATE_DEFINITION)]
        [InlineData(AUTH_RESULT_STATES.INVALID_VALUE_DEFINITION, Constants.ValidationCodes.INVALID_VALUE_DEFINITION)]
        [InlineData(AUTH_RESULT_STATES.KEY_MISMATCH, Constants.ValidationCodes.KEY_MISMATCH)]
        [InlineData(AUTH_RESULT_STATES.KEY_VALUE_MISMATCH, Constants.ValidationCodes.KEY_VALUE_MISMATCH)]
        [InlineData(AUTH_RESULT_STATES.SUCCEEDED, Constants.ValidationCodes.UNKNOWN)]
        public void GetLoginFailReturns_Returns_Correct_Code(AUTH_RESULT_STATES state, string errorcode)
        {
            var guid = Guid.NewGuid().ToString("N");

            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;

            var loginType = new LoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            mockSitecoreContext.Setup(x => x.GetCurrentItem<PageLoginModel>(false, false)).Returns(new PageLoginModel());
            var mockRenderingContext = new Mock<IRenderingContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl + "?" + requestUrlQuery, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.GetLoginFailReturns(state, loginType, guid);

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                var resultUri = new Uri(actionResult.Url);

                Assert.StartsWith(requestUrl, actionResult.Url);

                var query = HttpUtility.ParseQueryString(resultUri.Query);

                Assert.Contains("guid", query.AllKeys);
                Assert.Equal(guid, query["guid"]);
                Assert.Contains("error", query.AllKeys);
                Assert.Equal(errorcode, query["error"]);
            }
        }

        [Fact]
        public void GetChoiceViewModel_Throws_ArgumentNullException_When_Model_Null()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);

            Assert.Throws<ArgumentNullException>(() => { controller.GetChoiceViewModel((LoginTypeModel)null, offer); });
        }

        [Fact]
        public void GetChoiceViewModel_Throws_ArgumentNullException_When_Offer_Null()
        {
            var loginType = new LoginTypeModel();

            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            Assert.Throws<ArgumentNullException>(() => { controller.GetChoiceViewModel(loginType, (OfferModel)null); });
        }

        [Fact]
        public void GetChoiceViewModel_Success()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var loginType = new LoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.GetChoiceViewModel(loginType, offer);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetChoiceViewModel_With_Unique_Key()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var loginType = new LoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.GetChoiceViewModel(loginType, offer);

            Assert.NotNull(result);
            Assert.True(!string.IsNullOrEmpty(result.Key));
        }

        [Fact]
        public void GetChoiceViewModel_With_Corrent_Key()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var loginType = new LoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockUserDataCache = new Mock<IUserDataCacheService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginReportStore>();
            var mockSitecoreContext = new Mock<ISitecoreContext>();
            var mockRenderingContext = new Mock<IRenderingContext>();

            var controller = new eContracting2AuthController(logger, mockContextWrapper.Object, mockApiService.Object, mockSessionProvider.Object, mockAuthService.Object, mockSettingsReader.Object, mockLoginReportService.Object, mockSitecoreContext.Object, mockRenderingContext.Object, mockUserDataCache.Object, eventLogger, textService);
            var result = controller.GetChoiceViewModel(loginType, offer);

            Assert.NotNull(result);

            var key = Utils.GetUniqueKey(loginType, offer);
            Assert.Equal(key, result.Key);
        }
    }
}
