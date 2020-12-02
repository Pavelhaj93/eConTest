using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    public class FileSignService : ISignService
    {
        public async Task<OfferAttachmentModel> SignAsync(OfferAttachmentModel file, byte[] signature)
        {
            throw new NotImplementedException();
        }
    }
}
