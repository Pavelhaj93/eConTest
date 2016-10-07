using System;
using System.Globalization;

namespace eContracting.Kernel.Utils
{
    public class AuthenticationDataItem
    {
        public String ItemType { get; set; }
        public String ItemValue { get; set; }
        public String ItemFriendlyName { get; set; }
        public String DateOfBirth { get; set; }
        public String Identifier { get; set; }
        public String LastName { get; set; }
        public Boolean IsAccountNumber { get; set; }
        public String ExpDate { get; set; }
        public DateTime ExpDateConverted
        {
            get
            {
                DateTime outputDateTimeValue;
                DateTime.TryParseExact(this.ExpDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTimeValue);
                return outputDateTimeValue;
            }
        }
        public String ExpDateFormatted
        {
            get
            {
                return this.ExpDateConverted.ToString("dd.MM.yyy");
            }
        }
    }
}
