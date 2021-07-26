using System;
using System.Collections.Generic;
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
            mockSitecoreService.Setup(x => x.GetItem<TestSiteSettingsModel>(siteRoot)).Returns((TestSiteSettingsModel)null);
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
            mockSitecoreService.Setup(x => x.GetItem<MemoryFolderItemModel<MemoryLoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES, false, false)).Returns(new MemoryFolderItemModel<MemoryLoginTypeModel>());
            mockSitecoreService.Setup(x => x.GetItems<MemoryLoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<MemoryLoginTypeModel>());
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
            mockSitecoreService.Setup(x => x.GetItem<MemoryFolderItemModel<MemoryProcessModel>>(Constants.SitecorePaths.PROCESSES)).Returns(new MemoryFolderItemModel<MemoryProcessModel>());
            mockSitecoreService.Setup(x => x.GetItems<MemoryProcessModel>(Constants.SitecorePaths.PROCESSES)).Returns(Enumerable.Empty<MemoryProcessModel>());
            var mockContextWrapper = new Mock<IContextWrapper>();
            var logger = new MemoryLogger();

            var service = new SitecoreSettingsReaderService(mockSitecoreService.Object, mockContextWrapper.Object, logger);
            Assert.Throws<EcontractingMissingDatasourceException>(() => service.GetAllProcesses());
        }

        [Fact]
        [Trait("Method", "GetAllProcesses")]
        public void GetAllProcesses_Returns_Items()
        {
            var list = new List<MemoryProcessModel>();
            list.Add(new Mock<MemoryProcessModel>().Object);
            list.Add(new Mock<MemoryProcessModel>().Object);
            list.Add(new Mock<MemoryProcessModel>().Object);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IProcessModel>>(Constants.SitecorePaths.PROCESSES)).Returns(new MemoryFolderItemModel<IProcessModel>(list));
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
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<ILoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(new MemoryFolderItemModel<ILoginTypeModel>());
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
            var list = new List<MemoryProcessTypeModel>();
            list.Add(new Mock<MemoryProcessTypeModel>().Object);
            list.Add(new Mock<MemoryProcessTypeModel>().Object);
            list.Add(new Mock<MemoryProcessTypeModel>().Object);
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IProcessTypeModel>>(Constants.SitecorePaths.PROCESS_TYPES)).Returns(new MemoryFolderItemModel<IProcessTypeModel>(list));
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.ServiceUrl = "";
            siteSettings.ServiceUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(user));
            siteSettings.ServicePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.ServiceUrl = url;
            siteSettings.ServiceUser = "";
            siteSettings.ServicePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.ServiceUrl = url;
            siteSettings.ServiceUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(user));
            siteSettings.ServicePassword = "";

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

            var siteSettings = new TestSiteSettingsModel();

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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.ServiceUrl = url;
            siteSettings.ServiceUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(user));
            siteSettings.ServicePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
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
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = process };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = processType };
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
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
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = "ABCDED" };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = "09988" };
            var defaultCombination = new MemoryDefinitionCombinationModel();

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
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
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = "XYZ" };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = "123" };

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
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
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = "XYZ" };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = "123" };

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
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
            var loginTypes = new List<MemoryLoginTypeModel>();
            loginTypes.Add(new MemoryLoginTypeModel() { Name = "LT1" });
            loginTypes.Add(new MemoryLoginTypeModel() { Name = "LT2" });
            loginTypes.Add(new MemoryLoginTypeModel() { Name = "LT3" });
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = process };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = processType };
            combination.LoginTypes = loginTypes;
            combination.LoginTypesRandom = false;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
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
            var loginTypes = new List<MemoryLoginTypeModel>();

            for (int i = 0; i < preselectedCount; i++)
            {
                loginTypes.Add(new MemoryLoginTypeModel() { Name = $"Login type {i}" });
            }

            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = process };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = processType };
            combination.LoginTypes = loginTypes;
            combination.LoginTypesRandom = true; // this cause randomization

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
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
            var loginTypes = new List<MemoryLoginTypeModel>();
            loginTypes.Add(new MemoryLoginTypeModel() { Name = "LT1" });
            loginTypes.Add(new MemoryLoginTypeModel() { Name = "LT2" });
            loginTypes.Add(new MemoryLoginTypeModel() { Name = "LT3" });
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = process };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = processType };
            combination.LoginTypes = Enumerable.Empty<MemoryLoginTypeModel>();
            combination.LoginTypesRandom = false;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<ILoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(new MemoryFolderItemModel<ILoginTypeModel>(loginTypes));
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
            var loginTypes = new List<MemoryLoginTypeModel>();
            loginTypes.Add(new MemoryLoginTypeModel() { Name = "LT1" });
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.BusProcess = process;
            offer.Xml.Content.Body.BusProcessType = processType;
            var combination = new MemoryDefinitionCombinationModel();
            combination.Process = new MemoryProcessModel() { Code = process };
            combination.ProcessType = new MemoryProcessTypeModel() { Code = processType };
            combination.LoginTypes = new MemoryLoginTypeModel[] { };
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IDefinitionCombinationModel>>(Constants.SitecorePaths.DEFINITIONS)).Returns(new MemoryFolderItemModel<IDefinitionCombinationModel>(new[] { combination }));
            mockSitecoreService.Setup(x => x.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS)).Returns(new[] { combination });
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<ILoginTypeModel>>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(new MemoryFolderItemModel<ILoginTypeModel>(loginTypes));
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.Login = new Link();
            siteSettings.Login.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.Offer = new Link();
            siteSettings.Offer.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.ExpiredOffer = new Link();
            siteSettings.ExpiredOffer.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.AcceptedOffer = new Link();
            siteSettings.AcceptedOffer.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.SessionExpired = new Link();
            siteSettings.SessionExpired.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.SystemError = new Link();
            siteSettings.SystemError.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.ThankYou = new Link();
            siteSettings.ThankYou.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.UserBlocked = new Link();
            siteSettings.UserBlocked.Url = expected;
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
            var siteSettings = new TestSiteSettingsModel();
            siteSettings.WrongUrl = new Link();
            siteSettings.WrongUrl.Url = expected;
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
            var currentStep = new MemoryProcessStepModel();
            currentStep.Name = "B";
            currentStep.Path = "/sitecore/content/eCon/Settings/Steps/Step2";
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));

            var steps = new List<MemoryProcessStepModel>();
            steps.Add(new MemoryProcessStepModel() { Name = "A" });
            steps.Add(currentStep);
            steps.Add(new MemoryProcessStepModel() { Name = "C" });

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IProcessStepModel>>(parentPath)).Returns(new MemoryFolderItemModel<IProcessStepModel>(steps));
            mockSitecoreService.Setup(x => x.GetItems<IProcessStepModel>(parentPath)).Returns(steps);
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
            var currentStep = new MemoryProcessStepModel();
            currentStep.ID = Guid.NewGuid();
            currentStep.Name = "B";
            currentStep.Path = "/sitecore/content/eCon/Settings/Steps/Step2";
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));

            var steps = new List<MemoryProcessStepModel>();
            steps.Add(new MemoryProcessStepModel() { ID = Guid.NewGuid(), Name = "A" });
            steps.Add(currentStep);
            steps.Add(new MemoryProcessStepModel() { ID = Guid.NewGuid(), Name = "C" });

            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IFolderItemModel<IProcessStepModel>>(parentPath)).Returns(new MemoryFolderItemModel<IProcessStepModel>(steps));
            mockSitecoreService.Setup(x => x.GetItems<IProcessStepModel>(parentPath)).Returns(steps);
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
