using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;
using eContracting.Tests;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace eContracting.Services.Tests
{
    public class CognitoAuthServiceTests : BaseTestClass
    {
        [Fact]
        public void CognitoSettingsModel_Returns_Model_Only_When_All_Settings_Exists()
        {
            var settings = this.CreateCognitoSettings();

            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            var result = service.GetSettings();

            Assert.NotNull(result);
        }

        [Fact]
        public void CognitoSettingsModel_Throws_EcontractingApplicationException_When_CognitoBaseUrl_Missing()
        {
            var model = this.CreateCognitoSettings();

            var settings = new CognitoSettingsModel("", model.CognitoTokensUrl, model.CognitoClientId, model.CognitoCookiePrefix, model.CognitoCookieUser, model.InnogyLoginUrl, model.InnogyLogoutUrl, model.InnogyRegistrationUrl, model.InnogyDashboardUrl);

            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            Assert.Throws<EcontractingApplicationException>(() => service.GetSettings() );
        }

        [Fact]
        public void CognitoSettingsModel_Throws_EcontractingApplicationException_When_CognitoClientId_Missing()
        {
            var model = this.CreateCognitoSettings();

            var settings = new CognitoSettingsModel(model.CognitoBaseUrl, model.CognitoTokensUrl, "", model.CognitoCookiePrefix, model.CognitoCookieUser, model.InnogyLoginUrl, model.InnogyLogoutUrl, model.InnogyRegistrationUrl, model.InnogyDashboardUrl);

            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            Assert.Throws<EcontractingApplicationException>(() => service.GetSettings());
        }

        [Fact]
        public void CognitoSettingsModel_Throws_EcontractingApplicationException_When_InnogyLoginUrl_Missing()
        {
            var model = this.CreateCognitoSettings();

            var settings = new CognitoSettingsModel(model.CognitoBaseUrl, model.CognitoTokensUrl, model.CognitoClientId, model.CognitoCookiePrefix, model.CognitoCookieUser, "", model.InnogyLogoutUrl, model.InnogyRegistrationUrl, model.InnogyDashboardUrl);

            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            Assert.Throws<EcontractingApplicationException>(() => service.GetSettings());
        }

        [Fact]
        public void CognitoSettingsModel_Throws_EcontractingApplicationException_When_InnogyLogoutUrl_Missing()
        {
            var model = this.CreateCognitoSettings();

            var settings = new CognitoSettingsModel(model.CognitoBaseUrl, model.CognitoTokensUrl, model.CognitoClientId, model.CognitoCookiePrefix, model.CognitoCookieUser, model.InnogyLoginUrl, "", model.InnogyRegistrationUrl, model.InnogyDashboardUrl);

            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            Assert.Throws<EcontractingApplicationException>(() => service.GetSettings());
        }

        [Fact]
        public void GetToken_Returns_OAuthTokensModel()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.accessToken", "AccessToken"));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.idToken", "IdToken"));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.refreshToken", "RefreshToken"));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_Cookies_Are_Null()
        {
            var mockSettingReader = new Mock<ISettingsReaderService>();
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens((HttpCookieCollection)null);

            Assert.Null(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_CookieUser_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("CustomCookie"));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_AccessToken_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", "lukas.dvorak@actumdigital.com"));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_AccessToken_Cookie_Value_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.accessToken", ""));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_IdToken_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.accessToken", "AccessToken"));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_IdToken_Cookie_Value_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.accessToken", "AccessToken"));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.idToken", ""));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_RefreshToken_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.accessToken", "AccessToken"));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.idToken", "IdToken"));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetTokens_Returns_Null_When_RefreshToken_Cookie_Value_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.accessToken", "AccessToken"));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.idToken", "IdToken"));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.refreshToken", ""));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetTokens(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetUser_Returns_CognitoUserModel()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();
            var userData = this.CreateCognitoUserDataModel();

            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{HttpUtility.UrlEncode(username)}.userData", JsonConvert.SerializeObject(userData)));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);
            var result = service.GetUser(cookies);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUser_Returns_Null_When_Cookies_Are_Null()
        {
            var mockSettingReader = new Mock<ISettingsReaderService>();
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            var result = service.GetUser((HttpCookieCollection)null);

            Assert.Null(result);
        }

        [Fact]
        public void GetUser_Returns_Null_When_CookieUser_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();
            var cookies = new HttpCookieCollection();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            var result = service.GetUser(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetUser_Returns_Null_When_Cookie_UserData_Missing()
        {
            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            var logger = new MemoryLogger();
            var username = "lukas.dvorak@actumdigital.com";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie($"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}", username));

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            var result = service.GetUser(cookies);

            Assert.Null(result);
        }

        [Fact]
        public void GetRefreshedTokens_Returns_New_Instance_With_Same_RefreshToken_And_LastUser()
        {
            var tokens = new OAuthTokensModel("ZXNzIiwic2NvcGUiOiJhd3", "3MuY29tXC9ldS13ZXN0LT", "zQ0MjUsImp0aSI6Im", "lukas.dvorak@actumdigital.com");

            var jwtResponse = new JObject();
            jwtResponse.Add("access_token", new JValue("FmYS00MDYwLWFkYzctZGJk"));
            jwtResponse.Add("id_token", new JValue("iMmZmMTY1MTUtZmFmYS00MD"));
            jwtResponse.Add("token_type", new JValue("Bearer"));
            jwtResponse.Add("expires_in", new JValue(14400));

            var serializedJwtResponse = JsonConvert.SerializeObject(jwtResponse);
            var jwtRefreshTokens = JsonConvert.DeserializeObject<JwtRefreshTokenModel>(serializedJwtResponse);

            var settings = this.CreateCognitoSettings();
            var mockSettingReader = new Mock<ISettingsReaderService>();
            mockSettingReader.Setup(x => x.GetCognitoSettings()).Returns(settings);
            var mockTokensParser = new Mock<ITokenParser>();
            var mockRestApiService = new Mock<IRespApiService>();
            mockRestApiService.Setup(x => x.GetResponse<JwtRefreshTokenModel>(It.IsAny<HttpRequestMessage>())).Returns(new RestApiResponseModel<JwtRefreshTokenModel>(jwtRefreshTokens, System.Net.HttpStatusCode.OK, true));
            var logger = new MemoryLogger();

            var service = new CognitoAuthService(mockSettingReader.Object, mockTokensParser.Object, mockRestApiService.Object, logger);

            var result = service.GetRefreshedTokens(tokens);

            Assert.Equal(tokens.RefreshToken, result.RefreshToken);
            Assert.Equal(tokens.LastAuthUser, result.LastAuthUser);
            Assert.NotEqual(tokens.AccessToken, result.AccessToken);
            Assert.NotEqual(tokens.IdToken, result.IdToken);
        }

    }
}
