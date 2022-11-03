using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
using eContracting.Website.Rules.Conditions;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Builders;
using Moq;
using Sitecore.Rules;
using Xunit;

namespace eContracting.Website.Tests.Rules.Conditions
{
    public class WhenOfferHasSpecificProcessConditionTests : BaseTestClass
    {
        public class WhenOfferHasSpecificProcessConditionImpl<T> : WhenOfferHasSpecificProcessCondition<T> where T : RuleContext
        {
            public Guid ProcessItemGuidPublic
            {
                get => this.ProcessItemGuid;
                set { this.ProcessItemGuid = value; }
            }

            public WhenOfferHasSpecificProcessConditionImpl(IDataRequestCacheService cacheService, ISitecoreService sitecoreService, IContextWrapper contextWrapper) : base(cacheService, sitecoreService, contextWrapper)
            {
            }

            public bool ExecutePublic(T ruleContext)
            {
                return base.Execute(ruleContext);
            }
        }

        [Fact]
        public void Execute_Returns_False_When_ProcessItemGuid_Is_Empty()
        {
            var mockRequestCache = new Mock<IDataRequestCacheService>();
            var mockSitecoreService = new Mock<ISitecoreService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            var ruleContext = new RuleContext();

            var rule = new WhenOfferHasSpecificProcessConditionImpl<RuleContext>(
                mockRequestCache.Object,
                mockSitecoreService.Object,
                mockContextWrapper.Object);
            rule.ProcessId = Guid.Empty.ToString("N").ToUpperInvariant();
            var result = rule.ExecutePublic(ruleContext);

            Assert.False(result);
        }

        [Fact]
        public void Execute_Returns_False_When_Offer_Null()
        {
            var guid = this.CreateGuid();
            var offer = this.CreateOffer(guid);
            var mockRequestCache = new Mock<IDataRequestCacheService>();
            mockRequestCache.Setup(x => x.GetOffer(guid)).Returns((OfferCacheDataModel)null);
            var mockSitecoreService = new Mock<ISitecoreService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetQueryValue(Constants.QueryKeys.GUID)).Returns(guid);
            var ruleContext = new RuleContext();

            var rule = new WhenOfferHasSpecificProcessConditionImpl<RuleContext>(
                mockRequestCache.Object,
                mockSitecoreService.Object,
                mockContextWrapper.Object);
            rule.ProcessId = this.CreateGuid();
            var result = rule.ExecutePublic(ruleContext);

            Assert.False(result);
        }

        //[Fact]
        public void Execute_Returns_False_When_Process_Doesnt_Exist()
        {
            var guid = this.CreateGuid();
            var processId = this.CreateGuid();
            var processGuid = new Guid(processId);
            var offer = this.CreateOffer(guid);
            var processItem = new Mock<IProcessModel>().Object;
            var cachedOffer = new OfferCacheDataModel(offer);
            var ruleContext = new RuleContext();
            ruleContext.Item = this.CreateItem(new Sitecore.Data.ID(processId));

            var mockRequestCache = new Mock<IDataRequestCacheService>();
            mockRequestCache.Setup(x => x.GetOffer(guid)).Returns(cachedOffer);
            var mockSitecoreService = new Mock<ISitecoreService>();
            mockSitecoreService.Setup(x => x.GetItem<IProcessModel>(processGuid)).Returns(processItem);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetQueryValue(Constants.QueryKeys.GUID)).Returns(guid);

            var rule = new WhenOfferHasSpecificProcessConditionImpl<RuleContext>(
                mockRequestCache.Object,
                mockSitecoreService.Object,
                mockContextWrapper.Object);
            rule.ProcessItemGuidPublic = processGuid;
            var result = rule.ExecutePublic(ruleContext);

            Assert.False(result);
        }

        //[Fact]
        public void Execute_Returns_False_When_Process_Doesnt_Match_With_Offer()
        {
            var guid = this.CreateGuid();
            var processId = this.CreateGuid();
            var processGuid = new Guid(processId);
            var offer = this.CreateOffer(guid);
            offer.Xml.Content.Body.BusProcess = "XYZ";
            var cachedOffer = new OfferCacheDataModel(offer);
            var processItem = new Mock<IProcessModel>().Object;
            processItem.Code = "INVALID";
            var ruleContext = new RuleContext();
            ruleContext.Item = this.CreateItem(new Sitecore.Data.ID(processId));
            Action<GetItemByIdBuilder> action = (builder) => { };

            var mockRequestCache = new Mock<IDataRequestCacheService>();
            mockRequestCache.Setup(x => x.GetOffer(guid)).Returns(cachedOffer);
            var mockSitecoreService = new Mock<ISitecoreService>();
            mockSitecoreService.Setup(x => x.GetItem<IProcessModel>(processGuid, action)).Returns(processItem);
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetQueryValue(Constants.QueryKeys.GUID)).Returns(guid);

            var rule = new WhenOfferHasSpecificProcessConditionImpl<RuleContext>(
                mockRequestCache.Object,
                mockSitecoreService.Object,
                mockContextWrapper.Object);
            rule.ProcessItemGuidPublic = processGuid;
            var result = rule.ExecutePublic(ruleContext);

            Assert.False(result);
        }
    }
}
