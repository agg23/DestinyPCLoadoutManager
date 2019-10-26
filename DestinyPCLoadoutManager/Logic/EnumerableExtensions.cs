using DestinyPCLoadoutManager.API.Models;
using Fastenshtein;
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

        public static List<T> Search<T>(this IEnumerable<T> items, string searchTerm, Func<T, string> keyAction)
        {
            Levenshtein searchObject = new Levenshtein(searchTerm);

            var searchResults = items.Select(i => keyAction(i)).Select(i => searchObject.DistanceFrom(i)).Zip(items);
            return searchResults.OrderBy(i => i.First).Select(i => i.Second).ToList();
        }
    }
}
