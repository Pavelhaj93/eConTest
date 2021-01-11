﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using eContracting.Website.Pipelines.Initialize;
using Xunit;

namespace eContracting.Website.Tests.Pipelines.Initialize
{
    public class RegisterApiProcessorTests
    {
        [Fact]
        public void RegisterApiRoutes_Register_Api_Route()
        {
            RouteTable.Routes.Clear();

            var processor = new RegisterApiProcessor();
            processor.RegisterApiRoutes();

            var r = RouteTable.Routes.First() as Route;
            Assert.Equal("api/econ/{action}/{id}", r.Url);
        }
    }
}
