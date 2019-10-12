using Destiny2;
using Destiny2.Entities.Items;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API.Models
{
    [Serializable]
    public class Item
    {
        public static async Task<Item> BuildItem(DestinyItemComponent itemComponent)
        {
            var manifest = App.provider.GetService(typeof(IManifest)) as IManifest;

            var item = await manifest.LoadInventoryItem(itemComponent.ItemHash);

            return new Item(itemComponent.ItemInstanceId, itemComponent.ItemHash, item.DisplayProperties.Name, item.Equippable, item.Inventory.TierType);
        }

        public long Id;
        public long Hash;
        public string Name;
        public bool Equippable;
        public TierType Tier;

        public Item(long id, long hash, string name, bool equippable, TierType tier)
        {
            Id = id;
            Hash = hash;
            Name = name;
            Equippable = equippable;
            Tier = tier;
        }
    }
}
