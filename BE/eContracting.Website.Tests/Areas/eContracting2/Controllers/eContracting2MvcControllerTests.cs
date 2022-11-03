using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eContracting.Models;
using eContracting.Tests;
using eContracting.Website.Areas.eContracting2.Controllers;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc.Web.Mvc;
using Moq;
using Xunit;

namespace eContracting.Website.Tests.Areas.eContracting2.Controllers
{
    public class eContracting2MvcControllerTests : BaseTestClass
    {
        public class eContracting2MvcControllerImpl : eContracting2MvcController
        {
            public eContracting2MvcControllerImpl(
                ILogger logger,
                IContextWrapper contextWrapper,
                IUserService userService,
                ISettingsReaderService settingsReader,
                ISessionProvider sessionProvider,
                IDataRequestCacheService requestCacheService,
                IMvcContext mvcContext) : base(logger, contextWrapper, userService, settingsReader, sessionProvider, requestCacheService, mvcContext)
            {
            }
        }

        [Fact]
        public void GetGuid_Returns_Guid_From_Request()
        {
            var guid = this.CreateGuid();
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var logger = new MemoryLogger();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockMvcContext = new Mock<IMvcContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("/login?guid=" + guid, requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2MvcControllerImpl(
                    logger,
                    mockContextWrapper.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockSessionProvider.Object,
                    mockRequestCacheService.Object,
                    mockMvcContext.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.GetGuid();

                Assert.Equal(guid, result);
            }
        }

        [Fact]
        public void CanRead_Checks_If_User_Authorized_For_Guid()
        {
            var methodCalled = false;
            var offer = this.CreateOffer();
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + offer.Guid;
            var logger = new MemoryLogger();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.IsAuthorizedFor(offer.Guid)).Returns(true).Callback(() => { methodCalled = true; });
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockMvcContext = new Mock<IMvcContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2MvcControllerImpl(
                    logger,
                    mockContextWrapper.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockSessionProvider.Object,
                    mockRequestCacheService.Object,
                    mockMvcContext.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                controller.CanRead(offer.Guid);

                Assert.True(methodCalled);
            }
        }

        [Fact]
        public void Redirect_Returns_Url_With_Code()
        {
            var guid = this.CreateGuid();
            var redirectUrl = $"http://localhost/login?guid={guid}";
            var pageType = PAGE_LINK_TYPES.Login;
            var code = "XYZ";
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + guid;
            var logger = new MemoryLogger();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(pageType, guid)).Returns(redirectUrl);
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockMvcContext = new Mock<IMvcContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2MvcControllerImpl(
                    logger,
                    mockContextWrapper.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockSessionProvider.Object,
                    mockRequestCacheService.Object,
                    mockMvcContext.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Redirect(pageType, guid, code);

                Assert.Equal($"http://localhost/login?guid={guid}&code={code}", result.Url);
            }
        }

        [Fact]
        public void Redirect_Returns_Url_With_GA_Query_Parameters_From_Current_Request()
        {
            var utmQuery = "utm_source=Runway&utm_medium=Tlacitko-Rozhodovaci-stranka&utm_campaign=Stavajici_produkt_ZP-retence";
            var guid = this.CreateGuid();
            var redirectUrl = $"http://localhost/login?guid={guid}";
            var pageType = PAGE_LINK_TYPES.Login;
            var requestUrl = "http://localhost/login";
            var requestUrlQuery = $"guid={guid}&{utmQuery}";
            var logger = new MemoryLogger();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.IsNormalMode()).Returns(true);
            mockContextWrapper.Setup(x => x.GetQueryParams()).Returns(HttpUtility.ParseQueryString(requestUrlQuery));
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.Setup(x => x.GetPageLink(pageType, guid)).Returns(redirectUrl);
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockMvcContext = new Mock<IMvcContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2MvcControllerImpl(
                    logger,
                    mockContextWrapper.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockSessionProvider.Object,
                    mockRequestCacheService.Object,
                    mockMvcContext.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.Redirect(pageType, guid, null, true);

                Assert.Equal($"http://localhost/login?guid={guid}&{utmQuery}", result.Url);
            }
        }

        [Fact]
        public void GetProductName_Returns_Commodity_Product()
        {
            var offer = this.CreateOffer();
            offer.TextParameters.Add("COMMODITY_PRODUCT", "OPTIMAL2_COMMODITY_PRODUCT");

            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + offer.Guid;
            var logger = new MemoryLogger();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockMvcContext = new Mock<IMvcContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2MvcControllerImpl(
                    logger,
                    mockContextWrapper.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockSessionProvider.Object,
                    mockRequestCacheService.Object,
                    mockMvcContext.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.GetProductName(offer);

                Assert.Equal("OPTIMAL2_COMMODITY_PRODUCT", result);
            }
        }

        [Fact]
        public void GetProductName_Returns_NonCommodity_Product_When_Commodity_Product_Missing()
        {
            var offer = this.CreateOffer();
            offer.TextParameters.Add("NONCOMMODITY_PRODUCT", "OPTIMAL2_NONCOMMODITY_PRODUCT");

            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + offer.Guid;
            var logger = new MemoryLogger();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockMvcContext = new Mock<IMvcContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2MvcControllerImpl(
                    logger,
                    mockContextWrapper.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockSessionProvider.Object,
                    mockRequestCacheService.Object,
                    mockMvcContext.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.GetProductName(offer);

                Assert.Equal("OPTIMAL2_NONCOMMODITY_PRODUCT", result);
            }
        }

        [Fact]
        public void GetProductName_Returns_UNKNOWN_When_Product_Not_Found()
        {
            var offer = this.CreateOffer();

            var requestUrl = "http://localhost/login";
            var requestUrlQuery = "guid=" + offer.Guid;
            var logger = new MemoryLogger();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var mockSessionProvider = new Mock<ISessionProvider>();
            var mockUserService = new Mock<IUserService>();
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            var mockRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockMvcContext = new Mock<IMvcContext>();

            using (var writter = new StringWriter())
            {
                var httpRequest = new HttpRequest("", requestUrl, requestUrlQuery);
                var httpResponse = new HttpResponse(writter);
                var httpContext = new HttpContext(httpRequest, httpResponse);
                var httpContextWrapper = new HttpContextWrapper(httpContext);

                var controller = new eContracting2MvcControllerImpl(
                    logger,
                    mockContextWrapper.Object,
                    mockUserService.Object,
                    mockSettingsReader.Object,
                    mockSessionProvider.Object,
                    mockRequestCacheService.Object,
                    mockMvcContext.Object);
                controller.ControllerContext = new ControllerContext();
                controller.ControllerContext.HttpContext = httpContextWrapper;
                var result = controller.GetProductName(offer);

                Assert.Equal("UNKNOWN", result);
            }
        }
    }
}
