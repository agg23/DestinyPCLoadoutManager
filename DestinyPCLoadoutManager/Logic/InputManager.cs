using DestinyPCLoadoutManager.API.Models;
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
        
        public bool RegisterShortcut(string name, Shortcut shortcut, Action action)
        {
            var hash = hashForShortcut(shortcut);

            if (registeredKeys.Contains(hash))
            {
                return false;
            }

            registeredKeys.Add(hash);
            
            try
            {
                HotkeyManager.Current.AddOrReplace(name, shortcut.Key, shortcut.Modifiers, new EventHandler<NHotkey.HotkeyEventArgs>((sender, e) => action()));
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void UnregisterShortcut(string name, Shortcut shortcut)
        {
            HotkeyManager.Current.Remove(name);

            var hash = hashForShortcut(shortcut);
            registeredKeys.Remove(hash);
        }

        private string hashForShortcut(Shortcut shortcut)
        {
            return $"{shortcut.Key},{shortcut.Modifiers}";
        }
    }
}
