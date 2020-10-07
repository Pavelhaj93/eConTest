using System;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using eContracting.Kernel.GlassItems;
using eContracting.Kernel.Helpers;
using Sitecore.Diagnostics;

namespace eContracting.Kernel.Services
{
    /// <summary>
    /// Client encapsulation of the signing service.
    /// </summary>
    public class SigningClient
    {
        public FileToBeDownloaded SendDocumentsForMerge(FileToBeDownloaded pdfFile, byte[] signFile)
        {
            using (var api = this.InitApi())
            {
                var invokeParameters = new invoke();

                var inputPDFBlob = new BLOB() { binaryData = pdfFile.FileContent.ToArray(), contentType = "application/pdf", };
                var inputSignBlob = new BLOB() { binaryData = signFile, contentType = "image/png" };

                invokeParameters.inputPDF = inputPDFBlob;
                invokeParameters.inputPNGSign = inputSignBlob;
                invokeParameters.overlay = pdfFile.TemplAlcId;

                var signingResult = api.invoke(invokeParameters);
                if (signingResult.errCode != 0)
                {
                    Log.Error("Error occured during sending document for signing. Error message is " + signingResult.errMsg, this);
                    return null;
                }

                return new FileToBeDownloaded() { FileContent = signingResult.outputPDF.binaryData.ToList(), FileName = pdfFile.FileName, FileNumber = pdfFile.FileNumber, FileType = pdfFile.FileType, Index = pdfFile.Index, SignRequired = pdfFile.SignRequired, SignedVersion = true, TemplAlcId = pdfFile.TemplAlcId };
            }
        }

        /// <summary>
        /// Initializes the API of the signing service.
        /// </summary>
        /// <returns>Instance of the service.</returns>
        private CRM_SIGN_STAMP_MERGE InitApi()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            CRM_SIGN_STAMP_MERGE api = new CRM_SIGN_STAMP_MERGE();

            SiteRootModel siteSettings = ConfigHelpers.GetSiteSettings();
            var userName = Encoding.UTF8.GetString(Convert.FromBase64String(siteSettings.SigningServiceUser));
            var password = Encoding.UTF8.GetString(Convert.FromBase64String(siteSettings.SigningServicePassword));

            api.Url = siteSettings.SigningServiceUrl;

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new InvalidCredentialException("Wrong credentials for cache!");
            }

            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(
                new Uri(api.Url),
                "Basic",
                new NetworkCredential(userName, password)
            );

            api.Credentials = credentialCache;
            return api;
        }
    }
}
