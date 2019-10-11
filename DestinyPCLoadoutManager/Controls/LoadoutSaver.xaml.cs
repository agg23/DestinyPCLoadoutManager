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
        }

        public void SaveLoadout(object sender, RoutedEventArgs e)
        {
            _ = inventoryManager.SaveLoadout();
        }

        public void RestoreLoadout(object sender, RoutedEventArgs e)
        {

        }
    }
}
