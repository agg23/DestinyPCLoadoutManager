using Destiny2;
using Destiny2.Entities.Items;
using Destiny2.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API.Models
{
    public class Inventory
    {
        public static async Task<Inventory> BuildCharacterInventory(DestinyCharacterResponse character)
        {
            if (character.Inventory == null || character.Inventory.Data == null || character.Equipment == null || character.Equipment.Data == null)
            {
                return null;
            }

            var manifest = App.provider.GetService(typeof(IManifest)) as IManifest;
            //var classTask = manifest.LoadClass(character.Inventory.Data);

            var equippedItems = await ParseItems(character.Equipment.Data.Items, manifest);
            var inventoryItems = await ParseItems(character.Inventory.Data.Items, manifest);

            return new Inventory(equippedItems, inventoryItems, null);
        }

        public static async Task<Inventory> BuildVaultInventory(DestinyCharacterResponse character)
        {
            if (character.Inventory == null || character.Inventory.Data == null)
            {
                return null;
            }

            var manifest = App.provider.GetService(typeof(IManifest)) as IManifest;
            //var classTask = manifest.LoadClass(character.Inventory.Data);

            var vaultItems = await ParseItems(character.Equipment.Data.Items, manifest);

            return new Inventory(null, null, vaultItems);
        }

        private static async Task<IEnumerable<Item>> ParseItems(IEnumerable<DestinyItemComponent> unparsedItems, IManifest manifest)
        {
            var items = await Task.WhenAll((await Task.WhenAll(unparsedItems.Select(async item =>
            {
                var bucket = await manifest.LoadBucket(item.BucketHash);
                if (bucket.Category != BucketCategory.Equippable)
                {
                    return null;
                }

                return item;
            }))).Where(i => i != null).Select(item => Item.BuildItem(item)));

            return items.Where(i => i.Type == DestinyItemType.Armor || i.Type == DestinyItemType.Weapon);
        }

        public IEnumerable<Item> EquippedItems;
        public IEnumerable<Item> InventoryItems;

        public IEnumerable<Item> VaultItems;

        public bool IsVault {
            get
            {
                return VaultItems != null;
            }
        }

        public Inventory(IEnumerable<Item> equippedItems, IEnumerable<Item> inventoryItems, IEnumerable<Item> vaultItems)
        {
            EquippedItems = equippedItems;
            InventoryItems = inventoryItems;
            VaultItems = vaultItems;
        }
    }
}
