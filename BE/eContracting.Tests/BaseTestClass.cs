using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    public abstract class BaseTestClass
    {
        public OfferModel CreateOffer()
        {
            return CreateOffer(Guid.NewGuid());    
        }

        public OfferModel CreateOffer(Guid guid)
        {
            return CreateOffer(guid.ToString("N"));
        }

        public OfferModel CreateOffer(string guid)
        {
            return CreateOffer(guid, false, 2);
        }

        public OfferModel CreateOffer(string guid, bool isAccepted, int version)
        {
            return CreateOffer(guid, isAccepted, version, "3");
        }

        public OfferModel CreateOffer(string guid, bool isAccepted, int version, string state)
        {
            return CreateOffer(guid, isAccepted, version, state, "31.12.2021", null);
        }

        public OfferModel CreateOffer(string guid, bool isAccepted, int version, string state, string validTo, OfferAttributeModel[] attributes)
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            var offerHeader = new OfferHeaderModel("NABIDKA", guid, state, validTo);
            var offer = new OfferModel(offerXml, version, offerHeader, isAccepted, attributes ?? new OfferAttributeModel[] { });
            return offer;
        }
    }
}
