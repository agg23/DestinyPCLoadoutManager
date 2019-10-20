using DestinyPCLoadoutManager.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DestinyPCLoadoutManager.Logic
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Difference<T, TKey>(this IEnumerable<T> a, IEnumerable<T> b, Func<T, TKey> keyAction)
        {
            var subtractIdSet = b.Select(keyAction).ToHashSet();

            return a.Where(item => !subtractIdSet.Contains(keyAction(item)));
        }
    }
}
