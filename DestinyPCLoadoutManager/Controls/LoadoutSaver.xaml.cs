using DestinyPCLoadoutManager.API;
using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Logic;
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

        private Loadout _loadout = new Loadout();
        private Loadout loadout {
            get {
                return _loadout;
            }
            set {
                _loadout = value;

                nameTextBox.Text = value?.Name;
                restoreButton.IsEnabled = value != null;
            }
        }
        private int index;

        private DebounceDispatcher debounceTimer = new DebounceDispatcher();

        public LoadoutSaver()
        {
            InitializeComponent();

            nameTextBox.TextChanged += NameTextBoxChanged;

            shortcut.SetShortcut(RemoveShortcut, SaveShortcut);
        }

        public void SetLoadout(Loadout loadout, int index)
        {
            this.loadout = loadout;
            this.index = index;

            if (loadout.Shortcut != null)
            {
                shortcut.SetShortcut(loadout.Shortcut, RemoveShortcut, SaveShortcut);
                RegisterShortcut(loadout.Shortcut);
            }
        }

        public async void SaveLoadout(object sender, RoutedEventArgs e)
        {
            var temp = loadout;
            loadout = await inventoryManager.GetEquiped();

            loadout.Name = temp.Name;
            loadout.Shortcut = temp.Shortcut;

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

        private void SaveShortcut(Shortcut shortcut)
        {
            loadout.Shortcut = shortcut;

            RegisterShortcut(shortcut);

            SaveLoadoutChanges();
        }

        private void RegisterShortcut(Shortcut shortcut)
        {
            Logic.InputManager inputManager = App.provider.GetService(typeof(Logic.InputManager)) as Logic.InputManager;
            inputManager.RegisterShortcut($"{index}", shortcut, PerformRestore);
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
