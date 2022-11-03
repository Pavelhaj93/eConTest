using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
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
    [Trait("Service", "SitecoreSettingsReaderService")]
    [ExcludeFromCodeCoverage]
    public class SitecoreSettingsReaderServiceTests : BaseTestClass
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
        [Trait("Method", "GetSiteSettings")]
        public void GetSiteSettings_Throws_MissingDatasourceException_When_Not_Found()
        {
            var siteRoot = "/site";
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns((ISiteSettingsModel)null);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();
            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);

            Assert.Throws<EcontractingMissingDatasourceException>(() => service.GetSiteSettings());
        }

        [Fact]
        [Trait("Method", "GetAllLoginTypes")]
        public void GetAllLoginTypes_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockLoginTypeFolder = new Mock<IFolderItemModel<ILoginTypeModel>>();
            var loginTypeFolder = mockLoginTypeFolder.Object;
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<ILoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES, false, false)).Returns(loginTypeFolder);
            mockSitecoreService.Setup(x => x.GetItems<ILoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<ILoginTypeModel>());
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            Assert.Throws<EcontractingMissingDatasourceException>(() => service.GetAllLoginTypes());
        }

        [Fact]
        [Trait("Method", "GetAllProcesses")]
        public void GetAllProcesses_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IProcessModel>>(Constants.SitecorePaths.PROCESSES)).Returns(new Mock<IFolderItemModel<IProcessModel>>().Object);
            mockSitecoreService.Setup(x => x.GetItems<IProcessModel>(Constants.SitecorePaths.PROCESSES)).Returns(Enumerable.Empty<IProcessModel>());
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            Assert.Throws<EcontractingMissingDatasourceException>(() => service.GetAllProcesses());
        }

        [Fact]
        [Trait("Method", "GetAllProcesses")]
        public void GetAllProcesses_Returns_Items()
        {
            var list = new List<IProcessModel>();
            list.Add(new Mock<IProcessModel>().Object);
            list.Add(new Mock<IProcessModel>().Object);
            list.Add(new Mock<IProcessModel>().Object);
            var mockFolder = new Mock<IFolderItemModel<IProcessModel>>();
            mockFolder.SetupProperty(x => x.Children, list);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IProcessModel>>(Constants.SitecorePaths.PROCESSES)).Returns(mockFolder.Object);
            mockSitecoreService.Setup(x => x.GetItems<IProcessModel>(Constants.SitecorePaths.PROCESSES)).Returns(list);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetAllProcesses();

            Assert.True(result.Count() == 3);
        }

        [Fact]
        [Trait("Method", "GetAllProcessTypes")]
        public void GetAllProcessTypes_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<ILoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(new Mock<IFolderItemModel<ILoginTypeModel>>().Object);
            mockSitecoreService.Setup(x => x.GetItems<ILoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<ILoginTypeModel>());
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            Assert.Throws<EcontractingMissingDatasourceException>(() => service.GetAllProcessTypes());
        }

        [Fact]
        [Trait("Method", "GetAllProcessTypes")]
        public void GetAllProcessTypes_Returns_Items()
        {
            var list = new List<IProcessTypeModel>();
            list.Add(new Mock<IProcessTypeModel>().Object);
            list.Add(new Mock<IProcessTypeModel>().Object);
            list.Add(new Mock<IProcessTypeModel>().Object);
            var mockFolder = new Mock<IFolderItemModel<IProcessTypeModel>>();
            mockFolder.SetupProperty(x => x.Children, list);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IProcessTypeModel>>(Constants.SitecorePaths.PROCESS_TYPES)).Returns(mockFolder.Object);
            mockSitecoreService.Setup(x => x.GetItems<IProcessTypeModel>(Constants.SitecorePaths.PROCESS_TYPES)).Returns(list);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetAllProcessTypes();

            Assert.True(result.Count() == 3);
        }

        [Fact]
        [Trait("Method", "GetApiServiceOptions")]
        public void GetApiServiceOptions_Takes_Only_Url_From_GetSetting()
        {
            var url = "http://sap.cz";
            string user = "joe";
            string password = "secret";

            var siteRoot = "/site";
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.ServiceUrl, "");
            mockSiteSettings.SetupProperty(x => x.ServiceUser, Convert.ToBase64String(Encoding.UTF8.GetBytes(user)));
            mockSiteSettings.SetupProperty(x => x.ServicePassword, Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(url);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        [Trait("Method", "GetApiServiceOptions")]
        public void GetApiServiceOptions_Takes_Only_Username_From_GetSetting()
        {
            string user = "joe";
            string password = "secret";

            var url = "http://sap.cz";
            var siteRoot = "/site";
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.ServiceUrl, url);
            mockSiteSettings.SetupProperty(x => x.ServiceUser, "");
            mockSiteSettings.SetupProperty(x => x.ServicePassword, Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(user)));
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        [Trait("Method", "GetApiServiceOptions")]
        public void GetApiServiceOptions_Takes_Only_Password_From_GetSetting()
        {
            string user = "joe";
            string password = "secret";

            var url = "http://sap.cz";

            var siteRoot = "/site";
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.ServiceUrl, url);
            mockSiteSettings.SetupProperty(x => x.ServiceUser, Convert.ToBase64String(Encoding.UTF8.GetBytes(user)));
            mockSiteSettings.SetupProperty(x => x.ServicePassword, "");
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        [Trait("Method", "GetApiServiceOptions")]
        public void GetApiServiceOptions_Get_Data_From_GetSetting()
        {
            var url = "http://sap.cz";
            string user = "joe";
            string password = "secret";

            var siteSettings = new Mock<ISiteSettingsModel>().Object;

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>("/sitecore/content")).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns("/sitecore/content");
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(url);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(user)));
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        [Trait("Method", "GetApiServiceOptions")]
        public void GetApiServiceOptions_Get_Data_From_GeneralSettings()
        {
            var url = "http://sap.cz";
            string user = "joe";
            string password = "secret";

            var siteRoot = "/site";
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.ServiceUrl, url);
            mockSiteSettings.SetupProperty(x => x.ServiceUser, Convert.ToBase64String(Encoding.UTF8.GetBytes(user)));
            mockSiteSettings.SetupProperty(x => x.ServicePassword, Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUrl")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServiceUser")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSetting("eContracting.ServicePassword")).Returns(string.Empty);
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var options = service.GetApiServiceOptions();

            Assert.Equal(new Uri(url), options.Url);
            Assert.Equal(user, options.User);
            Assert.Equal(password, options.Password);
        }

        [Theory]
        [InlineData("CUSTTITLELET", "PERSON_CUSTTITLELET")]
        [InlineData("CUSTADDRESS", "PERSON_CUSTADDRESS")]
        [InlineData("PREMADR", "PERSON_PREMADR")]
        [InlineData("PREMLABEL", "PERSON_PREMLABEL")]
        [InlineData("PREMEXT", "PERSON_PREMEXT")]
        public void GetBackCompatibleTextParametersKeys_Contains_CUSTTITLELET_For_Version_1(string oldKey, string newKey)
        {
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetBackCompatibleTextParametersKeys(1);

            Assert.Contains(result, x => x.Key == oldKey && x.Value == newKey);
        }

        //[Fact]
        //public void GetCustomDatabaseConnectionString_Returns_Default_Connection_String()
        //{
        //    var expected = "http://localhost/db";
        //    System.Configuration.ConfigurationManager.ConnectionStrings.Add(new System.Configuration.ConnectionStringSettings(Constants.DatabaseContextConnectionStringName, expected));

        //    var mockSitecoreService = new Mock<ISitecoreContextExtended>();
        //    var mockContextWrapper = new Mock<IContextWrapper>();
        //    var logger = new MemoryLogger();

        //    var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
        //    var result = service.GetCustomDatabaseConnectionString();

        //    Assert.Equal(expected, result);

        //    System.Configuration.ConfigurationManager.ConnectionStrings.Clear();
        //}

        [Fact]
        public void GetCustomDatabaseConnectionString_Throws_ApplicationException_When_Name_Missing()
        {
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);

            Assert.Throws<ApplicationException>(() => service.GetCustomDatabaseConnectionString());
        }

        [Fact]
        public void GetCustomDatabaseConnectionString_Throws_ApplicationException_When_Connection_String_Not_Found()
        {
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);

            Assert.Throws<ApplicationException>(() => service.GetCustomDatabaseConnectionString());
        }

        [Fact]
        [Trait("Method", "GetDefinition")]
        public void GetDefinition_Returns_Model_When_Matched_Offer()
        {
            var process = "XYZ";
            var processType = "123";
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;

            var mockProcess = new Mock<IProcessModel>();
            mockProcess.SetupProperty(x => x.Code, process);
            var mockProcessType = new Mock<IProcessTypeModel>();
            mockProcessType.SetupProperty(x => x.Code, processType);
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.Process, mockProcess.Object);
            mockDefinition.SetupProperty(x => x.ProcessType, mockProcessType.Object);
            var combination = mockDefinition.Object;
            var mockFolder = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolder.SetupProperty(x => x.Children, new[] { combination });
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolder.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetDefinition(offer);

            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetDefinition")]
        public void GetDefinition_Returns_Default_Definition_When_NotMatched_Offer()
        {
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = "XYZ";
            offer.Xml.Content.Body.BusProcessType = "123";

            var mockProcess = new Mock<IProcessModel>();
            mockProcess.SetupProperty(x => x.Code, "ABCDED");
            var mockProcessType = new Mock<IProcessTypeModel>();
            mockProcessType.SetupProperty(x => x.Code, "09988");
            var mockCombination = new Mock<IDefinitionCombinationModel>();
            mockCombination.SetupProperty(x => x.Process, mockProcess.Object);
            mockCombination.SetupProperty(x => x.ProcessType, mockProcessType.Object);
            var combination = mockCombination.Object;
            var defaultCombination = new Mock<IDefinitionCombinationModel>().Object;

            var mockFolder = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolder.SetupProperty(x => x.Children, new[] { combination });

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolder.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            mockSitecoreService.Setup(x => x.GetItem<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(defaultCombination);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetDefinition(offer);

            Assert.Equal(result, defaultCombination);
        }

        [Fact]
        [Trait("Method", "GetDefinition")]
        public void GetDefinition_Returns_Model_When_Matched_Parameters()
        {
            var mockProcess = new Mock<IProcessModel>();
            mockProcess.SetupProperty(x => x.Code, "XYZ");
            var mockProcessType = new Mock<IProcessTypeModel>();
            mockProcessType.SetupProperty(x => x.Code, "123");
            var mockCombination = new Mock<IDefinitionCombinationModel>();
            mockCombination.SetupProperty(x => x.Process, mockProcess.Object);
            mockCombination.SetupProperty(x => x.ProcessType, mockProcessType.Object);
            var combination = mockCombination.Object;
            var mockFolder = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolder.SetupProperty(x => x.Children, new[] { combination });
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolder.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetDefinition("XYZ", "123");

            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetDefinition")]
        public void GetDefinition_Throws_EcontractingMissingDatasourceException_When_NotMatched_Parameters()
        {
            var mockProcess = new Mock<IProcessModel>();
            mockProcess.SetupProperty(x => x.Code, "XYZ");
            var mockProcessType = new Mock<IProcessTypeModel>();
            mockProcessType.SetupProperty(x => x.Code, "123");
            var mockCombination = new Mock<IDefinitionCombinationModel>();
            mockCombination.SetupProperty(x => x.Process, mockProcess.Object);
            mockCombination.SetupProperty(x => x.ProcessType, mockProcessType.Object);
            var combination = mockCombination.Object;
            var defaultCombination = new Mock<IDefinitionCombinationModel>().Object;
            var mockFolder = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolder.SetupProperty(x => x.Children, new[] { combination });
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolder.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);

            Assert.Throws<EcontractingMissingDatasourceException>(() => { service.GetDefinition("HS623HA", "094JF83"); });
        }

        [Fact]
        [Trait("Method", "GetLoginTypes")]
        public void GetLoginTypes_Returns_All_Preselected_When_Random_False()
        {
            var process = "XYZ";
            var processType = "123";

            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;

            var loginTypes = new List<ILoginTypeModel>();
            var mockLoginType1 = new Mock<ILoginTypeModel>();
            mockLoginType1.SetupProperty(x => x.Name, "LT1");
            loginTypes.Add(mockLoginType1.Object);
            var mockLoginType2 = new Mock<ILoginTypeModel>();
            mockLoginType2.SetupProperty(x => x.Name, "LT2");
            loginTypes.Add(mockLoginType2.Object);
            var mockLoginType3 = new Mock<ILoginTypeModel>();
            mockLoginType3.SetupProperty(x => x.Name, "LT3");
            loginTypes.Add(mockLoginType3.Object);

            var mockProcess = new Mock<IProcessModel>();
            mockProcess.SetupProperty(x => x.Code, process);
            var mockProcessType = new Mock<IProcessTypeModel>();
            mockProcessType.SetupProperty(x => x.Code, processType);
            var mockCombination = new Mock<IDefinitionCombinationModel>();
            mockCombination.SetupProperty(x => x.Process, mockProcess.Object);
            mockCombination.SetupProperty(x => x.ProcessType, mockProcessType.Object);
            mockCombination.SetupProperty(x => x.LoginTypes, loginTypes);
            mockCombination.SetupProperty(x => x.LoginTypesRandom, false);
            var combination = mockCombination.Object;
            var defaultCombination = new Mock<IDefinitionCombinationModel>().Object;
            var mockFolderCombinations = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolderCombinations.SetupProperty(x => x.Children, new[] { combination });
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolderCombinations.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetLoginTypes(offer);

            Assert.Equal(result.Count(), loginTypes.Count);
            Assert.Contains(loginTypes[0], result);
            Assert.Contains(loginTypes[1], result);
            Assert.Contains(loginTypes[2], result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetLoginTypes_Returns_1_Preseleted_When_Random_True(int preselectedCount)
        {
            var process = "XYZ";
            var processType = "123";
            var loginTypes = new List<ILoginTypeModel>();

            for (int i = 0; i < preselectedCount; i++)
            {
                var mockLoginType = new Mock<ILoginTypeModel>();
                mockLoginType.SetupProperty(x => x.Name, $"Login type {i}");
                loginTypes.Add(mockLoginType.Object);
            }

            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;

            var mockProcess = new Mock<IProcessModel>();
            mockProcess.SetupProperty(x => x.Code, process);
            var mockProcessType = new Mock<IProcessTypeModel>();
            mockProcessType.SetupProperty(x => x.Code, processType);
            var mockCombination = new Mock<IDefinitionCombinationModel>();
            mockCombination.SetupProperty(x => x.Process, mockProcess.Object);
            mockCombination.SetupProperty(x => x.ProcessType, mockProcessType.Object);
            mockCombination.SetupProperty(x => x.LoginTypes, loginTypes);
            mockCombination.SetupProperty(x => x.LoginTypesRandom, true);
            var combination = mockCombination.Object;
            var mockFolder = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolder.SetupProperty(x => x.Children, new[] { combination });
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolder.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetLoginTypes(offer);

            Assert.Single(result);
        }

        [Fact]
        [Trait("Method", "GetLoginTypes")]
        public void GetLoginTypes_Returns_All_When_No_Selected_And_Random_Is_False()
        {
            var process = "XYZ";
            var processType = "123";
            var loginTypes = new List<ILoginTypeModel>();
            var mockLoginType1 = new Mock<ILoginTypeModel>();
            mockLoginType1.SetupProperty(x => x.Name, "LT1");
            loginTypes.Add(mockLoginType1.Object);
            var mockLoginType2 = new Mock<ILoginTypeModel>();
            mockLoginType2.SetupProperty(x => x.Name, "LT2");
            loginTypes.Add(mockLoginType2.Object);
            var mockLoginType3 = new Mock<ILoginTypeModel>();
            mockLoginType3.SetupProperty(x => x.Name, "LT3");
            loginTypes.Add(mockLoginType3.Object);
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;

            var mockProcessModel = new Mock<IProcessModel>();
            mockProcessModel.SetupProperty(x => x.Code, process);
            var mockProcessTypeModel = new Mock<IProcessTypeModel>();
            mockProcessTypeModel.SetupProperty(x => x.Code, processType);
            var mockLoginType = new Mock<ILoginTypeModel>();
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.Process, mockProcessModel.Object);
            mockDefinition.SetupProperty(x => x.ProcessType, mockProcessTypeModel.Object);
            mockDefinition.SetupProperty(x => x.LoginTypes, Enumerable.Empty<ILoginTypeModel>());
            mockDefinition.SetupProperty(x => x.LoginTypesRandom, false);
            var definition = mockDefinition.Object;
            var mockFolderDefinitions = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolderDefinitions.SetupProperty(x => x.Children, new[] { definition });
            var mockFolderLoginTypes = new Mock<IFolderItemModel<ILoginTypeModel>>();
            mockFolderLoginTypes.SetupProperty(x => x.Children, loginTypes);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolderDefinitions.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { definition });
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<ILoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(mockFolderLoginTypes.Object);
            mockSitecoreService.Setup(x => x.GetItems<ILoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(loginTypes);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetLoginTypes(offer);

            Assert.Equal(loginTypes, result.ToList());
        }

        [Fact]
        [Trait("Method", "GetLoginTypes")]
        public void GetLoginTypes_Returns_1_When_1_Preselected()
        {
            var process = "XYZ";
            var processType = "123";

            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;

            var loginTypes = new List<ILoginTypeModel>();
            var mockLoginType = new Mock<ILoginTypeModel>();
            mockLoginType.SetupProperty(x => x.Name, "LT1");
            loginTypes.Add(mockLoginType.Object);

            var mockProcessModel = new Mock<IProcessModel>();
            mockProcessModel.SetupProperty(x => x.Code, process);
            var mockProcessTypeModel = new Mock<IProcessTypeModel>();
            mockProcessTypeModel.SetupProperty(x => x.Code, processType);
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.Process, mockProcessModel.Object);
            mockDefinition.SetupProperty(x => x.ProcessType, mockProcessTypeModel.Object);
            mockDefinition.SetupProperty(x => x.LoginTypes, Enumerable.Empty<ILoginTypeModel>());
            var definition = mockDefinition.Object;
            var mockFolderDefinitions = new Mock<IFolderItemModel<IDefinitionCombinationModel>>();
            mockFolderDefinitions.SetupProperty(x => x.Children, new[] { definition });
            var mockFolderLoginTypes = new Mock<IFolderItemModel<ILoginTypeModel>>();
            mockFolderLoginTypes.SetupProperty(x => x.Children, loginTypes);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(mockFolderDefinitions.Object);
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { definition });
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<ILoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(mockFolderLoginTypes.Object);
            mockSitecoreService.Setup(x => x.GetItems<ILoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(loginTypes);
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetLoginTypes(offer);

            Assert.Single(result);
            Assert.Equal(result.First(), loginTypes.First());
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_Login_Url()
        {
            var expected = "/login";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.Login, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.Login);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_Offer_Url()
        {
            var expected = "/offer";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.Offer, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.Offer);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_OfferExpired_Url()
        {
            var expected = "/expired-offer";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.ExpiredOffer, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.OfferExpired);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_AcceptedOffer_Url()
        {
            var expected = "/accepted-offer";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.AcceptedOffer, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.AcceptedOffer);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_SessionExpired_Url()
        {
            var expected = "/session-expired";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.SessionExpired, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.SessionExpired);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_SystemError_Url()
        {
            var expected = "/system-error";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.SystemError, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.SystemError);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_ThankYou_Url()
        {
            var expected = "/thank-you";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.ThankYou, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.ThankYou);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_UserBlocked_Url()
        {
            var expected = "/user-blocked";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.UserBlocked, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.UserBlocked);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetPageLink")]
        public void GetPageLink_Returns_WrongUrl_Url()
        {
            var expected = "/error";
            var siteRoot = "/site";
            var link = new Link();
            link.Url = expected;
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupProperty(x => x.WrongUrl, link);
            var siteSettings = mockSiteSettings.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<ISiteSettingsModel>(siteRoot)).Returns(siteSettings);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetSiteRoot()).Returns(siteRoot);
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetPageLink(PAGE_LINK_TYPES.WrongUrl);

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Method", "GetSteps")]
        public void GetSteps_Returns_Concurrent_Steps_In_Folder()
        {
            var mockStepB = new Mock<IStepModel>();
            mockStepB.SetupProperty(x => x.Name, "B");
            mockStepB.SetupProperty(x => x.Path, "/sitecore/content/eCon/Settings/Steps/Step2");
            var currentStep = mockStepB.Object;
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));

            var steps = new List<IStepModel>();
            var mockStepA = new Mock<IStepModel>();
            mockStepA.SetupProperty(x => x.Name, "A");
            steps.Add(mockStepA.Object);
            var mockStepC = new Mock<IStepModel>();
            mockStepC.SetupProperty(x => x.Name, "C");
            steps.Add(mockStepC.Object);
            var mockFolderSteps = new Mock<IFolderItemModel<IStepModel>>();
            mockFolderSteps.SetupProperty(x => x.Children, steps);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IStepModel>>(parentPath)).Returns(mockFolderSteps.Object);
            mockSitecoreService.Setup(x => x.GetItems<IStepModel>(parentPath)).Returns(steps);
            var logger = new MemoryLogger();

            var mockContextWrapper = new Mock<IContextWrapper>();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetSteps(currentStep);

            Assert.Equal(steps, result);

            for (int i = 0; i < steps.Count; i++)
            {
                Assert.Equal(steps[i], result[i]);
            }
        }

        [Fact]
        [Trait("Method", "GetSteps")]
        public void GetSteps_Returns_Current_Step_As_Selected()
        {
            var mockStepB = new Mock<IStepModel>();
            mockStepB.SetupProperty(x => x.ID, Guid.NewGuid());
            mockStepB.SetupProperty(x => x.Name, "B");
            mockStepB.SetupProperty(x => x.Path, "/sitecore/content/eCon/Settings/Steps/Step2");
            mockStepB.SetupProperty(x => x.IsSelected, false);
            //mockStepB.SetupSet(x => x.IsSelected = true);
            var currentStep = mockStepB.Object;
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));

            var mockStepA = new Mock<IStepModel>();
            mockStepA.SetupProperty(x => x.ID, Guid.NewGuid());
            mockStepA.SetupProperty(x => x.Name, "A");
            var mockStepC = new Mock<IStepModel>();
            mockStepC.SetupProperty(x => x.ID, Guid.NewGuid());
            mockStepC.SetupProperty(x => x.Name, "C");

            var steps = new List<IStepModel>();
            steps.Add(mockStepA.Object);
            steps.Add(currentStep);
            steps.Add(mockStepC.Object);

            var mockFolderSteps = new Mock<IFolderItemModel<IStepModel>>();
            mockFolderSteps.SetupProperty(x => x.Children, steps);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IStepModel>>(parentPath)).Returns(mockFolderSteps.Object);
            mockSitecoreService.Setup(x => x.GetItems<IStepModel>(parentPath)).Returns(steps);
            var logger = new MemoryLogger();

            var mockContextWrapper = new Mock<IContextWrapper>();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetSteps(currentStep);

            Assert.Equal(steps, result);

            Assert.False(result[0].IsSelected);
            Assert.True(result[1].IsSelected);
            Assert.False(result[2].IsSelected);
        }

        [Theory]
        [InlineData("BENEFITS_NOW_INTRO")]
        [InlineData("BENEFITS_NEXT_SIGN_INTRO")]
        [InlineData("BENEFITS_NEXT_TZD_INTRO")]
        public void GetXmlNodeNamesExcludeHtml_Return_Correct_Values(string expected)
        {
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            var result = service.GetXmlNodeNamesExcludeHtml();

            Assert.Contains(result, x => x == expected);
        }
    }
}
