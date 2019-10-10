using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DestinyPCLoadoutManager.Controls.Models
{
    class CharacterListItem
    {
        public string Title { get; set; }
        public long Id { get; set; }
        public bool IsSelected { get; set; }
    }
}
