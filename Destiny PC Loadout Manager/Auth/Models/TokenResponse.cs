using System;
using System.Collections.Generic;
using System.Text;

namespace DestinyPCLoadoutManager.Auth.Models
{
    class TokenResponse
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public long membership_id;
    }
}
