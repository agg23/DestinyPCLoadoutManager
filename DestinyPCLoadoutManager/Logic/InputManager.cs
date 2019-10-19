using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DestinyPCLoadoutManager.Logic
{
    class InputManager
    {
        private HashSet<string> registeredKeys = new HashSet<string>();
        
        public bool RegisterShortcut(string name, Key key, ModifierKeys modifiers, Action action)
        {
            var hash = hashForKeyCombo(key, modifiers);

            if (registeredKeys.Contains(hash))
            {
                return false;
            }

            registeredKeys.Add(hash);
            
            HotkeyManager.Current.AddOrReplace(name, key, modifiers, new EventHandler<NHotkey.HotkeyEventArgs>((sender, e) => action()));

            return true;
        }

        public void UnregisterShortcut(string name, Key key, ModifierKeys modifiers)
        {
            HotkeyManager.Current.Remove(name);

            var hash = hashForKeyCombo(key, modifiers);
            registeredKeys.Remove(hash);
        }

        private string hashForKeyCombo(Key key, ModifierKeys modifiers)
        {
            return $"{key},{modifiers}";
        }
    }
}
