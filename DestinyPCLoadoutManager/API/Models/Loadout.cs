using System;
using System.Collections.Generic;
using System.Text;

namespace DestinyPCLoadoutManager.API.Models
{
    class Loadout
    {
        public IEnumerable<Item> EquippedItems { get; set; }
    }
}
