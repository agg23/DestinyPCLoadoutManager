using DestinyPCLoadoutManager.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DestinyPCLoadoutManager.API.Models
{
    [Serializable]
    public class Loadout
    {
        public string Name { get; set; }
        public Shortcut Shortcut { get; set; }
        public List<Item> EquippedItems { get; set; }
        public List<Item> InventoryItems { get; set; }

        public Loadout()
        {
            EquippedItems = new List<Item>();
            InventoryItems = new List<Item>();
        }

        public Loadout Difference(Loadout additional)
        {
            return new Loadout
            {
                EquippedItems = this.EquippedItems.Difference(additional.EquippedItems, item => item.Id).ToList(),
                InventoryItems = this.InventoryItems.Difference(additional.InventoryItems, item => item.Id).ToList()
            };
        }
    }
}
