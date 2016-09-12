using System.Collections.Generic;
using System.Linq;

namespace rweClient
{
    public static class EnumExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> en)
        {
            return en == null || !en.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> en)
        {
            return !en.IsNullOrEmpty<T>();
        }
    }

    public static class ZCCH_CACHE_GETResponseExtensions
    {
        public static bool ThereAreFiles(this ZCCH_CACHE_GETResponse res)
        {
            return res != null && res.ET_FILES.IsNotNullOrEmpty();
        }
    }
}
