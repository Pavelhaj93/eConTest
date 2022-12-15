using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace eContracting.Services.Tests
{
    /// <summary>
    /// JUST FOR LOCAL TESTING, IT'S NOT UNIT TEST.
    /// </summary>
    public class CallMeBackServiceLOCALTests : BaseTestClass
    {
        // JUST FOR LOCAT TESTING
        //[Fact]
        public void GetCsrfToken_With_2_Secrets()
        {
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.SetupGet(x => x.CrmAnonymousUrl).Returns(new Uri("https://sitecoredev-gw-test.innogy.cz:52003/sap/opu/odata/sap/CRM_UTILITIES_UMC/"));
            mockSettingsReader.SetupGet(x => x.CrmAnonymousUser).Returns("RFC_TEL");
            mockSettingsReader.SetupGet(x => x.CrmAnonymousPassword).Returns("TEL-100rfc");
            var logger = new MemoryLogger();
            var cmb = new CallMeBackModel();
            cmb.OfferGuid = "0635F899B3111EDCB8B1835A273F49BE";
            var service = new CallMeBackService(mockSettingsReader.Object, logger);

            var result = service.GetCsrfTokenAsAnonymous(cmb);

            Assert.NotNull(result.Item1);
        }

        // JUST FOR LOCAT TESTING
        //[Fact]
        public void GetCsrfToken_With_Cognito()
        {
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            // https://sitecoredev-gw-test.innogy.cz:52003/sap/opu/odata/sap/CRM_UTILITIES_UMC/
            mockSettingsReader.SetupGet(x => x.CrmCognitoUrl).Returns(new Uri("https://api.innogy.cz/test/api/mcfu/CRM_UTILITIES_UMC"));
            mockSettingsReader.SetupGet(x => x.SapApiGatewayId).Returns("d6bwzpwoq4");
            var logger = new MemoryLogger();
            var cmb = new CallMeBackModel();
            cmb.OfferGuid = "0635F899B3111EDCB8B1835A273F49BE";
            var user = new UserCacheDataModel();
            var congnitoData = JsonConvert.DeserializeObject<CognitoUserDataModel>("{\"UserAttributes\":[{\"Name\":\"sub\",\"Value\":\"be30ca9b-12b3-4794-96f9-4dea14760104\"},{\"Name\":\"custom:segment\",\"Value\":\"B2C_Customers\"},{\"Name\":\"email_verified\",\"Value\":\"True\"},{\"Name\":\"preferred_username\",\"Value\":\"be30ca9b-12b3-4794-96f9-4dea14760104\"},{\"Name\":\"given_name\",\"Value\":\"Libor\"},{\"Name\":\"family_name\",\"Value\":\"Ševčík\"},{\"Name\":\"email\",\"Value\":\"runway@yopmail.com\"}],\"Username\":\"be30ca9b-12b3-4794-96f9-4dea14760104\"}");
            user.CognitoUser = new CognitoUserModel(congnitoData);
            user.AuthorizedGuids[cmb.OfferGuid] = AUTH_METHODS.COGNITO;
            user.Tokens = new OAuthTokensModel(
                "eyJraWQiOiI2M3ZkdXJCZ3lrd2NlcklQcFJvSWNtSVVBSGlNOFB6QWV5enRtU2QzZitnPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiJiZTMwY2E5Yi0xMmIzLTQ3OTQtOTZmOS00ZGVhMTQ3NjAxMDQiLCJjb2duaXRvOmdyb3VwcyI6WyJCMkNfQ3VzdG9tZXJzIl0sImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS13ZXN0LTEuYW1hem9uYXdzLmNvbVwvZXUtd2VzdC0xX0gxUkZPSERESSIsImNsaWVudF9pZCI6IjFqZ2JnaGpwbmg4Y3E0cWE0ODM4ZWhzcTlsIiwib3JpZ2luX2p0aSI6IjA5NmRjMDBkLTQyYTEtNDRmMi05ZWFiLTQ3ZGQ2N2UyM2U2MSIsImV2ZW50X2lkIjoiNWFmZTQzMjYtMGMxNy00NGEyLTkzZjUtNmE0ZGMxNmMxOWM1IiwidG9rZW5fdXNlIjoiYWNjZXNzIiwic2NvcGUiOiJhd3MuY29nbml0by5zaWduaW4udXNlci5hZG1pbiIsImF1dGhfdGltZSI6MTY2OTk4NjYwOCwiZXhwIjoxNjY5OTkwMjA4LCJpYXQiOjE2Njk5ODY2MDgsImp0aSI6ImU5MmI2MTczLTI4OGYtNGU3Ny1hN2NmLTYwMWFhMTIyMDZlMCIsInVzZXJuYW1lIjoiYmUzMGNhOWItMTJiMy00Nzk0LTk2ZjktNGRlYTE0NzYwMTA0In0.F2Cd3WV_sPlOhZrIOK2lhykaiMI-JSfmrzgZgHJ3ji-4DwuDNcWB7r53gX-NxdNbFXJo9_6Tcf_MjjEkaGlUiOeWglyTmAO1BeSrTdpX4NFZkLcfRNi_pFNICDzgHWaMxUvo-BTSNNb0gzaRhOyamW-3OesdMdFKDlVx4M68xjSoqPx8aSuIEs4BOP4BXwueGGhmccvorTlr6Fx6R9Q1Nvk7-6ihfVhL3t52gdFzKS0koF01jXKOJBUmo0T_Zf4piXUnOqwHCNyvGn_3X5oUXfXeOO9MBCdCjsPAiQ5u1aOl6YP9j8ZNuKdQwvJRyDVxdjittIyVts96dlXDDDZXOg",
                "idToken",
                "refreshToken",
                "runway@yopmail.com");
            var service = new CallMeBackService(mockSettingsReader.Object, logger);

            var result = service.GetCsrfTokenAsCognito(cmb, user);

            Assert.NotNull(result.Item1);
        }

        // JUST FOR LOCAT TESTING
        // eyJraWQiOiI2M3ZkdXJCZ3lrd2NlcklQcFJvSWNtSVVBSGlNOFB6QWV5enRtU2QzZitnPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI1OTVhMzYyZC03ODdhLTRiMjItYmNjZS0zOTkzM2U1ODBlYzkiLCJjb2duaXRvOmdyb3VwcyI6WyJCMkNfQ3VzdG9tZXJzIl0sImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS13ZXN0LTEuYW1hem9uYXdzLmNvbVwvZXUtd2VzdC0xX0gxUkZPSERESSIsImNsaWVudF9pZCI6IjFqZ2JnaGpwbmg4Y3E0cWE0ODM4ZWhzcTlsIiwib3JpZ2luX2p0aSI6IjA1ZjQ2MzZhLWJkNGYtNGZmOS1hMWM1LWNjNDk2ZDk4OGQ4NSIsImV2ZW50X2lkIjoiZWUzODM1MjktYmExZC00OGI1LWFlMjUtYWQ2MDJlYWQzZmM2IiwidG9rZW5fdXNlIjoiYWNjZXNzIiwic2NvcGUiOiJhd3MuY29nbml0by5zaWduaW4udXNlci5hZG1pbiIsImF1dGhfdGltZSI6MTY2ODYxNjA1MSwiZXhwIjoxNjY4NjE5NjUzLCJpYXQiOjE2Njg2MTYwNTMsImp0aSI6IjkzM2QzYTA2LTJmZTMtNGJlNS1hNTNkLWQ4YmFiN2JmYjI1ZSIsInVzZXJuYW1lIjoiNTk1YTM2MmQtNzg3YS00YjIyLWJjY2UtMzk5MzNlNTgwZWM5In0.IWI053z4penDbEIUumhHF1sUfP80jK08vT0gJ_xGzeer-wAg0i9CS6jhQEABYV_ZqkTWXCH08hcU7LAm3BQIxjfofsSwnaa1eIpGx9bdwDydRo5_os4sBk9M7g6GfDGylGgnrGVJ9R8jUFQ7Qq2UHMCWGMp4MaGC6SdMN4XePAamL7B2aSU2egUgG5-QDljNcYDGWjSzDF2ys3HHE2dwUFGVopisJKZfSDrYn2ebGqrHfzFGqCIi_OcqZ9l8IdpPiwj3Jlz1Mnu8_hPZQpvfnQC6RUE95EDz_20_qquIHPkhQpuMYidiLCvYFHgmlxbJk1Ypu7H84SFvuuHeGSxX0Q
        //[Fact]
        public void Send()
        {
            var mockSettingsReader = new Mock<ISettingsReaderService>();
            mockSettingsReader.SetupGet(x => x.CrmAuthService).Returns(new Uri("https://wti.rwe-services.cz:51012/sap/opu/odata/sap/ZCRM_AUTH_SRV/"));
            mockSettingsReader.SetupGet(x => x.CrmUtilitiesUmc).Returns(new Uri("https://sitecoredev-gw-test.innogy.cz:52003/sap/opu/odata/sap/CRM_UTILITIES_UMC"));
            var mockDatasource = new Mock<ICallMeBackModalWindow>();
            mockDatasource.SetupGet(x => x.SettingsDescription).Returns("CMB RW-odvolani_ret2");
            var logger = new MemoryLogger();

            var cmb = new CallMeBackModel();
            cmb.Partner = "9610909649";
            cmb.Note = "Test";
            cmb.EicEan = "27ZG200Z0289897F";
            cmb.Phone = "724 153 093";
            cmb.Process = "CMB_ECON";
            cmb.ExternalSystem = "econtracting";
            cmb.SelectedTime = "10:00-10:30";

            var service = new CallMeBackService(mockSettingsReader.Object, logger);

            //service.Send(cmb, mockDatasource.Object);
        }
    }
}
