using DestinyPCLoadoutManager.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DestinyPCLoadoutManager.API
{
    class Util
    {
        private static OAuthManager oauthManager;

        private static OAuthManager GetOAuthManager()
        {
            if (oauthManager != null)
            {
                return oauthManager;
            }

            oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
            return oauthManager;
        }

        public static async Task<T> RequestAndRetry<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (UnauthorizedAccessException _)
            {
                // Unauthorized. Attempt reauth and run again
                var manager = GetOAuthManager();
                if (manager == null || manager.RefreshToken() == null)
                {
                    return default(T);
                }

                return await func();
            }
        }
    }
}
