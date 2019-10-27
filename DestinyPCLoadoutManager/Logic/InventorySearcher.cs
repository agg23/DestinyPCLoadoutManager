using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Logic.Search;
using System.Collections.Generic;
using System.Linq;

namespace DestinyPCLoadoutManager.Logic
{
    public class InventorySearcher
    {
        private Dictionary<long, Item> storedItems = new Dictionary<long, Item>();

        public void Index(Inventory inventory)
        {
            var items = inventory.EquippedItems.Union(inventory.InventoryItems);

            foreach (var item in items)
            {
                storedItems.TryAdd(item.Id, item);
            }
        }

        public IEnumerable<Item> Search(string searchTerm)
        {
            var searchScorer = new SearchScorer(searchTerm);

            var items = storedItems.Values;
            var searchResults = items.Select(searchScorer.Score).Zip(items);
            return searchResults.OrderByDescending(i => i.First).Select(i => i.Second).ToList();
        }
    }
}
