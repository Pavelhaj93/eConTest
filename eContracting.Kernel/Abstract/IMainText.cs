using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Kernel.Abstract
{
    public interface IMainText
    {
        string MainTextIndividual { get; }
        
        string MainTextRetentionIndividual{ get; }
        
        string MainTextAcquisitionIndividual { get; }
        
        string MainTextCampaign { get; }
        
        string MainTextRetentionCampaign { get; }
        
        string MainTextAcquisitionCampaign { get; }
    }
}
