using Destiny2;
using Destiny2.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.Auth
{
    class AccountManager
    {
        private DestinyProfileUserInfoCard currentAccount;

        public async Task<DestinyProfileUserInfoCard> GetAccount()
        {
            var destinyApi = App.provider.GetService(typeof(IDestiny2)) as IDestiny2;
            var oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;

            if (!oauthManager.IsAuthorized)
            {
                return null;
            }

            if (currentAccount != null)
            {
                return currentAccount;
            }

            var linkedProfiles = await destinyApi.GetLinkedProfiles(oauthManager.currentToken.access_token, oauthManager.currentToken.membership_id);
            var account = linkedProfiles.Profiles.FirstOrDefault();

            currentAccount = account;

            return account;
        }

    }
}
