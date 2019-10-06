using System;
using System.Collections.Generic;
using System.Text;

namespace Destiny_PC_Loadout_Manager.Auth.Models
{
    class TokenResponse
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public long membership_id;
    }
}
