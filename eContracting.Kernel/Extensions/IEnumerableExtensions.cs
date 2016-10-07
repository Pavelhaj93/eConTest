using System.Linq;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
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
}
