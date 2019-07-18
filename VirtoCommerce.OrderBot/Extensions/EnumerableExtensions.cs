using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.OrderBot.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return data == null || !data.Any();
        }
    }
}
