using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using eContracting.Services;
using eContracting.Website.Areas.eContracting2.Controllers;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace eContracting.Website
{
    [ExcludeFromCodeCoverage]
    public class eContractingServiceConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ILogger, SitecoreLogger>();
            serviceCollection.AddScoped<IContextWrapper, SitecoreContextWrapper>();
            serviceCollection.AddScoped<IUserDataCacheService, UserSessionDataCacheService>();
            serviceCollection.AddScoped<IUserFileCacheService, DbUserFileCacheService>();
            serviceCollection.AddScoped<IOfferService, OfferService>();
            serviceCollection.AddScoped<IServiceFactory, ServiceFactory>();
            serviceCollection.AddScoped<IOfferParserService, OfferParserService>();
            serviceCollection.AddScoped<IOfferAttachmentParserService, OfferAttachmentParserService>();
            serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
            serviceCollection.AddScoped<ISettingsReaderService, SitecoreSettingsReaderService>();
            serviceCollection.AddScoped<ILoginFailedAttemptBlockerStore, DbLoginFailedAttemptBlockerStore>();
            serviceCollection.AddScoped<IOfferJsonDescriptor, OfferJsonDescriptor>();
            serviceCollection.AddScoped<ISignService, FileSignService>();
            serviceCollection.AddScoped<IFileOptimizer, FileOptimizer>();
            serviceCollection.AddScoped<IEventLogger, DbEventLogger>();
            serviceCollection.AddScoped<ITextService, SitecoreTextService>();
            serviceCollection.AddScoped<ISessionProvider, SessionProvider>();
            serviceCollection.AddScoped<IStartup, DbContextStartup>();
            serviceCollection.AddScoped<ISitecoreContext, SitecoreContext>();

            serviceCollection.AddScoped<eContracting2AuthController>();
            serviceCollection.AddScoped<eContracting2ApiController>();
            serviceCollection.AddScoped<eContracting2Controller>();
        }
    }
}
