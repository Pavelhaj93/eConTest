// <copyright file="RweClient.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

using System;
using System.Collections.Generic;
using System.Web;
using eContracting.Kernel.Models;

namespace eContracting.Kernel.Services
{
    [Obsolete("Use 'CacheApiService' instead")]
    public interface IRweClient
    {
        bool AcceptOffer(string guid);
        List<FileToBeDownloaded> GeneratePDFFiles(string guid);
        Offer GenerateXml(string guid);
        Offer GenerateXml(string guid, ZCCH_CACHE_GETResponse responseObject);
        Dictionary<string, string> GetAllAttributes(string guid, IEnumerable<XmlText> sources, IEnumerable<RweClientLoadTemplateModel> templateValues);
        Dictionary<string, string> GetAllAttributes(string guid, IEnumerable<XmlText> sources, string additionalInfoDocument);
        Dictionary<string, string> GetAllAttributes(string guid, XmlText sourceXml);
        ZCCH_ST_FILE[] GetFiles(string guid, bool IsAccepted);
        XmlText GetLetterXml(IEnumerable<XmlText> texts);
        IEnumerable<XmlText> GetTextsXml(string guid);
        bool LogAcceptance(string guid, DateTime when, HttpContextBase context, OfferTypes offerType, IEnumerable<string> acceptedDocuments);
        void ReadOffer(string guid);
        void ResetOffer(string guid);
        void SignOffer(string guid);
    }
}
