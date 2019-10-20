using System;
using System.Windows.Input;

namespace DestinyPCLoadoutManager.API.Models
{
    [Serializable]
    public class Shortcut
    {
        public Key Key { get; set; }
        public ModifierKeys Modifiers { get; set; }

        public Shortcut(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }
    }
}
