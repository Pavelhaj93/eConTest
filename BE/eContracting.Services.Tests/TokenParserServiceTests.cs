using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace eContracting.Services.Tests
{
    public class TokenParserServiceTests
    {
        [Fact]
        public void GetJwtToken_Returns_Null_When_AccessToken_Empty()
        {
            var service = new TokenParserService();
            
            var result = service.GetJwtToken("");

            Assert.Null(result);
        }
    }
}
