using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
using Moq;
using Xunit;

namespace eContracting.Services.Tests
{
    [Trait("Service", "SitecoreSettingsReaderService")]
    [ExcludeFromCodeCoverage]
    public class AuthenticationServiceTests : BaseTestClass
    {
        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_INVALID_BIRTHDATE_When_Empty()
        {
            var offer = this.CreateOffer();
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, "", "id", "132546796");

            Assert.Equal(AUTH_RESULT_STATES.INVALID_BIRTHDATE, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_KEY_MISMATCH_When_Empty()
        {
            var offer = this.CreateOffer();
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, "23.11.2001", "", "132546796");

            Assert.Equal(AUTH_RESULT_STATES.KEY_MISMATCH, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_INVALID_VALUE_When_Empty()
        {
            var offer = this.CreateOffer();
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, "23.11.2001", "key", "");

            Assert.Equal(AUTH_RESULT_STATES.INVALID_VALUE, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_INVALID_BIRTHDATE_When_Dont_Match()
        {
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BIRTHDT = "25.06.1971";
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, "17.11.2020", "id", "132546796");

            Assert.Equal(AUTH_RESULT_STATES.INVALID_BIRTHDATE, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_KEY_MISMATCH_When_Login_Type_Doesnt_Match()
        {
            var guid = Guid.NewGuid().ToString("N");
            var birthdate = "25.06.1971";
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BIRTHDT = birthdate;
            var id = Guid.NewGuid();
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();
            loginType.Key = "PARTNER";
            var key = Utils.GetUniqueKey(loginType, offer);
            var loginTypes = new List<LoginTypeModel>();
            loginTypes.Add(loginType);

            var mockReaderSettings = new Mock<ISettingsReaderService>();
            mockReaderSettings.Setup(x => x.GetAllLoginTypes()).Returns(loginTypes);
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, birthdate, "invalidkey", "132546796");

            Assert.Equal(AUTH_RESULT_STATES.KEY_MISMATCH, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_KEY_MISMATCH()
        {
            var guid = Guid.NewGuid().ToString("N");
            var birthdate = "25.06.1971";
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BIRTHDT = birthdate;

            var id = Guid.NewGuid();
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();
            loginType.Key = "PARTNER";
            var key = Utils.GetUniqueKey(loginType, offer);
            var loginTypes = new List<LoginTypeModel>();
            loginTypes.Add(loginType);

            var mockReaderSettings = new Mock<ISettingsReaderService>();
            mockReaderSettings.Setup(x => x.GetAllLoginTypes()).Returns(loginTypes);
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, birthdate, key, "132546796");

            Assert.Equal(AUTH_RESULT_STATES.INVALID_VALUE_DEFINITION, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_INVALID_VALUE_FORMAT()
        {
            var partnerId1 = "843374421";
            var partnerId2 = "8433x4421";
            var guid = Guid.NewGuid().ToString("N");
            var birthdate = "25.06.1971";
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BIRTHDT = birthdate;
            offer.Xml.Content.Body.PARTNER = partnerId1;

            var id = Guid.NewGuid();
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();
            loginType.Key = "PARTNER";
            loginType.ValidationRegex = "^[0-9]*$";
            var key = Utils.GetUniqueKey(loginType, offer);
            var loginTypes = new List<LoginTypeModel>();
            loginTypes.Add(loginType);

            var mockReaderSettings = new Mock<ISettingsReaderService>();
            mockReaderSettings.Setup(x => x.GetAllLoginTypes()).Returns(loginTypes);
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, birthdate, key, partnerId2);

            Assert.Equal(AUTH_RESULT_STATES.INVALID_VALUE_FORMAT, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_INVALID_VALUE()
        {
            var partnerId1 = "843374421";
            var partnerId2 = "843374422";
            var guid = Guid.NewGuid().ToString("N");
            var birthdate = "25.06.1971";
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BIRTHDT = birthdate;
            offer.Xml.Content.Body.PARTNER = partnerId1;
            var id = Guid.NewGuid();
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();
            loginType.Key = "PARTNER";
            loginType.ValidationRegex = "^[0-9]*$";
            var key = Utils.GetUniqueKey(loginType, offer);
            var loginTypes = new List<LoginTypeModel>();
            loginTypes.Add(loginType);

            var mockReaderSettings = new Mock<ISettingsReaderService>();
            mockReaderSettings.Setup(x => x.GetAllLoginTypes()).Returns(loginTypes);
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, birthdate, key, partnerId2);

            Assert.Equal(AUTH_RESULT_STATES.INVALID_VALUE, result);
        }

        [Fact]
        [Trait("Method", "GetLoginState")]
        public void GetLoginState_Returns_SUCCEEDED()
        {
            var partnerId = "843374421";
            var guid = Guid.NewGuid().ToString("N");
            var birthdate = "25.06.1971";
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BIRTHDT = birthdate;
            offer.Xml.Content.Body.PARTNER = partnerId;
            var id = Guid.NewGuid();
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();
            loginType.Key = "PARTNER";
            loginType.ValidationRegex = "^[0-9]*$";
            var key = Utils.GetUniqueKey(loginType, offer);
            var loginTypes = new List<LoginTypeModel>();
            loginTypes.Add(loginType);

            var mockReaderSettings = new Mock<ISettingsReaderService>();
            mockReaderSettings.Setup(x => x.GetAllLoginTypes()).Returns(loginTypes);
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetLoginState(offer, birthdate, key, partnerId);

            Assert.Equal(AUTH_RESULT_STATES.SUCCEEDED, result);
        }

        [Fact]
        [Trait("Method", "GetMatched")]
        public void GetMatched_Finds_Match()
        {
            var guid = Guid.NewGuid().ToString("N");
            var birthdate = "25.06.1971";
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BIRTHDT = birthdate;

            var id = Guid.NewGuid();
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();
            loginType.Key = "PARTNER";
            var key = Utils.GetUniqueKey(loginType, offer);
            var loginTypes = new List<LoginTypeModel>();
            loginTypes.Add(loginType);

            var mockReaderSettings = new Mock<ISettingsReaderService>();
            mockReaderSettings.Setup(x => x.GetAllLoginTypes()).Returns(loginTypes);
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetMatched(offer, key);

            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetMatched")]
        public void GetMatched_Not_Finds_Match()
        {
            var guid = Guid.NewGuid().ToString("N");
            var birthdate = "25.06.1971";
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BIRTHDT = birthdate;
            var id = Guid.NewGuid();
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();
            loginType.Key = "PARTNER";
            var key = Utils.GetUniqueKey(loginType, offer);
            var loginTypes = new List<LoginTypeModel>();
            loginTypes.Add(loginType);

            var mockReaderSettings = new Mock<ISettingsReaderService>();
            mockReaderSettings.Setup(x => x.GetAllLoginTypes()).Returns(loginTypes);
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.GetMatched(offer, "mishmashkey");

            Assert.Null(result);
        }

        [Fact]
        [Trait("Method", "IsRegexValid")]
        public void IsRegexValid_LoginType_Has_Valid_Regex_And_Returns_True()
        {
            var loginType = new LoginTypeModel();
            loginType.ValidationRegex = "^[0-9]*$";
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.IsRegexValid(loginType, "486937476");

            Assert.True(result);
        }

        [Fact]
        [Trait("Method", "IsRegexValid")]
        public void IsRegexValid_LoginType_Has_Valid_Regex_And_Returns_False()
        {
            var loginType = new LoginTypeModel();
            loginType.ValidationRegex = "^[0-9]*$";
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.IsRegexValid(loginType, "4869x7476");

            Assert.False(result);
        }

        [Fact]
        [Trait("Method", "IsRegexValid")]
        public void IsRegexValid_LoginType_Has_No_Regex_And_Returns_True()
        {
            var loginType = new LoginTypeModel();
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.IsRegexValid(loginType, "4869x7476");

            Assert.True(result);
        }

        [Fact]
        [Trait("Method", "IsRegexValid")]
        public void IsRegexValid_LoginType_Has_Invalid_Regex_And_Returns_False()
        {
            var loginType = new LoginTypeModel();
            loginType.ValidationRegex = "?^![0-9]*$";
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.IsRegexValid(loginType, "486937476");

            Assert.False(result);
        }

        [Fact]
        [Trait("Method", "Login")]
        public void Login_Adds_AuthDataModel_To_Cache()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            bool inserted = false;
            var authData = new AuthDataModel(offer);
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.AddToSession(Constants.CacheKeys.AUTH_DATA, authData)).Callback(() => { inserted = true; });

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            service.Login(authData);

            Assert.True(inserted);
        }

        [Fact]
        [Trait("Method", "IsLoggedIn")]
        public void IsLoggedIn_Returns_True_When_AuthDataModel_Exists()
        {
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);
            var authData = new AuthDataModel(offer);
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.GetFromSession<AuthDataModel>(Constants.CacheKeys.AUTH_DATA)).Returns(authData);

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.IsLoggedIn();

            Assert.True(result);
        }

        [Fact]
        [Trait("Method", "IsLoggedIn")]
        public void IsLoggedIn_Returns_False_Because_AuthDataModel_Doesnt_Exist()
        {
            var mockReaderSettings = new Mock<ISettingsReaderService>();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.GetFromSession<AuthDataModel>(Constants.CacheKeys.AUTH_DATA)).Returns((AuthDataModel)null);

            var service = new AuthenticationService(mockReaderSettings.Object, mockCache.Object);
            var result = service.IsLoggedIn();

            Assert.False(result);
        }
    }
}
