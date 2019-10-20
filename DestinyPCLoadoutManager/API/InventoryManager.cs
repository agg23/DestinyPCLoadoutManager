using Destiny2;
using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Auth;
using DestinyPCLoadoutManager.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Loadout> GetEquiped()
        {
            var characterTuple = await accountManager.GetCurrentCharacter();
            var character = characterTuple.Item1;

            if (!characterTuple.Item2)
            {
                // If we didn't fetch, refetch
                var inventoryResponse = await accountManager.GetCharacterInventoryResponse(character.Id);
                await character.UpdateInventory(inventoryResponse);
            }

            return new Loadout { EquippedItems = character.Inventory.EquippedItems };
        }

        public Loadout SaveLoadout(Loadout loadout, int index)
        {
            var savedLoadouts = Properties.Settings.Default.Loadouts;
            var newLoadouts = savedLoadouts != null ? new List<Loadout>(savedLoadouts) : new List<Loadout>();
            if (index == -1 || index == newLoadouts.Count)
            {
                newLoadouts.Add(loadout);
            }
            else
            {
                if (index > newLoadouts.Count)
                {
                    return loadout;
                }

                newLoadouts[index] = loadout;
            }

            Properties.Settings.Default.Loadouts = newLoadouts;
            Properties.Settings.Default.Save();

            return loadout;
        }

        public async Task EquipLoadout(Loadout loadout)
        {
            // TODO: Loadout diffing appears to be insufficient, as Bungie will often
            // return a cached state of your loadout. Some cache busting method is required
            var equippedItems = await GetEquiped();
            //var missingItems = savedLoadout.Difference(equippedItems);
            /*if (!missingItems.EquippedItems.Any())
            {
                return;
            }*/

            var characterTuple = await accountManager.GetCurrentCharacter();

            var allInventoryItems = characterTuple.Item1.Inventory.InventoryItems.Union(equippedItems.EquippedItems);

            // Transfer items (preferrably we diff instead)
            var itemsMissingFromInventory = loadout.EquippedItems.Difference(allInventoryItems, item => item.Id);
            foreach(var missingItem in itemsMissingFromInventory)
            {
                await Util.RequestAndRetry(() => destinyApi.TransferItem(oauthManager.currentToken.AccessToken, BungieMembershipType.TigerSteam, characterTuple.Item1.Id, missingItem.Id, false));
                await Task.Delay(100);
            }

            // Sort items to apply exotics last; ensuring any other exotic is removed before
            // attempting to insert a new exotic
            var sortedItems = loadout.EquippedItems.OrderBy(item => item.Tier);

            await Util.RequestAndRetry(() => destinyApi.EquipItems(oauthManager.currentToken.AccessToken, BungieMembershipType.TigerSteam, characterTuple.Item1.Id, sortedItems.Select(item => item.Id).ToArray()));
        }
    }
}
