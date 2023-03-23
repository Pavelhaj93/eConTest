using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;
using eContracting.Tests;
using Moq;
using Xunit;

namespace eContracting.Services.Tests
{
    public class UserServiceTests : BaseTestClass
    {
        [Fact]
        public void Abandon_Removes_User_From_Cache()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);
            var removeCalled = false;
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA)).Returns(user);
            mockCacheService.Setup(x => x.Remove(Constants.CacheKeys.USER_DATA)).Callback(() => { removeCalled = true; });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            service.Abandon(offer.Guid);

            Assert.True(removeCalled);
        }

        [Fact]
        public void GetUser_Always_Returns_User_Even_Does_Not_Exists_In_Cache()
        {
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA)).Returns((UserCacheDataModel)null);
            mockCacheService.Setup(x => x.Set(Constants.CacheKeys.USER_DATA, It.IsAny<UserCacheDataModel>()));
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = service.GetUser();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUser_Saves_New_User_When_Created()
        {
            var setCalled = false;
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA)).Returns((UserCacheDataModel)null);
            mockCacheService.Setup(x => x.Set(Constants.CacheKeys.USER_DATA, It.IsAny<UserCacheDataModel>())).Callback(() => { setCalled = true; });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = service.GetUser();

            Assert.True(setCalled);
        }

        [Fact]
        public void IsAuthorized_Returns_True_When_User_Is_Cognito()
        {
            var offer = this.CreateOffer();
            var user = this.CreateCognitoUser(offer);
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA)).Returns(user);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = service.IsAuthorized();

            Assert.True(result);
        }

        [Fact]
        public void IsAuthorized_Returns_True_When_User_Not_Cognito_And_Has_Authorized_Guids()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);
            user.SetAuth(offer.Guid, AUTH_METHODS.TWO_SECRETS);
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA)).Returns(user);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = service.IsAuthorized();

            Assert.True(result);
        }

        [Fact]
        public void IsAuthorizedFor_Returns_False_When_Guid_Empty()
        {
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            var result = service.IsAuthorizedFor("");

            Assert.False(result);
        }

        [Fact]
        public void IsAuthorizedFor_Returns_False_When_Guid_Not_Authorized()
        {
            var guid = this.CreateGuid();
            var user = this.CreateAnonymousUser(this.CreateOffer());
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA)).Returns(user);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            var result = service.IsAuthorizedFor(guid);

            Assert.False(result);
        }

        [Fact]
        public void IsAuthorizedFor_Returns_True_When_Guid_Authorized()
        {
            var guid = this.CreateGuid();
            var user = this.CreateAnonymousUser(this.CreateOffer());
            user.SetAuth(guid, AUTH_METHODS.TWO_SECRETS);
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA)).Returns(user);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            var result = service.IsAuthorizedFor(guid);

            Assert.True(result);
        }

        [Fact]
        public void IsUserValid_Returns_True_When_Not_Cognito()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            var result = service.IsUserValid(offer.Guid, user);

            Assert.True(result);
        }

        [Fact]
        public void IsUserValid_Returns_True_When_Cookies_Not_Available()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetCookies()).Returns((HttpCookieCollection)null);
            var logger = new MemoryLogger();

            var service = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            var result = service.IsUserValid(offer.Guid, user);

            Assert.True(result);
        }

        [Fact]
        public void IsUserValid_Returns_False_When_Cannot_Get_Tokens()
        {
            var offer = this.CreateOffer();
            var user = this.CreateCognitoUser(offer);
            var cookies = new HttpCookieCollection();
            var mockCognitoService = new Mock<ICognitoAuthService>();
            mockCognitoService.Setup(x => x.GetTokens(cookies)).Returns((OAuthTokensModel)null);
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetCookies()).Returns(cookies);
            var logger = new MemoryLogger();

            var service = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            var result = service.IsUserValid(offer.Guid, user);

            Assert.False(result);
        }

        [Fact]
        public void IsUserValid_Returns_False_When_Cognito_Email_Dont_Match_To_Cookies()
        {
            var offer = this.CreateOffer();
            var user = this.CreateCognitoUser(offer);
            user.CognitoUser = new CognitoUserModel("sub", new[] { "B2C" }, true, "abcdef", "Lukas", "Dvorak","radek.sukup@actumdigital.com","abcdef");
            var cookies = new HttpCookieCollection();
            var tokens = new OAuthTokensModel("ABC", "DEF", "GHI", "lukas.dvorak@actumdigital.com");
            var mockCognitoService = new Mock<ICognitoAuthService>();
            mockCognitoService.Setup(x => x.GetTokens(cookies)).Returns((OAuthTokensModel)null);
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetCookies()).Returns(cookies);
            var logger = new MemoryLogger();

            var service = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            var result = service.IsUserValid(offer.Guid, user);

            Assert.False(result);
        }

        [Fact]
        public void SaveUser_Stores_Data_In_Cache()
        {
            var guid = this.CreateGuid();
            var user = new UserCacheDataModel();
            bool dataSaved = false;
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            mockCacheService.Setup(x => x.Set(Constants.CacheKeys.USER_DATA, user)).Callback(() => { dataSaved = true; });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var userService = new UserService(
                mockCognitoService.Object,
                mockSettingsService.Object,
                mockCacheService.Object,
                mockContextWrapper.Object,
                logger
            );

            userService.SaveUser(guid, user);

            Assert.True(dataSaved);
        }

        [Fact]
        public void TryUpdateUserFromContext_Returns_True_When_User_IsCognito()
        {
            var offer = this.CreateOffer();
            var user = this.CreateCognitoUser(offer);
            var mockCognitoService = new Mock<ICognitoAuthService>();
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            using (var memoryStream = new MemoryStream())
            {
                using (var textWriter = new StreamWriter(memoryStream))
                {
                    var httpRequest = new HttpRequest("/", "http://localhost", "");
                    var httpResponse = new HttpResponse(textWriter);
                    var httpContext = new HttpContext(httpRequest, httpResponse);

                    HttpContext.Current = httpContext;

                    var userService = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

                    var result = userService.TryUpdateUserFromContext(offer.Guid, user);
                    Assert.True(result);
                }
            }
        }

        [Fact]
        public void GetRefreshTokens_Returns_Null_When_Validity_Missing()
        {
            var tokens = new OAuthTokensModel("ZXNzIiwic2NvcGUiOiJhd3", "3MuY29tXC9ldS13ZXN0LT", "zQ0MjUsImp0aSI6Im", "lukas.dvorak@actumdigital.com");

            var mockCognitoService = new Mock<ICognitoAuthService>();
            mockCognitoService.Setup(x => x.GetTokenValidity(tokens)).Returns((DateTime?)null);
            var mockSettingsService = new Mock<ISettingsReaderService>();
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var userService = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = userService.GetRefreshTokens(tokens);

            Assert.Null(result);
        }

        [Fact]
        public void GetRefreshTokens_Returns_Null_When_Validity_Didnt_Reach_Limit()
        {
            var tokens = new OAuthTokensModel("ZXNzIiwic2NvcGUiOiJhd3", "3MuY29tXC9ldS13ZXN0LT", "zQ0MjUsImp0aSI6Im", "lukas.dvorak@actumdigital.com");

            var mockCognitoService = new Mock<ICognitoAuthService>();
            mockCognitoService.Setup(x => x.GetTokenValidity(tokens)).Returns(DateTime.UtcNow.AddSeconds(360));
            var mockSettingsService = new Mock<ISettingsReaderService>();
            mockSettingsService.Setup(x => x.CognitoMinSecondsToRefreshToken).Returns(180);
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var userService = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = userService.GetRefreshTokens(tokens);

            Assert.Null(result);
        }

        [Fact]
        public void GetRefreshTokens_Returns_Null_When_New_Tokens_Are_Null()
        {
            var tokens = new OAuthTokensModel("ZXNzIiwic2NvcGUiOiJhd3", "3MuY29tXC9ldS13ZXN0LT", "zQ0MjUsImp0aSI6Im", "lukas.dvorak@actumdigital.com");

            var mockCognitoService = new Mock<ICognitoAuthService>();
            mockCognitoService.Setup(x => x.GetTokenValidity(tokens)).Returns(DateTime.UtcNow.AddSeconds(100));
            mockCognitoService.Setup(x => x.GetRefreshedTokens(tokens)).Returns((OAuthTokensModel)null);
            var mockSettingsService = new Mock<ISettingsReaderService>();
            mockSettingsService.Setup(x => x.CognitoMinSecondsToRefreshToken).Returns(180);
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var userService = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = userService.GetRefreshTokens(tokens);

            Assert.Null(result);
            }

        [Fact]
        public void GetRefreshTokens_Returns_Refresh_Tokens()
        {
            var tokens = new OAuthTokensModel("ZXNzIiwic2NvcGUiOiJhd3", "3MuY29tXC9ldS13ZXN0LT", "zQ0MjUsImp0aSI6Im", "lukas.dvorak@actumdigital.com");
            var newTokens = new OAuthTokensModel("ZXNzIiwic2NvcGUiOiJhd3", "3MuY29tXC9ldS13ZXN0LT", "5IiwidG9rZW5fdXNlIjoia", "lukas.dvorak@actumdigital.com");
            var mockCognitoService = new Mock<ICognitoAuthService>();
            mockCognitoService.Setup(x => x.GetTokenValidity(tokens)).Returns(DateTime.UtcNow.AddSeconds(100));
            mockCognitoService.Setup(x => x.GetRefreshedTokens(tokens)).Returns(newTokens);
            var mockSettingsService = new Mock<ISettingsReaderService>();
            mockSettingsService.Setup(x => x.CognitoMinSecondsToRefreshToken).Returns(180);
            var mockCacheService = new Mock<IDataSessionCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var userService = new UserService(
                        mockCognitoService.Object,
                        mockSettingsService.Object,
                        mockCacheService.Object,
                        mockContextWrapper.Object,
                        logger
                    );

            var result = userService.GetRefreshTokens(tokens);

            Assert.NotNull(result);
        }
    }
}
