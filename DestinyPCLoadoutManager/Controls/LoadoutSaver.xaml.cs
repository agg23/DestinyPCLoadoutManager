using DestinyPCLoadoutManager.API;
using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Logic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DestinyPCLoadoutManager.Controls
{
    /// <summary>
    /// Interaction logic for LoadoutSaver.xaml
    /// </summary>
    public partial class LoadoutSaver : UserControl
    {
        InventoryManager inventoryManager = App.provider.GetService(typeof(InventoryManager)) as InventoryManager;

        private Loadout _loadout;
        private Loadout loadout {
            get {
                return _loadout;
            }
            set {
                _loadout = value;

                nameTextBox.Text = value?.Name;
                restoreButton.IsEnabled = value != null && ((value.EquippedItems?.Any() ?? false) || (value.InventoryItems?.Any() ?? false));

                if (value.Shortcut != null)
                {
                    shortcut.SetShortcut(value.Shortcut, RemoveShortcut, SaveShortcut);
                    RegisterShortcut(value.Shortcut);
                }
            }
        }
        private int index;

        private DebounceDispatcher debounceTimer = new DebounceDispatcher();

        public LoadoutSaver()
        {
            InitializeComponent();

            loadout = new Loadout();

            nameTextBox.TextChanged += NameTextBoxChanged;

            shortcut.SetShortcut(RemoveShortcut, SaveShortcut);
        }

        public void SetLoadout(Loadout loadout, int index)
        {
            this.loadout = loadout;
            this.index = index;
        }

        public async void SaveLoadout(object sender, RoutedEventArgs e)
        {
            var temp = loadout;
            var newLoadout = await inventoryManager.GetEquiped();

            newLoadout.Name = temp.Name;
            newLoadout.Shortcut = temp.Shortcut;

            loadout = newLoadout;

            SaveLoadoutChanges();
        }

        public void RestoreLoadout(object sender, RoutedEventArgs e)
        {
            PerformRestore();
        }

        private void PerformRestore()
        {
            if (loadout == null)
            {
                return;
            }
            
            _ = inventoryManager.EquipLoadout(loadout);
        }

        private void NameTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            debounceTimer.Debounce(500, _ =>
            {
                if (loadout.Name == nameTextBox.Text)
                {
                    return;
                }

                loadout.Name = nameTextBox.Text;

                SaveLoadoutChanges();
            });
        }

        private bool SaveShortcut(Shortcut shortcut)
        {
            loadout.Shortcut = shortcut;

            if (RegisterShortcut(shortcut))
            {
                SaveLoadoutChanges();
                return true;
            }

            return false;
        }

        private bool RegisterShortcut(Shortcut shortcut)
        {
            Logic.InputManager inputManager = App.provider.GetService(typeof(Logic.InputManager)) as Logic.InputManager;
            return inputManager.RegisterShortcut($"{index}", shortcut, PerformRestore);
        }

        private void RemoveShortcut(Shortcut shortcut)
        {
            loadout.Shortcut = null;

            Logic.InputManager inputManager = App.provider.GetService(typeof(Logic.InputManager)) as Logic.InputManager;
            inputManager.UnregisterShortcut($"{index}", shortcut);

            SaveLoadoutChanges();
        }

        private void SaveLoadoutChanges()
        {
            inventoryManager.SaveLoadout(loadout, index);
        }
    }
}
