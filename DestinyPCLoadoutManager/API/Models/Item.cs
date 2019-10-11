using Destiny2;
using Destiny2.Entities.Items;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API.Models
{
    public class Item
    {
        public static async Task<Item> BuildItem(DestinyItemComponent itemComponent)
        {
            var manifest = App.provider.GetService(typeof(IManifest)) as IManifest;

            var item = await manifest.LoadInventoryItem(itemComponent.ItemHash);

            return new Item(item.DisplayProperties.Name, item.Equippable);
        }

        public string Name;
        public bool Equippable;

        public Item(string name, bool equippable)
        {
            Name = name;
            Equippable = equippable;
        }
    }
}
