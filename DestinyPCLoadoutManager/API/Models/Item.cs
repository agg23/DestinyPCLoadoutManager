using Destiny;
using Destiny2;
using Destiny2.Definitions;
using Destiny2.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var categories = await manifest.LoadItemCategories(item.ItemCategoryHashes);

            return new Item(itemComponent.ItemInstanceId, itemComponent.ItemHash, item.DisplayProperties.Name, item.Equippable, item.Inventory.TierType, BuildType(categories), BuildSubType(categories));
        }

        public static DestinyItemType BuildType(IEnumerable<DestinyItemCategoryDefinition> categories)
        {
            return categories.Select(c => c.GrantDestinyItemType).Where(c => c == DestinyItemType.Armor || c == DestinyItemType.Weapon).FirstOrDefault();
        }

        public static DestinyItemSubType BuildSubType(IEnumerable<DestinyItemCategoryDefinition> categories)
        {
            return categories.Select(c => c.GrantDestinySubType).Where(c => c != DestinyItemSubType.None).FirstOrDefault();
        }

        public long Id;
        public long Hash;
        public string Name;
        public bool Equippable;
        public TierType Tier;
        public DestinyItemType Type;
        public DestinyItemSubType SubType;

        public Item(long id, long hash, string name, bool equippable, TierType tier, DestinyItemType type, DestinyItemSubType subType)
        {
            Id = id;
            Hash = hash;
            Name = name;
            Equippable = equippable;
            Tier = tier;
            Type = type;
            SubType = subType;
        }
    }
}
