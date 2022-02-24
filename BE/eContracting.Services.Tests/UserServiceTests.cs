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
    }
}
