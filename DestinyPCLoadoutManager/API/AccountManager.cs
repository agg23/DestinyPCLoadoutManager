using Destiny2;
using Destiny2.Responses;
using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API
{
    class AccountManager
    {
        private static BungieMembershipType STEAM_MEMBERSHIP = (BungieMembershipType)3;

        private IDestiny2 destinyApi;
        private OAuthManager oauthManager;

        private DestinyProfileUserInfoCard currentAccount;
        private DestinyProfileResponse currentProfile;

        private Dictionary<long, Character> currentCharacters;
        private Inventory vault;

        public void SetupServices()
        {
            if (destinyApi != null && oauthManager != null)
            {
                return;
            }

            destinyApi = App.provider.GetService(typeof(IDestiny2)) as IDestiny2;
            oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
        }

        public async Task<DestinyProfileUserInfoCard> GetAccount(bool preventSet = false)
        {
            SetupServices();

            if (!oauthManager.IsAuthorized)
            {
                return null;
            }

            if (currentAccount != null)
            {
                return currentAccount;
            }

            var linkedProfiles = await destinyApi.GetLinkedProfiles(oauthManager.currentToken.access_token, oauthManager.currentToken.membership_id);

            if (linkedProfiles == null)
            {
                return null;
            }

            var account = linkedProfiles.Profiles.FirstOrDefault();

            if (!preventSet)
            {
                currentAccount = account;
            }

            return account;
        }

        public async Task<DestinyProfileResponse> GetProfile()
        {
            if (currentProfile != null)
            {
                return currentProfile;
            }

            var account = await GetAccount();

            var profile = await destinyApi.GetProfile(oauthManager.currentToken.access_token, STEAM_MEMBERSHIP, account.MembershipId);

            currentProfile = profile;

            return profile;
        }

        public async Task<DestinyCharacterResponse> GetCharacterInventoryResponse(long id)
        {
            var account = await GetAccount();

            DestinyComponentType[] types = new DestinyComponentType[]
            {
                DestinyComponentType.Profiles,
                DestinyComponentType.Characters,
                DestinyComponentType.CharacterInventories,
                DestinyComponentType.CharacterEquipment,
            };

            return await destinyApi.GetCharacterInfo(oauthManager.currentToken.access_token, STEAM_MEMBERSHIP,
                account.MembershipId, id, types);
        }

        public async Task<Character> GetCharacter(long id)
        {
            var response = await GetCharacterInventoryResponse(id);
            return await Character.BuildCharacter(id, response);
        }

        public async Task<Dictionary<long, Character>> GetCharacters()
        {
            if (currentCharacters != null)
            {
                return currentCharacters;
            }
            
            var profile = await GetProfile();

            var characterIds = profile.Profile?.Data?.CharacterIds ?? new List<long>();

            if (characterIds.Count() < 1)
            {
                // No characters
                return null;
            }

            DestinyComponentType[] types = new DestinyComponentType[]
            {
                DestinyComponentType.Profiles,
                DestinyComponentType.Characters,
                DestinyComponentType.CharacterInventories,
                DestinyComponentType.CharacterEquipment,
            };

            var characters = await Task.WhenAll(characterIds.Select(GetCharacter));
            currentCharacters = characters.ToDictionary(c => c.Id, c => c);
            return currentCharacters;
        }

        public async Task<Tuple<Character, bool>> GetCurrentCharacter()
        {
            var didFetch = currentCharacters == null;
            var characters = await GetCharacters();
            return Tuple.Create(characters.GetValueOrDefault(Properties.Settings.Default.SelectedGuardian), didFetch);
        }

        public async Task<Inventory> GetVault()
        {
            if (vault != null)
            {
                return vault;
            }
            
            var account = await GetAccount();
            var profile = await GetProfile();

            var characters = profile.Profile?.Data?.CharacterIds ?? new List<long>();

            if (characters.Count() < 1)
            {
                // No characters
                return null;
            }

            DestinyComponentType[] types = new DestinyComponentType[]
            {
                DestinyComponentType.ProfileInventories,
            };

            var character = await destinyApi.GetCharacterInfo(oauthManager.currentToken.access_token, STEAM_MEMBERSHIP, account.MembershipId, characters.First(), types);
            vault = await Inventory.BuildVaultInventory(character);
            return vault;
        }
    }
}
