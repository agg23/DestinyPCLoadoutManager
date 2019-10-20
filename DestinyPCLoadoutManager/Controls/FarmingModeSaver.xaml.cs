using DestinyPCLoadoutManager.API;
using DestinyPCLoadoutManager.API.Models;
using System.Windows;
using System.Windows.Controls;

namespace DestinyPCLoadoutManager.Controls
{
    /// <summary>
    /// Interaction logic for FarmingModeSaver.xaml
    /// </summary>
    public partial class FarmingModeSaver : UserControl
    {
        InventoryManager inventoryManager = App.provider.GetService(typeof(InventoryManager)) as InventoryManager;

        private Loadout _loadout = new Loadout();
        private Loadout loadout
        {
            get
            {
                return _loadout;
            }
            set
            {
                _loadout = value;

                if (value.Shortcut != null)
                {
                    shortcut.SetShortcut(value.Shortcut, RemoveShortcut, SaveShortcut);
                    RegisterShortcut(value.Shortcut);
                }
            }
        }

        private bool _isFarmMode = false;
        private bool isFarmMode
        {
            get
            {
                return _isFarmMode;
            }
            set
            {
                _isFarmMode = value;

                toggleButton.Content = value ? "Exit Farming Mode" : "Enter Farming Mode";
            }
        }

        public FarmingModeSaver()
        {
            InitializeComponent();

            shortcut.SetShortcut(RemoveShortcut, SaveShortcut);
        }

        public void SetLoadout(Loadout loadout, bool isFarmMode)
        {
            this.loadout = loadout;
            this.isFarmMode = isFarmMode;
        }

        public void ToggleFarm(object sender, RoutedEventArgs e)
        {
            if (isFarmMode)
            {
                PerformRestore();
            }
            else
            {
                SaveLoadout();
            }

            isFarmMode = !isFarmMode;
        }

        public void ResetFarm(object sender, RoutedEventArgs e)
        {
            isFarmMode = false;
            loadout = new Loadout();

            SaveLoadoutChanges();
        }

        public async void SaveLoadout()
        {
            var temp = loadout;
            var newLoadout = await inventoryManager.ClearInventory();

            newLoadout.Name = "Farming";
            newLoadout.Shortcut = temp.Shortcut;

            loadout = newLoadout;

            SaveLoadoutChanges();
        }

        private void PerformRestore()
        {
            if (loadout == null)
            {
                return;
            }

            _ = inventoryManager.EquipLoadout(loadout, true);

            SaveFarmMode();
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
            return inputManager.RegisterShortcut("FarmingMode", shortcut, PerformRestore);
        }

        private void RemoveShortcut(Shortcut shortcut)
        {
            loadout.Shortcut = null;

            Logic.InputManager inputManager = App.provider.GetService(typeof(Logic.InputManager)) as Logic.InputManager;
            inputManager.UnregisterShortcut("FarmingMode", shortcut);

            SaveLoadoutChanges();
        }

        private void SaveLoadoutChanges()
        {
            Properties.Settings.Default.FarmingLoadout = loadout;
            Properties.Settings.Default.IsFarmingMode = isFarmMode;
            Properties.Settings.Default.Save();
        }

        private void SaveFarmMode()
        {
            Properties.Settings.Default.IsFarmingMode = isFarmMode;
            Properties.Settings.Default.Save();
        }
    }
}
