using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;

namespace eContracting
{
    public interface IOfferDataService
    {
        ResponseCacheGetModel GetResponse(string guid, OFFER_TYPES type, string fileType = "B");

        ResponseStatusSetModel SetStatus(string guid, OFFER_TYPES type, decimal timestamp, string status);

        ResponseAccessCheckModel UserAccessCheck(string guid, string userId, OFFER_TYPES type);

        ResponsePutModel Put(string guid, ZCCH_ST_ATTRIB[] attributes, OfferFileXmlModel[] files);
    }
}
