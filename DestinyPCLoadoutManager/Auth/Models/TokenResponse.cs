using System;
using System.Collections.Generic;
using System.Text;

namespace DestinyPCLoadoutManager.Auth.Models
{
    class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public long membership_id { get; set; }
    }
}
