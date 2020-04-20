
namespace eContracting.Kernel.Helpers
{
    public static class CommodityHelper
    {
        private static readonly string extUiElectro = "8591824";
        public static CommodityTypes CommodityTypeByExtUi(string extUiValue)
        {
            if (string.IsNullOrEmpty(extUiValue))
            {
                return CommodityTypes.NotDefined;
            }

            if (extUiValue.StartsWith(extUiElectro))
            {
                return CommodityTypes.Electricity;
            }

            return CommodityTypes.Gas;
        }
    }
}
