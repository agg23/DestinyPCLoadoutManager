using DestinyPCLoadoutManager.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DestinyPCLoadoutManager.Controls
{
    /// <summary>
    /// Interaction logic for LoadoutSaver.xaml
    /// </summary>
    public partial class LoadoutSaver : UserControl
    {
        InventoryManager inventoryManager = App.provider.GetService(typeof(InventoryManager)) as InventoryManager;

        public LoadoutSaver()
        {
            InitializeComponent();

            shortcut.SetShortcut(RemoveShortcut, SaveShortcut);
        }

        public void SaveLoadout(object sender, RoutedEventArgs e)
        {
            _ = inventoryManager.SaveLoadout(0);
        }

        public void RestoreLoadout(object sender, RoutedEventArgs e)
        {
            _ = inventoryManager.EquipLoadout(0);
        }

        private void SaveShortcut(Key key, ModifierKeys modifiers)
        {  
            Logic.InputManager inputManager = App.provider.GetService(typeof(Logic.InputManager)) as Logic.InputManager;
            inputManager.RegisterShortcut("0", key, modifiers, ShortcutAction);
        }

        private void RemoveShortcut(Key key, ModifierKeys modifiers)
        {
            Logic.InputManager inputManager = App.provider.GetService(typeof(Logic.InputManager)) as Logic.InputManager;
            inputManager.UnregisterShortcut("0", key, modifiers);
        }

        private void ShortcutAction()
        {
            _ = inventoryManager.EquipLoadout(0);
        }
    }
}
