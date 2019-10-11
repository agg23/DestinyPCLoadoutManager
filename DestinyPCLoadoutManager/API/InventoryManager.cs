using Destiny2;
using DestinyPCLoadoutManager.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace DestinyPCLoadoutManager.API
{
    class InventoryManager
    {
        private IDestiny2 destinyApi;
        private OAuthManager oauthManager;

        public void SetupServices()
        {
            if (destinyApi != null && oauthManager != null)
            {
                return;
            }

            destinyApi = App.provider.GetService(typeof(IDestiny2)) as IDestiny2;
            oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
        }

        public void GetEquiped(long characterId)
        {
            
        }
    }
}
