using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using eContracting.Tests;
using Moq;
using Xunit;

namespace eContracting.Core.Tests
{
    public class ServiceFactoryTests
    {
        [Fact(DisplayName = "Return HTTPS binding based on input url")]
        [Trait("ServiceFactory", "GetBinding")]
        public void GetBinding_Returns_Https()
        {
            var options = new MemoryApiServiceOptions("https://localhost", "dXNlcm5hbWU=", "cGFzc3dvcmQ=");

            var factory = new ServiceFactory();
            var result = factory.GetBinding(options, "mybinding");

            Assert.IsType<BasicHttpsBinding>(result);
        }

        [Fact(DisplayName = "Return HTTP binding based on input url")]
        [Trait("ServiceFactory", "GetBinding")]
        public void GetBinding_Returns_Http()
        {
            var options = new MemoryApiServiceOptions("http://localhost", "dXNlcm5hbWU=", "cGFzc3dvcmQ=");

            var factory = new ServiceFactory();
            var result = factory.GetBinding(options, "mybinding");

            Assert.IsType<BasicHttpBinding>(result);
        }

        [Fact]
        [Trait("ServiceFactory", "GetBinding")]
        public void GetBinding_Returns_Correct_Name_Of_Binding()
        {
            var expected = "mybinding";
            var options = new MemoryApiServiceOptions("https://localhost", "dXNlcm5hbWU=", "cGFzc3dvcmQ=");

            var factory = new ServiceFactory();
            var result = factory.GetBinding(options, expected);

            Assert.Equal(expected, result.Name);
        }
    }
}
