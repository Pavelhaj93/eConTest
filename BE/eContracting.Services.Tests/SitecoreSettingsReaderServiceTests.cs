using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Maps;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Fluent;
using Moq;
using Moq.Language.Flow;
using Xunit;

namespace eContracting.Services.Tests
{
    [ExcludeFromCodeCoverage]
    public class SitecoreSettingsReaderServiceTests
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
        public void GetAllLoginTypes_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<LoginTypeModel>());

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object);
            Assert.Throws<MissingDatasourceException>(() => service.GetAllLoginTypes());
        }

        [Fact]
        public void GetAllProcesses_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<LoginTypeModel>());

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object);
            Assert.Throws<MissingDatasourceException>(() => service.GetAllProcesses());
        }

        [Fact]
        public void GetAllProcessTypes_Throws_MissingDatasourceException_When_No_Items_Found()
        {
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            mockSitecoreContext.Setup(x => x.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES)).Returns(Enumerable.Empty<LoginTypeModel>());

            var service = new SitecoreSettingsReaderService(mockSitecoreContext.Object);
            Assert.Throws<MissingDatasourceException>(() => service.GetAllProcessTypes());
        }
    }
}
