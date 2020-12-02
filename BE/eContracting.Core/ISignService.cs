using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    public interface ISignService
    {
        Task<OfferAttachmentModel> SignAsync(OfferAttachmentModel file, byte[] signature);
    }
}
