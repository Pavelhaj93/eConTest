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
using Glass.Mapper.Sc.Web.Mvc;
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
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_CanLogin_USER_BLOCKED()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var eventLogger = new MemoryEventLogger();
            var datasource = new Mock<MemoryPageLoginModel>().Object;
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            //mockSessionProvider.Setup(x => x.GetId()).Returns("b83235e2ab544a53b52950325327adbf");
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(false);
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LOGIN_STATES.USER_BLOCKED, result);
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_CanLogin_OFFER_NOT_FOUND()
        {
            var guid = Guid.NewGuid().ToString("N");

            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.IsAbleToLogin(guid, (OfferModel)null, datasource);

            Assert.Equal(LOGIN_STATES.OFFER_NOT_FOUND, result);
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_CanLogin_OFFER_STATE_1()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "1";
            var offer = this.CreateOffer(guid, true, 2, state);
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LOGIN_STATES.OFFER_STATE_1, result);
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_CanLogin_MISSING_BIRTHDAY()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "3";
            var offer = this.CreateOffer(guid, true, 2, state);
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LOGIN_STATES.MISSING_BIRTHDAY, result);
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_CanLogin_OK()
        {
            var guid = Guid.NewGuid().ToString("N");
            var state = "3";
            var offer = this.CreateOffer(guid, true, 2, state);
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.IsAbleToLogin(guid, offer, datasource);

            Assert.Equal(LOGIN_STATES.OK, result);
        }

        //[Fact]
        public void Login_Get_Returns_PreviewView_When_In_Preview_Mode()
        {
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var userCacheData = new UserCacheDataModel();
            var offerCacheData = new OfferCacheDataModel(this.CreateOffer());
            var definition = new MemoryDefinitionCombinationModel();
            definition.Process = new MemoryProcessModel() { Code = offerCacheData.Process };
            definition.ProcessType = new MemoryProcessTypeModel() { Code = offerCacheData.ProcessType };
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(false);
            mockContextWrapper.Setup(x => x.IsEditMode()).Returns(false);
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(userCacheData);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetDefinition(offerCacheData.Process, offerCacheData.ProcessType)).Returns(definition);
            mockSettingsReader.Setup(x => x.GetAllLoginTypes()).Returns(new MemoryLoginTypeModel[] { });
            mockSettingsReader.Setup(x => x.GetSteps(loginPageModel.Step)).Returns(new MemoryProcessStepModel[] { });
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", "http://localhost", "");
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<ViewResult>(result);
                var actionResult = (ViewResult)result;

                Assert.Equal("/Areas/eContracting2/Views/Preview/Login.cshtml", actionResult.ViewName);
            }
        }

        //[Fact]
        public void Login_Get_Returns_EditView_When_In_Editing_Mode()
        {
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var userCacheData = new UserCacheDataModel();
            var offerCacheData = new OfferCacheDataModel(this.CreateOffer());
            var definition = new MemoryDefinitionCombinationModel();
            definition.Process = new MemoryProcessModel() { Code = offerCacheData.Process };
            definition.ProcessType = new MemoryProcessTypeModel() { Code = offerCacheData.ProcessType };
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(false);
            mockContextWrapper.Setup(x => x.IsEditMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(userCacheData);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetDefinition(offerCacheData.Process, offerCacheData.ProcessType)).Returns(definition);
            mockSettingsReader.Setup(x => x.GetAllLoginTypes()).Returns(new MemoryLoginTypeModel[] { });
            mockSettingsReader.Setup(x => x.GetSteps(loginPageModel.Step)).Returns(new MemoryProcessStepModel[] { });
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", "http://localhost", "");
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<ViewResult>(result);
                var actionResult = (ViewResult)result;

                Assert.Equal("/Areas/eContracting2/Views/Edit/Login.cshtml", actionResult.ViewName);
            }
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_Redirect_When_Missing_Guid()
        {
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "";
            var redirectUrl = "http://localhost/error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.INVALID_GUID;

            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl, (string)null)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<MemoryPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
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
            var guid = this.CreateGuid();
            var requestQuery = "guid=" + guid;
            var requestUrl = "http://localhost/login?" + requestQuery;
            var redirectUrl = "http://localhost/error";
            var expected = redirectUrl + "?code=" + Constants.ErrorCodes.OFFER_NOT_FOUND;
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);

            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns((OfferModel)null);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl, guid)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, loginPageModel.MaxFailedAttempts, loginPageModel.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(loginPageModel);
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_Redirect_When_User_Blocked()
        {
            var offer = this.CreateOffer();
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + offer.Guid;
            var redirectUrl = $"http://localhost/blocked?{requestUrlQuery}";
            var user = this.CreateAnonymousUser(offer);
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(offer.Guid)).Returns(offer);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.UserBlocked, offer.Guid)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(offer.Guid, loginPageModel.MaxFailedAttempts, loginPageModel.GetDelayAfterFailedAttemptsTimeSpan())).Returns(false);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(loginPageModel);
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(redirectUrl, actionResult.Url);
            }
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_Redirect_Offer_State_1()
        {
            var state = "1";
            var offer = this.CreateOffer(true, 2, state);
            var user = this.CreateAnonymousUser(offer);
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + offer.Guid;
            var redirectUrl = $"http://localhost/error?{requestUrlQuery}";
            var expected = $"{redirectUrl}&code={Constants.ErrorCodes.OFFER_STATE_1}";
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(offer.Guid)).Returns(offer);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl, offer.Guid)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(offer.Guid, loginPageModel.MaxFailedAttempts, loginPageModel.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(loginPageModel);
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_Redirect_Missing_Birthdate()
        {
            var state = "2";
            var offer = this.CreateOffer(true, 2, state);
            var user = this.CreateAnonymousUser(offer);
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + offer.Guid;
            var redirectUrl = $"http://localhost/error?{requestUrlQuery}";
            var expected = $"{redirectUrl}&code={Constants.ErrorCodes.MISSING_BIRTDATE}";
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(offer.Guid)).Returns(offer);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.WrongUrl, offer.Guid)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(offer.Guid, loginPageModel.MaxFailedAttempts, loginPageModel.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_ReadOffer_When_State_3()
        {
            bool wasRead = false;
            var guid = Guid.NewGuid().ToString("N");
            var state = "3";
            var offer = this.CreateOffer(guid, true, 2, state);
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var user = this.CreateAnonymousUser(offer);
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            mockApiService.Setup(x => x.ReadOffer(guid, user)).Callback(() =>
            {
                wasRead = true;
            });
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetLoginTypes(offer)).Returns(new MemoryLoginTypeModel[] { new MemoryLoginTypeModel() });
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
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
        [Trait("eContracting2AuthController", "Login")]
        public void Login_Get_Succeeded()
        {
            var guid = this.CreateGuid();
            var state = "4";
            var offer = this.CreateOffer(guid, true, 2, state);
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var user = this.CreateAnonymousUser(offer);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetLoginTypes(offer)).Returns(new MemoryLoginTypeModel[] { new MemoryLoginTypeModel() });
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
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
        public void Login_Hide_Innogy_Account_Login_Box_When_Offer_Doesnt_Have_MCFU()
        {
            var guid = this.CreateGuid();
            var state = "4";
            var offer = this.CreateOffer(guid, true, 2, state);
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var user = this.CreateAnonymousUser(offer);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            mockApiService.Setup(x => x.CanReadOffer(guid, user, OFFER_TYPES.NABIDKA)).Returns(true);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetLoginTypes(offer)).Returns(new MemoryLoginTypeModel[] { new MemoryLoginTypeModel() });
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();
                var actionResult = (ViewResult)result;
                var loginViewModel = actionResult.Model as LoginViewModel;

                Assert.True(loginViewModel.HideInnogyAccount);
            }
        }

        [Fact]
        public void Login_Show_Innogy_Account_Login_Box_When_Offer_Have_MCFU()
        {
            var guid = this.CreateGuid();
            var state = "4";
            var attributes = new List<OfferAttributeModel>();
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.MCFU_REG_STAT, "12345"));
            var offer = this.CreateOffer(guid, false, 2, state, DateTime.Now.AddDays(1).ToString("dd.mm.yyyy"), attributes.ToArray());
            offer.Xml.Content.Body.BIRTHDT = "27.10.2020";
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var datasource = new MemoryPageLoginModel();
            var user = this.CreateAnonymousUser(offer);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(offer);
            mockApiService.Setup(x => x.CanReadOffer(guid, user, OFFER_TYPES.NABIDKA)).Returns(true);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetLoginTypes(offer)).Returns(new MemoryLoginTypeModel[] { new MemoryLoginTypeModel() });
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            mockLoginReportService.Setup(x => x.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan())).Returns(true);
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Login();
                var actionResult = (ViewResult)result;
                var loginViewModel = actionResult.Model as LoginViewModel;

                Assert.False(loginViewModel.HideInnogyAccount);
            }
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Loging_Get_Throws_AggregateException_With_EndpointNotFoundException_When_Call_GetOffer()
        {
            var guid = this.CreateGuid();
            var requestQuery = "guid=" + guid;
            var requestUrl = $"http://localhost/login?{requestQuery}";
            var redirectUrl = $"http://localhost/system-error?{requestQuery}";
            var expected = $"{redirectUrl}&code={Constants.ErrorCodes.AUTH1_CACHE}";

            var aggregageException = new AggregateException(new EndpointNotFoundException());

            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(guid)).Returns(() => { throw aggregageException; });
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.SystemError, guid)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;

                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Fact]
        [Trait("eContracting2AuthController", "Login")]
        public void Loging_Get_Throws_AggregateException_When_Call_GetOffer()
        {
            var offer = this.CreateOffer();
            var requestQuery = "guid=" + offer.Guid;
            var requestUrl = $"http://localhost/login?{requestQuery}";
            var redirectUrl = $"http://localhost/system-error?{requestQuery}";
            var expected = $"{redirectUrl}&code=" + Constants.ErrorCodes.AUTH1_CACHE2;
            var user = this.CreateAnonymousUser(offer);
            var aggregageException = new AggregateException();

            var loginPageModel = new MemoryPageLoginModel();
            loginPageModel.Step = new MemoryProcessStepModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockApiService = new Mock<IOfferService>();
            mockApiService.Setup(x => x.GetOffer(offer.Guid)).Returns(() => { throw aggregageException; });
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUser()).Returns(user);
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(PAGE_LINK_TYPES.SystemError, offer.Guid)).Returns(redirectUrl);
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetContextItem<IPageLoginModel>()).Returns(new Mock<IPageLoginModel>().Object);
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;

                var result = controller.Login();

                Assert.IsType<RedirectResult>(result);
                var actionResult = (RedirectResult)result;

                Assert.Equal(expected, actionResult.Url);
            }
        }

        [Theory]
        [Trait("eContracting2AuthController", "Login")]
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

            var loginType = new MemoryLoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            mockMvcContext.Setup(x => x.GetPageContextItem<IPageLoginModel>()).Returns(new MemoryPageLoginModel());
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl + "?" + requestUrlQuery, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
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
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);

            Assert.Throws<ArgumentNullException>(() => { controller.GetChoiceViewModel((MemoryLoginTypeModel)null, offer); });
        }

        [Fact]
        public void GetChoiceViewModel_Throws_ArgumentNullException_When_Offer_Null()
        {
            var loginType = new MemoryLoginTypeModel();

            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            Assert.Throws<ArgumentNullException>(() => { controller.GetChoiceViewModel(loginType, (OfferModel)null); });
        }

        [Fact]
        public void GetChoiceViewModel_Success()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var loginType = new MemoryLoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.GetChoiceViewModel(loginType, offer);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetChoiceViewModel_With_Unique_Key()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var loginType = new MemoryLoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.GetChoiceViewModel(loginType, offer);

            Assert.NotNull(result);
            Assert.True(!string.IsNullOrEmpty(result.Key));
        }

        [Fact]
        public void GetChoiceViewModel_With_Corrent_Key()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var loginType = new MemoryLoginTypeModel();
            var logger = new MemoryLogger();
            var eventLogger = new MemoryEventLogger();
            var textService = new MemoryTextService();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockApiService = new Mock<IOfferService>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockLoginReportService = new Mock<ILoginFailedAttemptBlockerStore>();
            var mockMvcContext = new Mock<IMvcContext>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();

            var controller = new eContracting2AuthController(
                    logger,
                    mockContextWrapper.Object,
                    mockApiService.Object,
                    mockSessionProvider.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockLoginReportService.Object,
                    mockMvcContext.Object,
                    eventLogger,
                    textService,
                    mockRequestCacheService.Object);
            var result = controller.GetChoiceViewModel(loginType, offer);

            Assert.NotNull(result);

            var key = Utils.GetUniqueKey(loginType, offer);
            Assert.Equal(key, result.Key);
        }
    }
}
