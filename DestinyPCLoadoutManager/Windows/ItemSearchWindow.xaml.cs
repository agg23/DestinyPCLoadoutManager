using DestinyPCLoadoutManager.API;
using DestinyPCLoadoutManager.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DestinyPCLoadoutManager.Windows
{
    /// <summary>
    /// Interaction logic for ItemSearchWindow.xaml
    /// </summary>
    public partial class ItemSearchWindow : Window
    {
        private InventoryManager inventoryManager;

        private DebounceDispatcher debounceTimer = new DebounceDispatcher();

        public ItemSearchWindow()
        {
            InitializeComponent();

            Topmost = true;

            inventoryManager = App.provider.GetService(typeof(InventoryManager)) as InventoryManager;

            searchBox.TextChanged += SearchBoxChanged;
            searchBox.PreviewKeyDown += SearchBoxPreviewKeyDown;
        }

        private void SearchBoxChanged(object sender, TextChangedEventArgs e)
        {
            debounceTimer.Debounce(100, async _ =>
            {
                var inventories = await inventoryManager.GetAllInventories(true);
                
                if (inventories == null)
                {
                    return;
                }

                var items = inventories.Select(i => i.EquippedItems.Union(i.InventoryItems)).Aggregate((a, b) => a.Union(b));

                var sortedItems = items.Search(searchBox.Text, i => i.Name);

                itemList.SetItems(sortedItems);
            });
        }

        private void SearchBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            {
                // Mark handled
                e.Handled = true;
                Close();
                return;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            searchBox.Focus();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            //this.Topmost = true;
            //this.Activate();
        }
    }
}
