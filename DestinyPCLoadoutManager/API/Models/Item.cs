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

            return new Item(itemComponent.ItemHash, item.DisplayProperties.Name, item.Equippable);
        }

        public long Id;
        public string Name;
        public bool Equippable;

        public Item(long id, string name, bool equippable)
        {
            Id = id;
            Name = name;
            Equippable = equippable;
        }
    }
}
