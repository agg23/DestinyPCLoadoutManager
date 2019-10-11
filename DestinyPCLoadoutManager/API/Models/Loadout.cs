using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DestinyPCLoadoutManager.API.Models
{
    class Loadout
    {
        public IEnumerable<Item> EquippedItems { get; set; }

        public Loadout Difference(Loadout additional)
        {
            return new Loadout
            {
                EquippedItems = Difference(this.EquippedItems, additional.EquippedItems)
            };
        }

        private IEnumerable<Item> Difference(IEnumerable<Item> a, IEnumerable<Item> b)
        {
            var subtractIdSet = b.Select(item => item.Id).ToHashSet();

            return a.Where(item => !subtractIdSet.Contains(item.Id));
        }
    }
}
