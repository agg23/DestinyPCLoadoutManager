using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DestinyPCLoadoutManager.Auth.Models
{
    [Serializable]
    class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("refresh_expires_in")]
        public long RefreshExpiresIn { get; set; }

        [JsonProperty("membership_id")]
        public long MembershipId { get; set; }
    }
}
