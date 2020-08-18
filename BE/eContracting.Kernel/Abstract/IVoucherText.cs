
namespace eContracting.Kernel.Abstract
{
    public interface IVoucherText
    {
        string VoucherText { get; }
        
        string RetentionVoucherText { get; }
        
        string AcquistionVoucherText { get; }
    }
}
