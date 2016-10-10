using System;
using System.Globalization;

namespace eContracting.Kernel.Utils
{
    [Serializable]
    public class AuthenticationDataItem
    {
        public string ItemType { get; set; }
        public string ItemValue { get; set; }
        public string ItemFriendlyName { get; set; }
        public string DateOfBirth { get; set; }
        public string Identifier { get; set; }
        public string LastName { get; set; }
        public bool IsAccountNumber { get; set; }
        public string ExpDate { get; set; }
        public DateTime ExpDateConverted
        {
            get
            {
                DateTime outputDateTimeValue;
                DateTime.TryParseExact(this.ExpDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTimeValue);
                return outputDateTimeValue;
            }
        }
        public string ExpDateFormatted
        {
            get
            {
                return this.ExpDateConverted.ToString("dd.MM.yyy");
            }
        }
    }
}
