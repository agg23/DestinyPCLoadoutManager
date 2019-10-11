using Destiny2;
using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API
{
    class InventoryManager
    {
        private IDestiny2 destinyApi;
        private OAuthManager oauthManager;
        private AccountManager accountManager;

        public void SetupServices()
        {
            if (destinyApi != null && oauthManager != null)
            {
                return;
            }

            destinyApi = App.provider.GetService(typeof(IDestiny2)) as IDestiny2;
            oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
            accountManager = App.provider.GetService(typeof(AccountManager)) as AccountManager;
        }

        public async Task<IEnumerable<Item>> GetEquiped()
        {
            var characterTuple = await accountManager.GetCurrentCharacter();
            var character = characterTuple.Item1;

            if (!characterTuple.Item2)
            {
                // If we didn't fetch, refetch
                var inventoryResponse = await accountManager.GetCharacterInventoryResponse(character.Id);
                await character.UpdateInventory(inventoryResponse);
            }

            return character.Inventory.EquippedItems;
        }

        public async Task SaveLoadout(int index = -1)
        {
            var characterTuple = await accountManager.GetCurrentCharacter();
            var character = characterTuple.Item1;

            var loadout = new Loadout { EquippedItems = character.Inventory.EquippedItems };
            var savedLoadouts = Properties.Settings.Default.Loadouts;

            var newLoadouts = savedLoadouts != null ? new List<Loadout>(savedLoadouts) : new List<Loadout>();
            if (index == -1)
            {
                newLoadouts.Add(loadout);
            }
            else
            {
                if (index >= newLoadouts.Count)
                {
                    return;
                }
                
                newLoadouts[index] = loadout;
            }

            Properties.Settings.Default.Loadouts = newLoadouts;
            Properties.Settings.Default.Save();
        }
    }
}
